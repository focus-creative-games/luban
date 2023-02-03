using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Db.Defs;
using Luban.Job.Db.Generate;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Db
{

    public class JobController : IJobController
    {
        private bool TryParseArg(List<string> args, out GenArgs options, out string errMsg)
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

            options = (parseResult as Parsed<GenArgs>).Value;
            errMsg = null;
            if (!options.ValidateOutouptCodeDir(ref errMsg))
            {
                return false;
            }
            if (options.GenType.Contains("typescript") && !options.ValidateTypescriptRequire(options.GenType, ref errMsg))
            {
                return false;
            }
            if (!options.ValidateConvention(ref errMsg))
            {
                return false;
            }
            return true;
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
                string outputCodeDir = args.OutputCodeDir;


                timer.StartPhase("build defines");
                var loader = new DbDefLoader(agent);
                await loader.LoadAsync(args.DefineFile);
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                var ass = new DefAssembly();
                ass.Load(rawDefines, agent, args);


                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                var tasks = new List<Task>();
                var genCodeFiles = new ConcurrentBag<FileInfo>();
                var genScatteredFiles = new ConcurrentBag<FileInfo>();

                var genType = args.GenType;
                switch (genType)
                {
                    case "cs":
                    {
                        ass.CurrentLanguage = ELanguage.CS;
                        var render = new SyncCsRender();
                        foreach (var c in ass.Types.Values)
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), Common.ELanguage.CS);
                                var file = RenderFileUtil.GetDefTypePath(c.FullName, Common.ELanguage.CS);
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                        }
                        tasks.Add(Task.Run(() =>
                        {
                            var module = ass.TopModule;
                            var name = "Tables";
                            var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                                render.RenderTables(name, module,
                                ass.Types.Values.Where(t => t is DefTable).Select(t => (DefTable)t).ToList()),
                                Common.ELanguage.CS);
                            var file = RenderFileUtil.GetDefTypePath(name, Common.ELanguage.CS);
                            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                            genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                        }));
                        break;
                    }
                    case "typescript":
                    {
                        ass.CurrentLanguage = ELanguage.TYPESCRIPT;
                        var render = new TypescriptRender();
                        var brightRequirePath = args.TypescriptBrightRequirePath;
                        var brightPackageName = args.TypescriptBrightPackageName;
                        tasks.Add(Task.Run(() =>
                        {
                            var fileContent = new List<string>();

                            fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));

                            fileContent.Add(TypescriptStringTemplate.GetSerializeImports(brightRequirePath, brightPackageName));
                            fileContent.Add(TypescriptStringTemplate.GetProtocolImports(brightRequirePath, brightPackageName));
                            fileContent.Add(TypescriptStringTemplate.GetVectorImports(brightRequirePath, brightPackageName));

                            if (!string.IsNullOrEmpty(brightRequirePath))
                            {
                                fileContent.Add($"import {{FieldLogger, FieldLoggerGeneric1, FieldLoggerGeneric2}} from '{brightRequirePath}/transaction/FieldLogger'");
                                fileContent.Add($"import TxnBeanBase from '{brightRequirePath}/transaction/TxnBeanBase'");
                                fileContent.Add($"import {{TxnTable, TxnTableGeneric}} from '{brightRequirePath}/transaction/TxnTable'");
                                fileContent.Add($"import TransactionContext from '{brightRequirePath}/transaction/TransactionContext'");
                                fileContent.Add($"import {{FieldTag}} from '{brightRequirePath}/serialization/FieldTag'");
                                fileContent.Add($"import TKey from '{brightRequirePath}/storage/TKey'");
                                fileContent.Add($"import PList from '{brightRequirePath}/transaction/collections/PList'");
                                fileContent.Add($"import PList1 from '{brightRequirePath}/transaction/collections/PList1'");
                                fileContent.Add($"import PList2 from '{brightRequirePath}/transaction/collections/PList2'");
                                fileContent.Add($"import PSet from '{brightRequirePath}/transaction/collections/PSet'");
                                fileContent.Add($"import PMap from '{brightRequirePath}/transaction/collections/PMap'");
                                fileContent.Add($"import PMap1 from '{brightRequirePath}/transaction/collections/PMap1'");
                                fileContent.Add($"import PMap2 from '{brightRequirePath}/transaction/collections/PMap2'");
                                fileContent.Add($"import SerializeFactory from '{brightRequirePath}/serialization/SerializeFactory'");
                            }
                            else
                            {
                                fileContent.Add($"import {{FieldLogger, FieldLoggerGeneric1, FieldLoggerGeneric2}} from '{brightPackageName}'");
                                fileContent.Add($"import {{TxnBeanBase}} from '{brightPackageName}'");
                                fileContent.Add($"import {{TxnTable, TxnTableGeneric}} from '{brightPackageName}'");
                                fileContent.Add($"import {{TransactionContext}} from '{brightPackageName}'");
                                fileContent.Add($"import {{FieldTag}} from '{brightPackageName}'");
                                fileContent.Add($"import {{TKey}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PList}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PList1}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PList2}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PSet}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PMap}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PMap1}} from '{brightPackageName}'");
                                fileContent.Add($"import {{PMap2}} from '{brightPackageName}'");
                                fileContent.Add($"import {{SerializeFactory}} from '{brightPackageName}'");
                            }

                            fileContent.Add($"export namespace {ass.TopModule} {{");


                            foreach (var type in exportTypes)
                            {
                                fileContent.Add(render.RenderAny(type));
                            }

                            var tables = ass.Types.Values.Where(t => t is DefTable).Select(t => (DefTable)t).ToList();
                            fileContent.Add(render.RenderTables("Tables", ass.TopModule, tables));

                            fileContent.Add("}"); // end of topmodule

                            var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', fileContent), ELanguage.TYPESCRIPT);
                            var file = "Types.ts";
                            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                            genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                        }));
                        break;
                    }
                    default:
                    {
                        throw new NotSupportedException($"not support gen type:{genType}");
                    }

                }

                await Task.WhenAll(tasks.ToArray());

                res.FileGroups.Add(new FileGroup() { Dir = outputCodeDir, Files = genCodeFiles.ToList() });
            }
            catch (Exception e)
            {
                res.ErrCode = Luban.Common.EErrorCode.JOB_EXCEPTION;
                res.ErrMsg = $"{e.Message} \n {e.StackTrace}";
            }
            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }
    }
}
