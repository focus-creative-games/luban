namespace Luban.Job.Proto.Server.Net
{
    class RpcController
    {

        //private void LogException(IContext ctx, Exception e)
        //{
        //    if (e is AggregateException ae)
        //    {
        //        foreach (var ie in ae.InnerExceptions)
        //        {
        //            LogException(ctx, ie);
        //        }
        //    }
        //    else
        //    {

        //        s_logger.Error(e, "发生异常");
        //        ctx.Error(e, " {0} \n ", e.StackTrace);
        //    }
        //}

        //private bool ValidateDataType(string outputDataType)
        //{
        //    switch (outputDataType)
        //    {
        //        case "bin":
        //        case "cbin":
        //        case "json": return true;
        //        default: return false;
        //    }
        //}

        //private void OnGenDb(Session session, GenDb proto)
        //{
        //    var timer = new ProfileTimer();

        //    timer.StartPhase("生成代码");

        //    var res = new GenDbRes();

        //    string relatePath = proto.Arg.OutputCodeRelatePath;

        //    var ass = new DefAssembly();
        //    var ctx = new SessionContext(session, proto.Arg.Verbose);
        //    try
        //    {
        //        var genCodeTasks = new List<Task>();
        //        var outputFiles = new ConcurrentBag<FileInfo>();

        //        ass.Load(EJobType.DB, proto.Arg.Define, ctx);
        //        var render = new Generate.Db.CsRender();
        //        foreach (var c in ass.Types.Values)
        //        {
        //            genCodeTasks.Add(Task.Run(() =>
        //            {
        //                var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cs");
        //                var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                var md5 = GenMd5AndAddCache(file, content);
        //                outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //            }));
        //        }
        //        genCodeTasks.Add(Task.Run(() =>
        //        {
        //            var module = ass.TopModule;
        //            var name = "Tables";
        //            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderTables(name, module, ass.Types.Values.Where(t => t is BTable).ToList()), "cs");
        //            var file = FileUtil.GetCsDefTypePath(name);
        //            var md5 = GenMd5AndAddCache(file, content);
        //            outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //        }));
        //        Task.WhenAll(genCodeTasks).Wait();
        //        ctx.Info(timer.EndPhaseAndLogToString());

        //        res.NewCodeFiles = outputFiles.ToList();
        //        res.OK = true;
        //    }
        //    catch (Exception e)
        //    {
        //        res.OK = false;
        //        LogException(ctx, e);
        //    }

        //    proto.ReturnResult(session, res);
        //}

        //private ICsCodeRender CreateCsCodeRender(string outputDataType)
        //{
        //    switch (outputDataType)
        //    {
        //        case "bin": return new AppCsBinCodeRender();
        //        case "cbin": return new AppCsCompatibleBinCodeRender();
        //        case "json": return new AppCsJsonCodeRender();
        //        default: throw new ArgumentException($"not support output data type:{outputDataType}");
        //    }
        //}

        //private void OnGenCfg(Session session, GenCfg proto)
        //{
        //    var res = new GenCfgRes();

        //    string lan = proto.Arg.Define.Language;

        //    DefAssembly ass = new DefAssembly();

        //    var ctx = new SessionContext(session, proto.Arg.Verbose);


        //    var allJobs = new List<Task>();


        //    try
        //    {
        //        string outputDataType = proto.Arg.OutputDataType;
        //        if (!ValidateDataType(outputDataType))
        //        {
        //            throw new ArgumentException($"unknown outputdatatype:{outputDataType}");
        //        }

        //        bool exportTestData = proto.Arg.ExportTestData;

        //        long genStartTime = TimeUtil.NowMillis;
        //        var genCodeTasks = new List<Task>();
        //        var outputCodeFiles = new ConcurrentBag<FileInfo>();

        //        ass.Load(EJobType.CONFIG, proto.Arg.Define, ctx);
        //        EGenTypes genTypes = proto.Arg.GenTypes;
        //        var targetService = ass.CfgTargetService;

        //        List<CTable> exportTables = ass.Types.Values.Where(t => t is CTable ct && ct.NeedExport).Select(t => (CTable)t).ToList();

        //        if (genTypes.HasFlag(EGenTypes.APP_CODE))
        //        {
        //            var refTypes = new Dictionary<string, IDefType>();
        //            long genCodeStartTime = TimeUtil.NowMillis;

        //            foreach (var refType in targetService.Refs)
        //            {
        //                if (!ass.Types.ContainsKey(refType))
        //                {
        //                    throw new Exception($"service:{targetService.Name} ref:{refType} 类型不存在");
        //                }
        //                if (!refTypes.TryAdd(refType, ass.Types[refType]))
        //                {
        //                    throw new Exception($"service:{targetService.Name} ref:{refType} 重复引用");
        //                }
        //            }

        //            foreach (var table in exportTables)
        //            {
        //                refTypes[table.FullName] = table;
        //                table.ValueTType.Apply(RefTypeVisitor.Ins, refTypes);
        //            }

        //            foreach (var type in ass.Types)
        //            {
        //                if (type.Value is DefConst || type.Value is DefEnum)
        //                {
        //                    refTypes[type.Key] = type.Value;
        //                }
        //            }

        //            List<IDefType> exportTypes = refTypes.Values.ToList();

        //            switch (lan)
        //            {
        //                case "cs":
        //                {
        //                    ICsCodeRender render = CreateCsCodeRender(outputDataType);
        //                    foreach (var c in exportTypes)
        //                    {
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cs");
        //                            var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }


        //                    genCodeTasks.Add(Task.Run(() =>
        //                    {
        //                        var module = ass.TopModule;
        //                        var name = targetService.Manager;
        //                        var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), "cs");
        //                        var file = FileUtil.GetCsDefTypePath(name);
        //                        var md5 = GenMd5AndAddCache(file, content);
        //                        outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    }));

        //                    break;
        //                }
        //                case "go":
        //                {
        //                    //
        //                    // TODO?
        //                    // 由于 go 语言不支持类型继承
        //                    // go 不支持 table.value_type 为多态的表
        //                    //
        //                    //
        //                    var render = new AppGoCodeRender();


        //                    foreach (var c in exportTypes)
        //                    {
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "go");
        //                            var file = FileUtil.GetGoDefTypePath(c.FullName);
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }

        //                    genCodeTasks.Add(Task.Run(() =>
        //                    {
        //                        var module = ass.TopModule;
        //                        var name = targetService.Manager;
        //                        var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), "go");
        //                        var file = FileUtil.GetGoDefTypePath(name);
        //                        var md5 = GenMd5AndAddCache(file, content);
        //                        outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    }));

        //                    break;
        //                }
        //                case "lua":
        //                {
        //                    genCodeTasks.Add(Task.Run(() =>
        //                    {
        //                        var render = new Generate.Cfg.LuaRender();
        //                        var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAll(exportTypes.ToList()), "lua");
        //                        var file = "CfgTypes.lua";
        //                        var md5 = GenMd5AndAddCache(file, content);
        //                        outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    }));
        //                    break;
        //                }
        //                case "editor_cs":
        //                {
        //                    var render = new EditorCsRender();
        //                    foreach (var c in ass.Types.Values)
        //                    {
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cpp");
        //                            var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }
        //                    break;
        //                }
        //                case "editor_cpp":
        //                {
        //                    var render = new EditorCppRender();
        //                    foreach (var c in ass.Types.Values)
        //                    {
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cpp");
        //                            var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }
        //                    break;
        //                }
        //                case "editor_ue_cpp":
        //                {
        //                    var render = new UE4EditorCppRender();

        //                    var renderTypes = ass.Types.Values.Where(c => c is DefEnum || c is DefBean).ToList();

        //                    foreach (var c in renderTypes)
        //                    {
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cpp");
        //                            var file = c.UeEditorHeaderFileName;
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }

        //                    int TYPE_PER_STUB_FILE = 200;

        //                    for (int i = 0, n = (renderTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
        //                    {
        //                        int index = i;
        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            int startIndex = index * TYPE_PER_STUB_FILE;
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(
        //                                render.RenderStub(renderTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, renderTypes.Count - startIndex))),
        //                                "cpp");
        //                            var file = $"Stub_{index}.cpp";
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }
        //                    break;
        //                }
        //                case "ue_bp_cpp":
        //                {
        //                    var render = new UE4BpCppRender();
        //                    foreach (var c in exportTypes)
        //                    {
        //                        if (c is DefConst || c is CTable)
        //                        {
        //                            continue;
        //                        }

        //                        genCodeTasks.Add(Task.Run(() =>
        //                        {
        //                            var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cpp");
        //                            var file = c.UeBpHeaderFileName;
        //                            var md5 = GenMd5AndAddCache(file, content);
        //                            outputCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                        }));
        //                    }

        //                    //{
        //                    //    var module = ass.TopModule;
        //                    //    var name = targetService.Manager;
        //                    //    var content = render.RenderService(name, module, exportTables);
        //                    //    var file = FileUtil.GetCppDefTypeCppFilePath(name);
        //                    //    var md5 = GenMd5AndAddCache(file, content);
        //                    //    outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    //}
        //                    break;
        //                }
        //                default:
        //                {
        //                    throw new Exception($"unknown language:{lan}");
        //                }
        //            }


        //            allJobs.Add(Task.Run(async () =>
        //            {
        //                await Task.WhenAll(genCodeTasks);
        //                res.NewAppCodeFiles = outputCodeFiles.ToList();
        //                long genCodeEndTime = TimeUtil.NowMillis;
        //                ctx.Info("====== 生成代码 总共耗时 {0} ms ======", (genCodeEndTime - genCodeStartTime));
        //            }));
        //        }

        //        if ((genTypes & (EGenTypes.APP_DATA | EGenTypes.APP_RESOURCE_LIST)) != 0)
        //        {
        //            var genDataTasks = new List<Task>();
        //            var outputDataFiles = new ConcurrentBag<FileInfo>();
        //            var render = new AppBinaryDataRender();
        //            long genDataStartTime = TimeUtil.NowMillis;

        //            foreach (CTable c in exportTables)
        //            {
        //                genDataTasks.Add(Task.Run(async () =>
        //                {
        //                    long beginTime = TimeUtil.NowMillis;
        //                    await c.Load(session, exportTestData);
        //                    long endTime = TimeUtil.NowMillis;
        //                    if (endTime - beginTime > 100)
        //                    {
        //                        ctx.Info("====== 配置表 {0} 加载耗时 {1} ms ======", c.FullName, (endTime - beginTime));
        //                    }
        //                }));
        //            }
        //            Task.WaitAll(genDataTasks.ToArray());
        //        }


        //        if (genTypes.HasFlag(EGenTypes.APP_DATA))
        //        {
        //            var genDataTasks = new List<Task>();
        //            var outputDataFiles = new ConcurrentBag<FileInfo>();
        //            var render = new AppBinaryDataRender();
        //            long genDataStartTime = TimeUtil.NowMillis;

        //            foreach (CTable c in exportTables)
        //            {
        //                genDataTasks.Add(Task.Run(() =>
        //                {
        //                    var content = c.ToOutputData(outputDataType);
        //                    var file = c.OutputDataFile;
        //                    var md5 = GenMd5AndAddCache(file, content);
        //                    outputDataFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                }));
        //            }

        //            allJobs.Add(Task.Run(async () =>
        //            {
        //                await Task.WhenAll(genDataTasks);
        //                long genDataEndTime = TimeUtil.NowMillis;
        //                ctx.Info("====== 生成配置数据 总共耗时 {0} ms ======", (genDataEndTime - genDataStartTime));
        //                res.NewAppDataFiles = outputDataFiles.ToList();

        //                long verifyStartTime = TimeUtil.NowMillis;
        //                render.VerifyTables(exportTables);
        //                res.PathQueries = ass.GetPathQueries();

        //                long verifyEndTime = TimeUtil.NowMillis;
        //                ctx.Info("====== 校验配置 总共耗时 {0} ms ======", (verifyEndTime - verifyStartTime));
        //            }));
        //        }

        //        if (genTypes.HasFlag(EGenTypes.APP_RESOURCE_LIST))
        //        {
        //            var genDataTasks = new List<Task<List<CfgResourceInfo>>>();
        //            var render = new AppBinaryDataRender();
        //            long genDataStartTime = TimeUtil.NowMillis;

        //            foreach (CTable c in exportTables)
        //            {
        //                genDataTasks.Add(Task.Run(() =>
        //                {
        //                    return c.ExportResourceList();
        //                }));
        //            }

        //            allJobs.Add(Task.Run(async () =>
        //            {
        //                var ress = new HashSet<(string, string)>(10000);
        //                foreach (var task in genDataTasks)
        //                {
        //                    foreach (var ri in await task)
        //                    {
        //                        if (ress.Add((ri.Resource, ri.Tag)))
        //                        {
        //                            res.ResourceList.Add(ri);
        //                        }
        //                    }
        //                }
        //                long genDataEndTime = TimeUtil.NowMillis;
        //                ctx.Info("====== 生成导出资源列表 总共耗时 {0} ms ======", (genDataEndTime - genDataStartTime));
        //            }));
        //        }

        //        Task.WhenAll(allJobs).Wait();

        //        long genEndTime = TimeUtil.NowMillis;
        //        ctx.Info("========  服务器端 总共耗时 {0} ms =======", (genEndTime - genStartTime));

        //        res.OK = true;
        //    }
        //    catch (Exception e)
        //    {
        //        res.OK = false;
        //        LogException(ctx, e);
        //    }

        //    proto.ReturnResult(session, res);
        //}

        //private void OnGenProto(Session session, GenProto proto)
        //{
        //    var timer = new ProfileTimer();

        //    timer.StartPhase("服务器生成代码");

        //    var res = new GenProtoRes();

        //    string relatePath = proto.Arg.OutputCodeRelatePath;

        //    DefAssembly ass = new DefAssembly();
        //    var ctx = new SessionContext(session, proto.Arg.Verbose);
        //    try
        //    {
        //        var genCodeTasks = new List<Task>();
        //        var outputFiles = new ConcurrentBag<FileInfo>();

        //        ass.Load(EJobType.PROTO, proto.Arg.Define, ctx);
        //        var outputSyncFiles = new ConcurrentBag<FileInfo>();

        //        switch (proto.Arg.Define.Language)
        //        {
        //            case "cs":
        //            {
        //                var render = new CsRender();
        //                foreach (var c in ass.Types.Values)
        //                {
        //                    genCodeTasks.Add(Task.Run(() =>
        //                    {
        //                        var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cs");
        //                        var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                        var md5 = GenMd5AndAddCache(file, content);
        //                        outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    }));
        //                }
        //                genCodeTasks.Add(Task.Run(() =>
        //                {
        //                    var module = ass.TopModule;
        //                    var name = "ProtocolStub";
        //                    var content = RenderUtils.ConcatAutoGenerationHeader(
        //                        render.RenderStubs(name, module,
        //                        ass.Types.Values.Where(t => t is PProto).ToList(),
        //                        ass.Types.Values.Where(t => t is PRpc).ToList()),
        //                        "cs");
        //                    var file = FileUtil.GetCsDefTypePath(name);
        //                    var md5 = GenMd5AndAddCache(file, content);
        //                    outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                }));
        //                break;
        //            }
        //            case "lua":
        //            {
        //                genCodeTasks.Add(Task.Run(() =>
        //                {
        //                    var render = new Generate.Proto.LuaRender();
        //                    var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderTypes(ass.Types.Values.ToList()), "lua");
        //                    var file = "ProtoTypes.lua";
        //                    var md5 = GenMd5AndAddCache(file, content);
        //                    outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                }));
        //                break;
        //            }
        //            default:
        //            {
        //                throw new Exception($"unknown lan:{proto.Arg.Define.Language}");
        //            }
        //        }

        //        Task.WhenAll(genCodeTasks).Wait();
        //        res.NewCodeFiles = outputFiles.ToList();

        //        ctx.Info(timer.EndPhaseAndLogToString());

        //        res.OK = true;
        //    }
        //    catch (Exception e)
        //    {
        //        res.OK = false;
        //        LogException(ctx, e);
        //    }

        //    proto.ReturnResult(session, res);
        //}

        //private void OnGenRep(Session session, GenRep proto)
        //{
        //    var timer = new ProfileTimer();

        //    timer.StartPhase("服务器生成代码");

        //    var res = new GenRepRes();

        //    string relatePath = proto.Arg.OutputCodeRelatePath;

        //    DefAssembly ass = new DefAssembly();
        //    var ctx = new SessionContext(session, proto.Arg.Verbose);
        //    try
        //    {
        //        var genCodeTasks = new List<Task>();
        //        var outputFiles = new ConcurrentBag<FileInfo>();

        //        ass.Load(EJobType.REP, proto.Arg.Define, ctx);
        //        var outputSyncFiles = new ConcurrentBag<FileInfo>();

        //        switch (proto.Arg.Define.Language)
        //        {
        //            case "cs":
        //            {
        //                var render = new Generate.Rep.CsRender();
        //                foreach (var c in ass.Types.Values)
        //                {
        //                    genCodeTasks.Add(Task.Run(() =>
        //                    {
        //                        var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderAny(c), "cs");
        //                        var file = FileUtil.GetCsDefTypePath(c.FullName);
        //                        var md5 = GenMd5AndAddCache(file, content);

        //                        outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                    }));
        //                }
        //                genCodeTasks.Add(Task.Run(() =>
        //                {
        //                    var module = ass.TopModule;
        //                    var name = "RepStub";
        //                    var content = RenderUtils.ConcatAutoGenerationHeader(
        //                        render.RenderStubs(name, module, ass.Types.Values.Where(t => (t is DefActor) || (t is DefComponent)).ToList()),
        //                        "cs");
        //                    var file = FileUtil.GetCsDefTypePath(name);
        //                    var md5 = GenMd5AndAddCache(file, content);
        //                    outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                }));
        //                break;
        //            }
        //            case "lua":
        //            {
        //                genCodeTasks.Add(Task.Run(() =>
        //                {
        //                    var render = new Generate.Rep.LuaRender();
        //                    var content = RenderUtils.ConcatAutoGenerationHeader(render.RenderTypes(ass.Types.Values.ToList()), "lua");
        //                    var file = "RepTypes.lua";
        //                    var md5 = GenMd5AndAddCache(file, content);
        //                    outputFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        //                }));
        //                break;
        //            }
        //            default:
        //            {
        //                throw new Exception($"unknown lan:{proto.Arg.Define.Language}");
        //            }
        //        }

        //        Task.WhenAll(genCodeTasks).Wait();
        //        res.NewCodeFiles = outputFiles.ToList();

        //        ctx.Info(timer.EndPhaseAndLogToString());

        //        res.OK = true;
        //    }
        //    catch (Exception e)
        //    {
        //        res.OK = false;
        //        LogException(ctx, e);
        //    }

        //    proto.ReturnResult(session, res);
        //}
    }
}
