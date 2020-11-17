using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
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
using System.Text;
using System.Threading.Tasks;
using FileInfo = Luban.Common.Protos.FileInfo;

namespace Luban.Job.Db
{
    public class JobController : IJobController
    {
        class GenArgs
        {
            [Option('d', "define_file", Required = true, HelpText = "define file")]
            public string DefineFile { get; set; }

            [Option('c', "output_code_dir", Required = true, HelpText = "output code directory")]
            public string OutputCodeDir { get; set; }

            [Option('g', "gen_type", Required = true, HelpText = "cs . current only support cs")]
            public string GenType { get; set; }
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

                ass.Load(rawDefines, agent);


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
