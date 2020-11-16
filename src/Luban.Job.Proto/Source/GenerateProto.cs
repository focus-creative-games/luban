//using Bright.Time;
//using Gen.Proto.Common.RawDefs;
//using Luban.Job.Common.Utils;

//namespace Luban.Job.Proto.Client
//{
//    class GenerateProto
//    {
//        public bool Handle(CommandLineOptions options)
//        {
//            var timer = new ProfileTimer();
//            timer.StartPhase("所有阶段");

//            timer.StartPhase("加载定义");
//            var protoLoader = new ProtoDefLoader();
//            protoLoader.Load(options.DefineFile);
//            long defEndTime = TimeUtil.NowMillis;
//            timer.EndPhaseAndLog();


//            timer.StartPhase("发起 GenProto");
//            string outputDir = options.OutputCodeDir;

//            var defines = protoLoader.BuildDefines();


//            defines.Target = options.Target;
//            defines.Language = options.Languange;
//            var rpc = new GenProto();
//            var res = rpc.Call(GenClient.Ins.Session, new GenProtoArg()
//            {
//                Define = defines,
//                OutputCodeRelatePath = outputDir,
//            }).Result;
//            timer.EndPhaseAndLog();


//            if (res.OK)
//            {
//                timer.StartPhase("获取生成文件");

//                DownloadFileUtil.DownloadGeneratedFiles(outputDir, res.NewCodeFiles).Wait();

//                timer.EndPhaseAndLog();

//                timer.EndPhaseAndLog();
//                return true;
//            }
//            else
//            {
//                timer.EndPhaseAndLog();
//                return false;
//            }
//        }
//    }
//}
