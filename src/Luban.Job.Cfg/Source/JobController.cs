using Bright.Time;
using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Generate;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Cfg
{
    [Controller("cfg")]
    public class JobController : IJobController
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        class GenArgs : GenArgsBase
        {

            [Option("input_data_dir", Required = true, HelpText = "input data dir")]
            public string InputDataDir { get; set; }

            [Option('v', "validate_root_dir", Required = false, HelpText = "validate root directory")]
            public string ValidateRootDir { get; set; }

            [Option("output_data_dir", Required = true, HelpText = "output data directory")]
            public string OutputDataDir { get; set; }

            [Option("output_data_resource_list_file", Required = false, HelpText = "output resource list file")]
            public string OutputDataResourceListFile { get; set; }

            [Option("output_data_json_monolithic_file", Required = false, HelpText = "output monolithic json file")]
            public string OutputDataJsonMonolithicFile { get; set; }

            [Option("gen_types", Required = true, HelpText = "code_cs_bin,code_cs_json,code_lua_bin,code_java_bin,code_go_bin,code_go_json,code_cpp_bin,code_python27_json,code_python3_json,code_typescript_bin,code_typescript_json,data_bin,data_lua,data_json,data_json_monolithic . can be multi")]
            public string GenType { get; set; }

            [Option('s', "service", Required = true, HelpText = "service")]
            public string Service { get; set; }

            [Option("export_test_data", Required = false, HelpText = "export test data")]
            public bool ExportTestData { get; set; } = false;

            [Option('t', "l10n_timezone", Required = false, HelpText = "timezone")]
            public string TimeZone { get; set; }

            [Option("input_l10n_text_files", Required = false, HelpText = "input l10n text table files. can be multi, sep by ','")]
            public string InputTextTableFiles { get; set; }

            [Option("l10n_text_field_name", Required = false, HelpText = "text value field name of text table files. default is text")]
            public string TextValueFieldName { get; set; }

            [Option("output_l10n_not_converted_text_file", Required = false, HelpText = "the file save not converted l10n texts.")]
            public string OutputNotConvertTextFile { get; set; }

            [Option("branch", Required = false, HelpText = "branch name")]
            public string BranchName { get; set; }

            [Option("branch_input_data_dir", Required = false, HelpText = "branch input data root dir")]
            public string BranchInputDataDir { get; set; }
        }

        private ICfgCodeRender CreateCodeRender(string genType)
        {
            switch (genType)
            {
                case "code_cs_bin": return new CsCodeBinRender();
                case "code_cs_json": return new CsCodeJsonRender();
                case "code_java_bin": return new JavaCodeBinRender();
                case "code_go_bin": return new GoCodeBinRender();
                case "code_go_json": return new GoCodeJsonRender();
                case "code_cpp_bin": return new CppCodeBinRender();
                case "code_lua_bin": return new LuaCodeBinRender();
                case "code_lua_lua": return new LuaCodeLuaRender();
                case "code_python27_json": return new Python27CodeJsonRender();
                case "code_python3_json": return new Python3CodeJsonRender();
                case "code_typescript_bin": return new TypescriptCodeBinRender();
                case "code_typescript_json": return new TypescriptCodeJsonRender();
                case "code_cpp_ue_editor": return new UE4EditorCppRender();
                case "code_cpp_ue_bp": return new UE4BpCppRender();
                case "code_cs_unity_editor": return new EditorCsRender();
                default: throw new ArgumentException($"not support gen type:{genType}");
            }
        }

        private ELanguage GetLanguage(string genType)
        {
            switch (genType)
            {
                case "code_cs_bin":
                case "code_cs_json": return ELanguage.CS;
                case "code_java_bin": return ELanguage.JAVA;
                case "code_go_bin":
                case "code_go_json": return ELanguage.GO;
                case "code_cpp_bin": return ELanguage.CPP;
                case "code_lua_bin":
                case "code_lua_lua": return ELanguage.LUA;
                case "code_python27_json":
                case "code_python3_json": return ELanguage.PYTHON;
                case "code_typescript_bin":
                case "code_typescript_json": return ELanguage.TYPESCRIPT;
                case "code_cpp_ue_editor":
                case "code_cpp_ue_bp": return ELanguage.CPP;
                case "code_cs_unity_editor": return ELanguage.CS;
                default: throw new ArgumentException($"not support output data type:{genType}");
            }
        }

        private string GetOutputFileSuffix(string genType)
        {
            switch (genType)
            {
                case "data_bin": return "bin";
                case "data_json": return "json";
                case "data_lua": return "lua";
                default: throw new Exception($"not support output data type:{genType}");
            }
        }

        private string GetOutputFileName(string genType, string fileName)
        {
            return $"{(genType.EndsWith("lua") ? fileName.Replace('.', '_') : fileName)}.{GetOutputFileSuffix(genType)}";
        }

        private static bool TryParseArg(List<string> args, out GenArgs options, out string errMsg)
        {
            var helpWriter = new StringWriter();
            var parser = new Parser(ps =>
            {
                ps.HelpWriter = helpWriter;
            }); ;
            var parseResult = parser.ParseArguments<GenArgs>(args);
            if (parseResult.Tag == ParserResultType.NotParsed)
            {
                errMsg = helpWriter.ToString();
                options = null;
                return false;
            }
            else
            {
                options = (parseResult as Parsed<GenArgs>).Value;
                errMsg = null;

                string inputDataDir = options.InputDataDir;
                string outputCodeDir = options.OutputCodeDir;
                string outputDataDir = options.OutputDataDir;

                var genTypes = options.GenType.Split(',').Select(s => s.Trim()).ToList();

                if (genTypes.Any(t => t.StartsWith("code_", StringComparison.Ordinal)) && string.IsNullOrWhiteSpace(outputCodeDir))
                {
                    errMsg = "--outputcodedir missing";
                    return false;
                }
                if (genTypes.Any(t => t.StartsWith("data_", StringComparison.Ordinal)))
                {
                    if (string.IsNullOrWhiteSpace(inputDataDir))
                    {
                        errMsg = "--inputdatadir missing";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(outputDataDir))
                    {
                        errMsg = "--outputdatadir missing";
                        return false;
                    }
                    if (genTypes.Contains("data_resources") && string.IsNullOrWhiteSpace(options.OutputDataResourceListFile))
                    {
                        errMsg = "--output_data_resource_list_file missing";
                        return false;
                    }
                    if (genTypes.Contains("output_data_json_monolithic_file") && string.IsNullOrWhiteSpace(options.OutputDataJsonMonolithicFile))
                    {
                        errMsg = "--output_data_json_monolithic_file missing";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(options.InputTextTableFiles) ^ string.IsNullOrWhiteSpace(options.OutputNotConvertTextFile))
                    {
                        errMsg = "--input_l10n_text_files must be provided with --output_l10n_not_converted_text_file";
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(options.BranchName) ^ string.IsNullOrWhiteSpace(options.BranchInputDataDir))
                {
                    errMsg = "--branch must be provided with --branch_input_data_dir";
                    return false;
                }

                if (!options.ValidateTypescriptRequire(options.GenType, ref errMsg))
                {
                    return false;
                }

                return true;
            }
        }


        class GenContext
        {
            public GenArgs GenArgs { get; init; }
            public DefAssembly Assembly { get; init; }
            public string GenType { get; set; }
            public ICfgCodeRender Render { get; set; }
            public ELanguage Lan { get; set; }

            public string TopModule => Assembly.TopModule;
            public Service TargetService => Assembly.CfgTargetService;


            public List<DefTypeBase> ExportTypes { get; init; }
            public List<DefTable> ExportTables { get; init; }
            public ConcurrentBag<FileInfo> GenCodeFilesInOutputCodeDir { get; init; }
            public ConcurrentBag<FileInfo> GenDataFilesInOutputDataDir { get; init; }
            public ConcurrentBag<FileInfo> GenScatteredFiles { get; init; }
            public List<Task> Tasks { get; init; }
        }

        public async Task GenAsync(RemoteAgent agent, GenJob rpc)
        {
            var res = new GenJobRes()
            {
                ErrCode = Luban.Common.EErrorCode.OK,
                ErrMsg = "succ",
                FileGroups = new List<FileGroup>(),
            };

            if (!TryParseArg(rpc.Arg.JobArguments, out GenArgs args, out string errMsg))
            {
                res.ErrCode = Luban.Common.EErrorCode.JOB_ARGUMENT_ERROR;
                res.ErrMsg = errMsg;
                agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
                return;
            }

            var timer = new ProfileTimer();
            timer.StartPhase("= gen_all =");
            try
            {
                string inputDataDir = args.InputDataDir;
                string outputCodeDir = args.OutputCodeDir;
                string outputDataDir = args.OutputDataDir;

                var genTypes = args.GenType.Split(',').Select(s => s.Trim()).ToList();

                timer.StartPhase("build defines");
                var loader = new CfgDefLoader(agent);
                await loader.LoadAsync(args.DefineFile);
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                TimeZoneInfo timeZoneInfo = string.IsNullOrEmpty(args.TimeZone) ? null : TimeZoneInfo.FindSystemTimeZoneById(args.TimeZone);

                var ass = new DefAssembly(args.BranchName, timeZoneInfo, args.ExportTestData, agent);

                ass.Load(args.Service, rawDefines);

                var targetService = ass.CfgTargetService;


                List<DefTable> exportTables = ass.GetExportTables();
                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                bool hasLoadCfgData = false;

                bool needL10NTextConvert = !string.IsNullOrWhiteSpace(args.InputTextTableFiles);

                async Task CheckLoadCfgDataAsync()
                {
                    if (!hasLoadCfgData)
                    {
                        hasLoadCfgData = true;
                        var timer = new ProfileTimer();
                        timer.StartPhase("load config data");
                        await DataLoaderUtil.LoadCfgDataAsync(agent, ass, args.InputDataDir, args.BranchName, args.BranchInputDataDir, args.ExportTestData);
                        timer.EndPhaseAndLog();

                        if (needL10NTextConvert)
                        {
                            ass.InitL10n(args.TextValueFieldName);
                            await DataLoaderUtil.LoadTextTablesAsync(agent, ass, ".", args.InputTextTableFiles);
                        }

                        timer.StartPhase("validate");
                        var validateCtx = new ValidatorContext(ass, args.ValidateRootDir);
                        await validateCtx.ValidateTables(exportTables);
                        timer.EndPhaseAndLog();
                    }
                }

                var tasks = new List<Task>();

                var genCodeFilesInOutputCodeDir = new ConcurrentBag<FileInfo>();
                var genDataFilesInOutputDataDir = new ConcurrentBag<FileInfo>();
                var genScatteredFiles = new ConcurrentBag<FileInfo>();

                foreach (var genType in genTypes)
                {
                    var ctx = new GenContext()
                    {
                        GenType = genType,
                        Assembly = ass,
                        GenArgs = args,
                        ExportTypes = exportTypes,
                        ExportTables = exportTables,
                        GenCodeFilesInOutputCodeDir = genCodeFilesInOutputCodeDir,
                        GenDataFilesInOutputDataDir = genDataFilesInOutputDataDir,
                        GenScatteredFiles = genScatteredFiles,
                        Tasks = tasks,
                    };
                    switch (genType)
                    {
                        case "code_cs_bin":
                        case "code_cs_json":
                        case "code_java_bin":
                        case "code_go_bin":
                        case "code_go_json":
                        {
                            GenerateCodeScatter(ctx);
                            break;
                        }
                        case "code_lua_bin":
                        case "code_lua_lua":
                        {
                            GenLuaCode(ctx);
                            break;
                        }
                        case "code_typescript_bin":
                        case "code_typescript_json":
                        {
                            GenTypescriptCode(ctx);
                            break;
                        }
                        case "code_python27_json":
                        {
                            GenPythonCodes(ctx);
                            break;
                        }
                        case "code_cpp_bin":
                        {
                            GenCppCode(ctx);
                            break;
                        }
                        case "code_cpp_ue_editor":
                        {
                            GenCppUeEditor(ctx);
                            break;
                        }
                        case "code_cs_unity_editor":
                        {
                            GenCsUnityEditor(ctx);
                            break;
                        }
                        case "code_cpp_ue_bp":
                        {
                            GenCppUeBp(ctx);
                            break;
                        }
                        case "data_bin":
                        case "data_json":
                        case "data_lua":
                        {
                            await CheckLoadCfgDataAsync();
                            GenDataScatter(ctx);
                            break;
                        }
                        case "data_json_monolithic":
                        {
                            await CheckLoadCfgDataAsync();
                            tasks.Add(GenJsonDataMonolithic(ctx));
                            break;
                        }
                        case "data_resources":
                        {
                            await CheckLoadCfgDataAsync();
                            GenResourceList(ctx);
                            break;
                        }

                        default:
                        {
                            s_logger.Error("unknown gentype:{gentype}", genType);
                            break;
                        }
                    }
                }
                await Task.WhenAll(tasks.ToArray());

                if (needL10NTextConvert)
                {
                    var notConvertTextList = DataExporterUtil.GenNotConvertTextList(ass.NotConvertTextSet);
                    var md5 = FileUtil.CalcMD5(notConvertTextList);
                    string outputNotConvertTextFile = args.OutputNotConvertTextFile;
                    CacheManager.Ins.AddCache(outputNotConvertTextFile, md5, notConvertTextList);

                    genScatteredFiles.Add(new FileInfo() { FilePath = outputNotConvertTextFile, MD5 = md5 });
                }

                if (!genCodeFilesInOutputCodeDir.IsEmpty)
                {
                    res.FileGroups.Add(new FileGroup() { Dir = outputCodeDir, Files = genCodeFilesInOutputCodeDir.ToList() });
                }
                if (!genDataFilesInOutputDataDir.IsEmpty)
                {
                    res.FileGroups.Add(new FileGroup() { Dir = outputDataDir, Files = genDataFilesInOutputDataDir.ToList() });
                }
                if (!genScatteredFiles.IsEmpty)
                {
                    res.ScatteredFiles.AddRange(genScatteredFiles);
                }
            }
            catch (DataCreateException e)
            {
                res.ErrCode = Luban.Common.EErrorCode.DATA_PARSE_ERROR;
                res.ErrMsg = $@"
=======================================================================
    解析失败!

        文件:        {e.OriginDataLocation}
        错误位置:    {e.DataLocationInFile}
        Err:         {e.OriginErrorMsg}
        字段:        {e.VariableFullPathStr}

=======================================================================
";
                res.StackTrace = e.OriginStackTrace;
            }
            catch (Exception e)
            {
                res.ErrCode = Luban.Common.EErrorCode.JOB_EXCEPTION;
                res.ErrMsg = $"{e.Message} \n {e.StackTrace}";
            }
            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }

        private void GenerateCodeScatter(GenContext ctx)
        {
            string genType = ctx.GenType;
            ctx.Render = CreateCodeRender(genType);
            ctx.Lan = GetLanguage(genType);
            foreach (var c in ctx.ExportTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(ctx.Render.RenderAny(c), ctx.Lan);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ctx.Lan);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }

            ctx.Tasks.Add(Task.Run(() =>
            {
                var module = ctx.TopModule;
                var name = ctx.TargetService.Manager;
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(ctx.Render.RenderService(name, module, ctx.ExportTables), ctx.Lan);
                var file = RenderFileUtil.GetDefTypePath(name, ctx.Lan);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        private void GenerateCodeMonolithic(GenContext ctx, string outputFile, List<string> fileContent, Action<List<string>> preContent, Action<List<string>> postContent)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                fileContent.Add(FileHeaderUtil.GetAutoGenerationHeader(ctx.Lan));

                preContent?.Invoke(fileContent);

                foreach (var type in ctx.ExportTypes)
                {
                    fileContent.Add(ctx.Render.RenderAny(type));
                }

                fileContent.Add(ctx.Render.RenderService("Tables", ctx.TopModule, ctx.ExportTables));
                postContent?.Invoke(fileContent);

                var file = outputFile;
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', fileContent));
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        private void GenLuaCode(GenContext ctx)
        {
            string genType = ctx.GenType;
            LuaCodeRenderBase render = CreateCodeRender(genType) as LuaCodeRenderBase;
            var file = "Types.lua";
            var content = render.RenderAll(ctx.ExportTypes);
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
            ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }

        private void GenTypescriptCode(GenContext ctx)
        {
            string genType = ctx.GenType;
            var args = ctx.GenArgs;
            ctx.Render = CreateCodeRender(genType);
            ctx.Lan = GetLanguage(genType);

            var lines = new List<string>(10000);
            Action<List<string>> preContent = (fileContent) =>
            {
                var brightRequirePath = args.TypescriptBrightRequirePath;
                var brightPackageName = args.TypescriptBrightPackageName;
                bool isGenBinary = genType.EndsWith("bin");
                if (isGenBinary)
                {
                    if (args.UsePuertsByteBuf)
                    {
                        fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                    }
                    else
                    {
                        fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                    }
                }

                if (args.EmbedBrightTypes)
                {
                    fileContent.Add(isGenBinary ? TypescriptStringTemplate.VectorTypesByteBuf : TypescriptStringTemplate.VectorTypesJson);
                    if (isGenBinary)
                    {
                        fileContent.Add(TypescriptStringTemplate.SerializeTypes);
                    }
                }
                else
                {
                    if (isGenBinary)
                    {
                        fileContent.Add(TypescriptStringTemplate.GetSerializeImports(brightRequirePath, brightPackageName));
                    }
                    fileContent.Add(TypescriptStringTemplate.GetVectorImports(brightRequirePath, brightPackageName));
                }

                fileContent.Add(@$"export namespace {ctx.TopModule} {{");
            };

            Action<List<string>> postContent = (fileContent) =>
            {
                fileContent.Add("}"); // end of topmodule
            };

            GenerateCodeMonolithic(ctx, "Types.ts", lines, preContent, postContent);
        }

        private void GenPythonCodes(GenContext ctx)
        {
            string genType = ctx.GenType;
            ctx.Render = CreateCodeRender(genType);
            ctx.Lan = GetLanguage(genType);

            var isPython3 = genType.Contains("python3");

            var lines = new List<string>(10000);
            Action<List<string>> preContent = (fileContent) =>
            {
                if (isPython3)
                {
                    fileContent.Add(PythonStringTemplates.ImportTython3Enum);
                }
                fileContent.Add(PythonStringTemplates.PythonVectorTypes);
            };

            GenerateCodeMonolithic(ctx, "Types.py", lines, preContent, null);


            ctx.Tasks.Add(Task.Run(() =>
            {
                var moduleInitContent = "";
                var initFile = "__init__.py";

                var initMd5 = CacheFileUtil.GenMd5AndAddCache(initFile, moduleInitContent);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = initFile, MD5 = initMd5 });
            }));
        }

        private void GenCppCode(GenContext ctx)
        {
            var render = new CppCodeBinRender();
            // 将所有 头文件定义 生成到一个文件
            // 按照 const,enum,bean,table, service 的顺序生成

            ctx.Tasks.Add(Task.Run(() =>
            {
                var headerFileContent = new List<string>
                                {
                                    @$"
#pragma once
#include <functional>

#include ""bright/serialization/ByteBuf.h""
#include ""bright/CfgBean.hpp""

using ByteBuf = bright::serialization::ByteBuf;

namespace {ctx.TopModule}
{{

"
                                };

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefEnum e)
                    {
                        headerFileContent.Add(render.Render(e));
                    }
                }

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefConst c)
                    {
                        headerFileContent.Add(render.Render(c));
                    }
                }

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefBean e)
                    {
                        headerFileContent.Add(render.RenderForwardDefine(e));
                    }
                }

                foreach (var type in ctx.ExportTypes)
                {
                    if (type is DefBean e)
                    {
                        headerFileContent.Add(render.Render(e));
                    }
                }

                foreach (var type in ctx.ExportTables)
                {
                    headerFileContent.Add(render.Render(type));
                }

                headerFileContent.Add(render.RenderService("Tables", ctx.TopModule, ctx.ExportTables));

                headerFileContent.Add("}"); // end of topmodule

                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', headerFileContent), ELanguage.CPP);
                var file = "gen_types.h";
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));

            var beanTypes = ctx.ExportTypes.Where(c => c is DefBean).ToList();

            int TYPE_PER_STUB_FILE = 100;

            for (int i = 0, n = (beanTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
            {
                int index = i;
                ctx.Tasks.Add(Task.Run(() =>
                {
                    int startIndex = index * TYPE_PER_STUB_FILE;
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                        render.RenderStub(ctx.TopModule, beanTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, beanTypes.Count - startIndex))),
                        ELanguage.CPP);
                    var file = $"gen_stub_{index}.cpp";
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        private void GenCppUeEditor(GenContext ctx)
        {
            var render = new UE4EditorCppRender();

            var renderTypes = ctx.Assembly.Types.Values.Where(c => c is DefEnum || c is DefBean).ToList();

            foreach (var c in renderTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                    var file = "editor_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }

            int TYPE_PER_STUB_FILE = 200;

            for (int i = 0, n = (renderTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
            {
                int index = i;
                ctx.Tasks.Add(Task.Run(() =>
                {
                    int startIndex = index * TYPE_PER_STUB_FILE;
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                        render.RenderStub(renderTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, renderTypes.Count - startIndex))),
                        ELanguage.CPP);
                    var file = $"stub_{index}.cpp";
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        private void GenCsUnityEditor(GenContext ctx)
        {
            var render = new EditorCsRender();
            foreach (var c in ctx.Assembly.Types.Values)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CS);
                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ELanguage.CS);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        private void GenCppUeBp(GenContext ctx)
        {
            var render = new UE4BpCppRender();
            foreach (var c in ctx.ExportTypes)
            {
                if (!(c is DefEnum || c is DefBean))
                {
                    continue;
                }

                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                    var file = "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        private void GenDataScatter(GenContext ctx)
        {
            string genType = ctx.GenType;
            foreach (var c in ctx.ExportTables)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = DataExporterUtil.ToOutputData(c, ctx.Assembly.GetTableExportDataList(c), genType);
                    var file = GetOutputFileName(genType, c.OutputDataFile);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                    ctx.GenDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        private async Task GenJsonDataMonolithic(GenContext ctx)
        {
            var exportTables = ctx.ExportTables;
            List<Task<byte[]>> allJsonTask = new List<Task<byte[]>>();
            foreach (var c in exportTables)
            {
                allJsonTask.Add(Task.Run(() =>
                {
                    return DataExporterUtil.ToOutputData(c, ctx.Assembly.GetTableExportDataList(c), "data_json");
                }));
            }
            await Task.WhenAll(allJsonTask);

            int estimatedCapacity = allJsonTask.Sum(t => t.Result.Length + 100);
            var sb = new MemoryStream(estimatedCapacity);
            sb.Write(System.Text.Encoding.UTF8.GetBytes("{\n"));
            for (int i = 0; i < exportTables.Count; i++)
            {
                if (i != 0)
                {
                    sb.Write(System.Text.Encoding.UTF8.GetBytes((",\n")));
                }
                sb.Write(System.Text.Encoding.UTF8.GetBytes("\"" + exportTables[i].Name + "\":"));
                sb.Write(allJsonTask[i].Result);
            }
            sb.Write(System.Text.Encoding.UTF8.GetBytes("\n}"));

            var content = sb.ToArray();
            s_logger.Debug("estimated size:{0} actual size:{1}", estimatedCapacity, content.Length);
            var outputFile = ctx.GenArgs.OutputDataJsonMonolithicFile;
            var md5 = CacheFileUtil.GenMd5AndAddCache(outputFile, content);
            ctx.GenScatteredFiles.Add(new FileInfo() { FilePath = outputFile, MD5 = md5 });
        }

        private void GenResourceList(GenContext ctx)
        {
            var genDataTasks = new List<Task<List<ResourceInfo>>>();
            foreach (var c in ctx.ExportTables)
            {
                genDataTasks.Add(Task.Run(() =>
                {
                    return DataExporterUtil.ExportResourceList(ctx.Assembly.GetTableExportDataList(c));
                }));
            }

            ctx.Tasks.Add(Task.Run(async () =>
            {
                var ress = new HashSet<(string, string)>(10000);
                var resourceLines = new List<string>(10000);
                foreach (var task in genDataTasks)
                {
                    foreach (var ri in await task)
                    {
                        if (ress.Add((ri.Resource, ri.Tag)))
                        {
                            resourceLines.Add($"{ri.Resource},{ri.Tag}");
                        }
                    }
                }
                var file = ctx.GenArgs.OutputDataResourceListFile;
                var content = string.Join("\n", resourceLines);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);

                ctx.GenScatteredFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }
}
