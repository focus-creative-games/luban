using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Generate;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Cfg
{
    [Controller("cfg")]
    public class JobController : IJobController
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

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
                        errMsg = "--output:data:resource_list_file missing";
                        return false;
                    }
                    if (genTypes.Contains("data_json_monolithic") && string.IsNullOrWhiteSpace(options.OutputDataJsonMonolithicFile))
                    {
                        errMsg = "--output:data:json_monolithic_file missing";
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(options.L10nInputTextTableFiles) ^ string.IsNullOrWhiteSpace(options.L10nOutputNotTranslatedTextFile))
                    {
                        errMsg = "--l10n:input_text_files should be used with --l10n:output_not_translated_text_file";
                        return false;
                    }
                    if (genTypes.Contains("data_template") ^ !string.IsNullOrWhiteSpace(options.TemplateDataFile))
                    {
                        errMsg = "gen_types data_template should be used with --template:data:file";
                        return false;
                    }
                    if (genTypes.Contains("convert_template") ^ !string.IsNullOrWhiteSpace(options.TemplateConvertFile))
                    {
                        errMsg = "gen_types convert_template should be used with --template:convert:file";
                        return false;
                    }
                    if (genTypes.Contains("code_template") ^ !string.IsNullOrWhiteSpace(options.TemplateCodeDir))
                    {
                        errMsg = "gen_types code_template should be used with --template:code:dir";
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(options.L10nPatchName) ^ string.IsNullOrWhiteSpace(options.L10nPatchInputDataDir))
                {
                    errMsg = "--patch must be provided with --patch_input_data_dir";
                    return false;
                }

                if (options.GenType.Contains("typescript_bin") && !options.ValidateTypescriptRequire(options.GenType, ref errMsg))
                {
                    return false;
                }
                if (options.GenType.Contains("go_") && !options.ValidateGoRequire(options.GenType, ref errMsg))
                {
                    return false;
                }
                if (!options.ValidateConvention(ref errMsg))
                {
                    return false;
                }

                if (options.GenType.Contains("unity"))
                {
                    options.CsUseUnityVectors = true;
                }

                return true;
            }
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
                await loader.LoadDefinesFromFileAsync(inputDataDir);
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                TimeZoneInfo timeZoneInfo = string.IsNullOrEmpty(args.L10nTimeZone) ? null : TimeZoneInfo.FindSystemTimeZoneById(args.L10nTimeZone);

                var excludeTags = args.OutputExcludeTags.Split(',').Select(t => t.Trim().ToLowerInvariant()).Where(t => !string.IsNullOrEmpty(t)).ToList();
                var ass = new DefAssembly(args.L10nPatchName, timeZoneInfo, excludeTags, agent);
                ass.Load(rawDefines, agent, args);

                List<DefTable> exportTables = ass.GetExportTables();
                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                bool hasLoadCfgData = false;

                bool needL10NTextConvert = !string.IsNullOrWhiteSpace(args.L10nInputTextTableFiles);

                async Task CheckLoadCfgDataAsync()
                {
                    if (!hasLoadCfgData)
                    {
                        hasLoadCfgData = true;
                        var timer = new ProfileTimer();
                        timer.StartPhase("load config data");
                        await DataLoaderUtil.LoadCfgDataAsync(agent, ass, args.InputDataDir, args.L10nPatchName, args.L10nPatchInputDataDir, args.InputConvertDataDir);
                        timer.EndPhaseAndLog();

                        if (needL10NTextConvert)
                        {
                            ass.InitL10n(args.L10nTextValueFieldName);
                            await DataLoaderUtil.LoadTextTablesAsync(agent, ass, ".", args.L10nInputTextTableFiles);
                        }

                        timer.StartPhase("validate");
                        var validateCtx = new ValidatorContext(ass, args.ValidateRootDir);
                        await validateCtx.ValidateTables(ass.GetAllTables());
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
                        DataLoader = CheckLoadCfgDataAsync,
                    };
                    GenContext.Ctx = ctx;

                    var render = RenderFactory.CreateRender(genType);
                    if (render == null)
                    {
                        throw new Exception($"unknown gentype:{genType}");
                    }
                    if (render is DataRenderBase)
                    {
                        await CheckLoadCfgDataAsync();
                    }
                    render.Render(ctx);
                    GenContext.Ctx = null;
                }
                await Task.WhenAll(tasks.ToArray());

                if (needL10NTextConvert)
                {
                    var notConvertTextList = DataExporterUtil.GenNotConvertTextList(ass.NotConvertTextSet);
                    var md5 = FileUtil.CalcMD5(notConvertTextList);
                    string outputNotConvertTextFile = args.L10nOutputNotTranslatedTextFile;
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
                res.ErrMsg = $@"
=======================================================================

{ExceptionUtil.ExtractMessage(e)}

=======================================================================
";
                res.StackTrace = e.StackTrace;
            }
            DefAssemblyBase.LocalAssebmly = null;
            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }
    }
}
