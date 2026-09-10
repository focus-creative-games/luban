// Copyright 2026 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using CommandLine;
using Luban;
using Luban.Diagnostics;
using Luban.Pipeline;
using Luban.Schema;
using Luban.Tmpl;
using NLog;
using System.Text;

namespace Luban.Agent;

internal static class Program
{
    private static readonly HashSet<string> s_modes = new(StringComparer.OrdinalIgnoreCase)
    {
        "validate", "schema", "list-tables", "describe", "capabilities",
    };

    private class Options
    {
        [Option("mode", Required = false, HelpText = "validate|schema|list-tables|describe|capabilities")]
        public string Mode { get; set; }

        [Option("name", Required = false, HelpText = "Name filter / describe target")]
        public string Name { get; set; }

        [Option('s', "schemaCollector", Required = false, Default = "default")]
        public string SchemaCollector { get; set; } = "default";

        [Option("conf", Required = false, HelpText = "luban.conf path")]
        public string ConfigFile { get; set; }

        [Option('t', "target", Required = false, HelpText = "export target")]
        public string Target { get; set; }

        [Option('o', "outputTable", Required = false)]
        public IEnumerable<string> OutputTables { get; set; }

        [Option('i', "includeTag", Required = false)]
        public IEnumerable<string> IncludeTags { get; set; }

        [Option('e', "excludeTag", Required = false)]
        public IEnumerable<string> ExcludeTags { get; set; }

        [Option("variant", Required = false, HelpText = "field/table variants")]
        public IEnumerable<string> Variants { get; set; }

        [Option("timeZone", Required = false)]
        public string TimeZone { get; set; }

        [Option("customTemplateDir", Required = false)]
        public IEnumerable<string> CustomTemplateDirs { get; set; }

        [Option("locale", Required = false)]
        public string Locale { get; set; }

        [Option('x', "xargs", Required = false)]
        public IEnumerable<string> Xargs { get; set; }

        [Option('p', "pipeline", Required = false, Default = "default")]
        public string Pipeline { get; set; } = "default";

        [Option('l', "logConfig", Required = false, Default = "nlog.xml")]
        public string LogConfig { get; set; } = "nlog.xml";

        [Value(0, MetaName = "mode", Required = false, HelpText = "mode as first positional arg")]
        public string PositionalMode { get; set; }
    }

    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var opts = ParseArgs(args);
        string mode = ResolveMode(opts);
        SetupLogging(opts);

        if (string.IsNullOrEmpty(mode) || !s_modes.Contains(mode))
        {
            Exit(AgentResult.Usage(
                $"Unknown or missing mode. Expected: {string.Join('|', s_modes.OrderBy(x => x))}"));
            return;
        }

        mode = mode.ToLowerInvariant();
        if (mode == "capabilities")
        {
            Exit(AgentResult.Success("capabilities", BuildCapabilities()));
            return;
        }

        if (string.IsNullOrWhiteSpace(opts.ConfigFile) || string.IsNullOrWhiteSpace(opts.Target))
        {
            Exit(AgentResult.Usage("--conf and -t/--target are required for this mode"));
            return;
        }

        if (mode == "describe" && string.IsNullOrWhiteSpace(opts.Name))
        {
            Exit(AgentResult.Usage("--name is required for mode=describe"));
            return;
        }

