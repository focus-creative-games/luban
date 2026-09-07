// Copyright 2025 Code Philosophy

using System.Text;
using Luban.Pipeline;
using Luban.Schema;
using NLog;
using Xunit;

namespace Luban.Tests.Helpers;

/// <summary>
/// Serializes pipeline runs so plugin registration / managers do not race across tests.
/// </summary>
[CollectionDefinition("LubanPipeline", DisableParallelization = true)]
public class LubanPipelineCollection : ICollectionFixture<object>
{
}

public sealed class LubanRunResult
{
    public string CodeDir { get; init; }

    public string DataDir { get; init; }

    public bool AnyValidatorFail { get; init; }

    public Exception Exception { get; init; }

    public bool Succeeded => Exception == null;
}

public static class LubanRunner
{
    private static bool s_nlogConfigured;
    private static readonly object s_lock = new();

    public static LubanRunResult Run(
        string confPath,
        string target = "server",
        IEnumerable<string> codeTargets = null,
        IEnumerable<string> dataTargets = null,
        string outputCodeDir = null,
        string outputDataDir = null,
        bool forceLoadTableDatas = false,
        IEnumerable<string> includeTags = null,
        IEnumerable<string> excludeTags = null,
        Dictionary<string, string> variants = null,
        Dictionary<string, string> extraOptions = null,
        bool throwOnError = true)
    {
        EnsureLogging();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        confPath = Path.GetFullPath(confPath);
        if (!File.Exists(confPath))
        {
            throw new FileNotFoundException("luban.conf not found", confPath);
        }

        var config = new GlobalConfigLoader().Load(confPath);
        var options = new Dictionary<string, string>();
        if (config.Xargs != null)
        {
            foreach (var xarg in config.Xargs)
            {
                var pair = xarg.Split('=', 2);
                if (pair.Length == 2)
                {
                    options[pair[0]] = pair[1];
                }
            }
        }

        if (!string.IsNullOrEmpty(outputCodeDir))
        {
            options[BuiltinOptionNames.OutputCodeDir] = outputCodeDir;
        }

        if (!string.IsNullOrEmpty(outputDataDir))
        {
            options[BuiltinOptionNames.OutputDataDir] = outputDataDir;
        }

        if (extraOptions != null)
        {
            foreach (var kv in extraOptions)
            {
                options[kv.Key] = kv.Value;
            }
        }

        var args = new PipelineArguments
        {
            Target = target,
            ForceLoadTableDatas = forceLoadTableDatas,
            SchemaCollector = "default",
            Config = config,
            OutputTables = new List<string>(),
            CodeTargets = codeTargets?.ToList() ?? new List<string>(),
            DataTargets = dataTargets?.ToList() ?? new List<string>(),
            IncludeTags = includeTags?.ToList() ?? new List<string>(),
            ExcludeTags = excludeTags?.ToList() ?? new List<string>(),
            Variants = variants ?? new Dictionary<string, string>(),
            TimeZone = null,
        };

        bool validatorFail = false;
        Exception caught = null;
        try
        {
            using var scope = PipelineScope.Create(options, scanPluginAssemblies: true);
            using (scope.Enter())
            {
                scope.Config = config;
                var pipeline = scope.Pipelines.CreatePipeline("default");
                scope.Pipeline = pipeline;
                pipeline.Run(args);
                validatorFail = scope.GenerationContext.AnyValidatorFail;
            }
        }
        catch (Exception e)
        {
            caught = e;
            if (throwOnError)
            {
                throw;
            }
        }

        return new LubanRunResult
        {
            CodeDir = outputCodeDir,
            DataDir = outputDataDir,
            AnyValidatorFail = validatorFail,
            Exception = caught,
        };
    }

    public static LubanRunResult GenerateToTemp(
        string fixtureName,
        string target = "server",
        IEnumerable<string> codeTargets = null,
        IEnumerable<string> dataTargets = null,
        bool forceLoadTableDatas = false,
        IEnumerable<string> includeTags = null,
        IEnumerable<string> excludeTags = null,
        Dictionary<string, string> variants = null,
        Dictionary<string, string> extraOptions = null,
        bool throwOnError = true,
        TempWorkspace workspace = null)
    {
        workspace ??= new TempWorkspace();
        return Run(
            TestPaths.FixtureConf(fixtureName),
            target,
            codeTargets,
            dataTargets,
            workspace.CodeDir,
            workspace.DataDir,
            forceLoadTableDatas,
            includeTags,
            excludeTags,
            variants,
            extraOptions,
            throwOnError);
    }

    private static void EnsureLogging()
    {
        lock (s_lock)
        {
            if (s_nlogConfigured)
            {
                return;
            }

            LogManager.Setup().LoadConfiguration(builder =>
            {
                builder.ForLogger().FilterMinLevel(LogLevel.Warn).WriteToConsole();
            });
            Diagnostics.MessageCatalog.Init("en");
            s_nlogConfigured = true;
        }
    }
}
