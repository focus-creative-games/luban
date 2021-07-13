using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Luban.Job.Proto.Generate;
using Luban.Job.Proto.RawDefs;
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

        class GenArgs
        {
            [Option('d', "define_file", Required = true, HelpText = "define file")]
            public string DefineFile { get; set; }

            [Option('c', "output_code_dir", Required = true, HelpText = "output code directory")]
            public string OutputCodeDir { get; set; }

            [Option('g', "gen_type", Required = true, HelpText = "cs,lua,java,cpp,ts")]
            public string GenType { get; set; }

            [Option('s', "service", Required = true, HelpText = "service")]
            public string Service { get; set; }

            [Option("typescript_bright_require_path", Required = false, HelpText = "bright require path in typescript")]
            public string TypescriptBrightRequirePath { get; set; }

            [Option("use_puerts_bytebuf", Required = false, HelpText = "use puerts bytebuf class. default is false")]
            public bool UsePuertsByteBuf { get; set; }

            [Option("embed_bright_types", Required = false, HelpText = "use puerts bytebuf class. default is false")]
            public bool EmbedBrightTypes { get; set; }
        }


        private bool TryParseArg(List<string> args, out GenArgs result, out string errMsg)
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
                result = null;
                return false;
            }

            result = (parseResult as Parsed<GenArgs>).Value;
            errMsg = null;


            if (!result.UsePuertsByteBuf && string.IsNullOrWhiteSpace(result.TypescriptBrightRequirePath))
            {
                errMsg = $"while use_puerts_bytebuf is false, should provide option --typescript_bright_require_path";
                return false;
            }
            if (!result.EmbedBrightTypes && string.IsNullOrWhiteSpace(result.TypescriptBrightRequirePath))
            {
                errMsg = $"while embed_bright_types is false, should provide option --typescript_bright_require_path";
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

                var ass = new DefAssembly();

                ass.Load(rawDefines, agent);

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

                        tasks.Add(Task.Run(() =>
                        {
                            var fileContent = new List<string>();
                            if (args.UsePuertsByteBuf)
                            {
                                fileContent.Add(TypescriptBrightTypeTemplates.PuertsByteBufImports);
                            }
                            else
                            {
                                fileContent.Add(string.Format(TypescriptBrightTypeTemplates.BrightByteBufImportsFormat, brightRequirePath));
                            }
                            if (args.EmbedBrightTypes)
                            {
                                fileContent.Add(TypescriptBrightTypeTemplates.VectorTypes);
                                fileContent.Add(TypescriptBrightTypeTemplates.SerializeTypes);
                                fileContent.Add(TypescriptBrightTypeTemplates.ProtoTypes);
                            }
                            else
                            {
                                fileContent.Add(string.Format(TypescriptBrightTypeTemplates.SerializeImportsFormat, brightRequirePath));
                                fileContent.Add(string.Format(TypescriptBrightTypeTemplates.ProtocolImportsFormat, brightRequirePath));
                                fileContent.Add(string.Format(TypescriptBrightTypeTemplates.VectorImportsFormat, brightRequirePath));
                            }

                            fileContent.Add(@$"
export namespace {ass.TopModule} {{
");

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
                res.ErrMsg = $"{e.Message} \n {e.StackTrace}";
            }
            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }
    }
}
