using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Luban.Job.Proto.Generate;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Proto
{
    [Controller("proto")]
    public class JobController : IJobController
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        class GenArgs : GenArgsBase
        {
            [Option('g', "gen_type", Required = true, HelpText = "cs,lua,java,cpp,ts")]
            public string GenType { get; set; }

            [Option('s', "service", Required = true, HelpText = "service")]
            public string Service { get; set; }
        }


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
            if (!options.ValidateTypescriptRequire(options.GenType, ref errMsg))
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
                var loader = new ProtoDefLoader(agent);
                await loader.LoadAsync(args.DefineFile);
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                var ass = new DefAssembly() { UseUnityVectors = args.UseUnityVectors };

                ass.Load(rawDefines, agent);

                DefAssemblyBase.LocalAssebmly = ass;

                var targetService = args.Service;


                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                var tasks = new List<Task>();
                var genCodeFiles = new ConcurrentBag<FileInfo>();


                var genType = args.GenType;
                switch (genType)
                {
                    case "cs":
                    {
                        var render = new CsRender();
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
                            var name = "ProtocolStub";
                            var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                                render.RenderStubs(name, module,
                                ass.Types.Values.Where(t => t is DefProto).ToList(),
                                ass.Types.Values.Where(t => t is DefRpc).ToList()),
                                Common.ELanguage.CS);
                            var file = RenderFileUtil.GetDefTypePath(name, Common.ELanguage.CS);
                            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                            genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                        }));
                        break;
                    }
                    case "lua":
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            var render = new LuaRender();
                            var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderTypes(ass.Types.Values.ToList()), Common.ELanguage.LUA);
                            var file = "Types.lua";
                            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                            genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                        }));
                        break;
                    }
                    case "ts":
                    {
                        var render = new TypescriptRender();
                        var brightRequirePath = args.TypescriptBrightRequirePath;
                        var brightPackageName = args.TypescriptBrightPackageName;

                        tasks.Add(Task.Run(() =>
                        {
                            var fileContent = new List<string>();
                            if (args.UsePuertsByteBuf)
                            {
                                fileContent.Add(TypescriptStringTemplate.PuertsByteBufImports);
                            }
                            else
                            {
                                fileContent.Add(TypescriptStringTemplate.GetByteBufImports(brightRequirePath, brightPackageName));
                            }
                            if (args.EmbedBrightTypes)
                            {
                                fileContent.Add(StringTemplateUtil.GetTemplateString("config/typescript_bin/vectors"));
                                fileContent.Add(TypescriptStringTemplate.SerializeTypes);
                                fileContent.Add(TypescriptStringTemplate.ProtoTypes);
                            }
                            else
                            {
                                fileContent.Add(TypescriptStringTemplate.GetSerializeImports(brightRequirePath, brightPackageName));
                                fileContent.Add(TypescriptStringTemplate.GetProtocolImports(brightRequirePath, brightPackageName));
                                fileContent.Add(TypescriptStringTemplate.GetVectorImports(brightRequirePath, brightPackageName));
                            }

                            fileContent.Add(@$"export namespace {ass.TopModule} {{");

                            foreach (var type in exportTypes)
                            {
                                fileContent.Add(render.RenderAny(type));
                            }

                            fileContent.Add(render.RenderStubs("ProtocolStub", ass.TopModule, ass.Types.Values.Where(t => t is DefProto).ToList(),
                                ass.Types.Values.Where(t => t is DefRpc).ToList()));

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
                res.ErrMsg = ExceptionUtil.ExtractMessage(e);
                res.StackTrace = e.StackTrace;
            }

            DefAssemblyBase.LocalAssebmly = null;

            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }
    }
}
