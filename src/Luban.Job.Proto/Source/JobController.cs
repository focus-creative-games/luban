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
            if (options.GenType.Contains("go_") && !options.ValidateGoRequire(options.GenType, ref errMsg))
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
                var loader = new ProtoDefLoader(agent);
                await loader.LoadAsync(args.DefineFile);
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                var ass = new DefAssembly();

                ass.Load(rawDefines, agent, args);

                var targetService = args.Service;


                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                var tasks = new List<Task>();
                var genCodeFiles = new ConcurrentBag<FileInfo>();
                var genScatteredFiles = new ConcurrentBag<FileInfo>();


                var genType = args.GenType;
                var render = RenderFactory.CreateRender(genType);
                if (render == null)
                {
                    throw new NotSupportedException($"not support gen type:{genType}");
                }
                ass.CurrentLanguage = RenderFileUtil.GetLanguage(genType);
                render.Render(new GenContext()
                {
                    GenArgs = args,
                    Assembly = ass,
                    Lan = ass.CurrentLanguage,
                    GenType = genType,
                    Render = render,
                    Tasks = tasks,
                    ExportTypes = exportTypes,
                    GenCodeFilesInOutputCodeDir = genCodeFiles,
                    GenScatteredFiles = genScatteredFiles,
                });


                await Task.WhenAll(tasks.ToArray());

                res.FileGroups.Add(new FileGroup() { Dir = outputCodeDir, Files = genCodeFiles.ToList() });
                res.ScatteredFiles.AddRange(genScatteredFiles);
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