        try
        {
            switch (mode)
            {
                case "validate":
                    RunValidate(opts);
                    break;
                case "schema":
                case "list-tables":
                case "describe":
                    RunSchemaQuery(opts, mode);
                    break;
            }
        }
        catch (Exception e)
        {
            Exit(AgentResult.Fail(mode, e));
        }
    }

    private static string ResolveMode(Options opts)
    {
        if (!string.IsNullOrWhiteSpace(opts.Mode))
        {
            return opts.Mode.Trim();
        }
        return opts.PositionalMode?.Trim();
    }

    private static void SetupLogging(Options opts)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        LogManager.Setup().LoadConfigurationFromFile(opts.LogConfig);
        MessageCatalog.Init(opts.Locale);
    }

    private static Options ParseArgs(string[] args)
    {
        var help = new StringWriter();
        var parser = new Parser(s =>
        {
            s.AllowMultiInstance = true;
            s.HelpWriter = help;
            s.IgnoreUnknownArguments = false;
        });
        var result = parser.ParseArguments<Options>(args);
        if (result.Tag == ParserResultType.NotParsed)
        {
            Console.Error.WriteLine(help.ToString());
            Exit(AgentResult.Usage(help.ToString().Trim()));
        }
        return ((Parsed<Options>)result).Value;
    }

    private static object BuildCapabilities()
    {
        return new
        {
            tool = "Luban.Agent",
            agentResultVersion = AgentResult.FormatVersion,
            schemaJsonVersion = SchemaJsonExporter.FormatVersion,
            modes = s_modes.OrderBy(x => x).ToList(),
            companion = new
            {
                generate = "Use main Luban.dll for code/data generation; prefer --errorFormat json for machine-readable errors",
                mcp = "Luban.Mcp for IDE MCP integration",
            },
            exitCodes = new
            {
                ok = AgentExitCode.Ok,
                dataOrValidation = AgentExitCode.DataOrValidation,
                schemaOrConfig = AgentExitCode.SchemaOrConfig,
                usage = AgentExitCode.Usage,
                @internal = AgentExitCode.Internal,
            },
        };
    }

    private static void RunValidate(Options opts)
    {
        var config = new GlobalConfigLoader().Load(opts.ConfigFile);
        var xargs = MergeXargs(config.Xargs, opts.Xargs);
        xargs["outputSaver"] = "null";

        using var scope = PipelineScope.Create(xargs);
        using (scope.Enter())
        {
            scope.Config = config;
            AddCustomTemplateDirs(opts.CustomTemplateDirs);

            var pipeline = scope.Pipelines.CreatePipeline(opts.Pipeline);
            scope.Pipeline = pipeline;
            pipeline.Run(new PipelineArguments
            {
                Target = opts.Target,
                ForceLoadTableDatas = true,
                SchemaCollector = opts.SchemaCollector,
                Config = config,
                OutputTables = opts.OutputTables?.ToList() ?? new List<string>(),
                CodeTargets = new List<string>(),
                DataTargets = new List<string>(),
                IncludeTags = opts.IncludeTags?.ToList() ?? new List<string>(),
                ExcludeTags = opts.ExcludeTags?.ToList() ?? new List<string>(),
                Variants = ParseKeyValues(opts.Variants, "variant"),
                TimeZone = opts.TimeZone,
            });

            if (scope.GenerationContext.AnyValidatorFail)
            {
                Exit(AgentResult.FromReport("validate", DiagnosticReport.ValidationFailed()));
                return;
            }
            Exit(AgentResult.Success("validate", new { validated = true }));
        }
    }

    private static void RunSchemaQuery(Options opts, string mode)
    {
        var config = new GlobalConfigLoader().Load(opts.ConfigFile);
        var xargs = MergeXargs(config.Xargs, opts.Xargs);

        using var scope = PipelineScope.Create(xargs);
        using (scope.Enter())
        {
            scope.Config = config;
            AddCustomTemplateDirs(opts.CustomTemplateDirs);

            var args = new PipelineArguments
            {
                Target = opts.Target,
                SchemaCollector = opts.SchemaCollector,
                Config = config,
                OutputTables = opts.OutputTables?.ToList() ?? new List<string>(),
                CodeTargets = new List<string>(),
                DataTargets = new List<string>(),
                IncludeTags = opts.IncludeTags?.ToList() ?? new List<string>(),
                ExcludeTags = opts.ExcludeTags?.ToList() ?? new List<string>(),
                Variants = ParseKeyValues(opts.Variants, "variant"),
                TimeZone = opts.TimeZone,
            };

            var ctx = SchemaCompilation.Compile(args);
            scope.GenerationContext = ctx;
            var doc = SchemaJsonExporter.Build(ctx);

            object result = mode switch
            {
                "schema" => FilterSchema(doc, opts.Name),
                "list-tables" => ListTables(doc, opts.Name),
                "describe" => Describe(doc, opts.Name),
                _ => null,
            };

            if (mode == "describe" && result == null)
            {
                Exit(new AgentResult
                {
                    Ok = false,
                    ExitCode = AgentExitCode.SchemaOrConfig,
                    Command = mode,
                    Errors =
                    {
                        new DiagnosticItem
                        {
                            Category = "schema",
                            Code = "error.cli.name_not_found",
                            Message = $"No table/bean/enum matched --name '{opts.Name}'",
                        },
                    },
                });
                return;
            }

            Exit(AgentResult.Success(mode, result));
        }
    }

    private static object FilterSchema(SchemaJsonDocument doc, string nameFilter)
    {
        if (string.IsNullOrWhiteSpace(nameFilter))
        {
            return doc;
        }
        bool Match(string fullName) =>
            fullName != null && fullName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase);

        return new SchemaJsonDocument
        {
            Version = doc.Version,
            Target = doc.Target,
            Manager = doc.Manager,
            TopModule = doc.TopModule,
            Groups = doc.Groups,
            Tables = doc.Tables.Where(t => Match(t.FullName)).ToList(),
            Beans = doc.Beans.Where(b => Match(b.FullName)).ToList(),
            Enums = doc.Enums.Where(e => Match(e.FullName)).ToList(),
        };
    }

    private static object ListTables(SchemaJsonDocument doc, string nameFilter)
    {
        IEnumerable<SchemaJsonTable> tables = doc.Tables;
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            tables = tables.Where(t => t.FullName.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
        }
        return tables.Select(t => new
        {
            t.FullName,
            t.ValueType,
            t.Mode,
            t.Index,
            t.Comment,
            t.InputFiles,
            t.Groups,
            t.Source,
        }).ToList();
    }

    private static object Describe(SchemaJsonDocument doc, string name)
    {
        var table = doc.Tables.FirstOrDefault(t =>
            t.FullName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            t.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (table != null)
        {
            var bean = doc.Beans.FirstOrDefault(b =>
                b.FullName.Equals(table.ValueType, StringComparison.OrdinalIgnoreCase) ||
                b.FullName.EndsWith("." + table.ValueType, StringComparison.OrdinalIgnoreCase) ||
                b.Name.Equals(table.ValueType, StringComparison.OrdinalIgnoreCase));
            return new { kind = "table", table, valueType = bean };
        }

        var beanOnly = doc.Beans.FirstOrDefault(b =>
            b.FullName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            b.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            b.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (beanOnly != null)
        {
            return new { kind = "bean", bean = beanOnly };
        }

        var en = doc.Enums.FirstOrDefault(e =>
            e.FullName.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            e.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase));
        return en != null ? new { kind = "enum", @enum = en } : null;
    }

    private static void AddCustomTemplateDirs(IEnumerable<string> dirs)
    {
        if (dirs == null)
        {
            return;
        }
        foreach (var dir in dirs)
        {
            TemplateManager.Ins.AddTemplateSearchPath(dir, true, true);
        }
    }

    private static Dictionary<string, string> MergeXargs(IEnumerable<string> defaults, IEnumerable<string> overrides)
    {
        var map = ParseKeyValues(defaults, "xargs");
        foreach (var kv in ParseKeyValues(overrides, "xargs"))
        {
            map[kv.Key] = kv.Value;
        }
        return map;
    }

    private static Dictionary<string, string> ParseKeyValues(IEnumerable<string> items, string kind)
    {
        var result = new Dictionary<string, string>();
        if (items == null)
        {
            return result;
        }
        string invalidKey = kind == "variant" ? "error.cli.invalid_variant" : "error.cli.invalid_xargs";
        string dupKey = kind == "variant" ? "error.cli.duplicate_variant" : "error.cli.duplicate_xargs";
        foreach (var item in items)
        {
            string[] pair = item.Split('=', 2);
            if (pair.Length != 2)
            {
                throw new LubanException(invalidKey, item);
            }
            if (!result.TryAdd(pair[0], pair[1]))
            {
                throw new LubanException(dupKey, item);
            }
        }
        return result;
    }

    private static void Exit(AgentResult result)
    {
        Console.Out.WriteLine(result.ToJson());
        Environment.Exit(result.ExitCode);
    }
}
