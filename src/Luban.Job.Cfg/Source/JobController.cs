using Bright.Serialization;
using Bright.Time;
using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
using Luban.Config.Common.RawDefs;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataVisitors;
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

        class GenArgs
        {
            [Option('d', "define_file", Required = true, HelpText = "define file")]
            public string DefineFile { get; set; }

            [Option("input_data_dir", Required = true, HelpText = "input data dir")]
            public string InputDataDir { get; set; }

            [Option('v', "validate_root_dir", Required = false, HelpText = "validate root directory")]
            public string ValidateRootDir { get; set; }

            [Option("output_code_dir", Required = false, HelpText = "output code directory")]
            public string OutputCodeDir { get; set; }

            [Option("output_data_dir", Required = true, HelpText = "output data directory")]
            public string OutputDataDir { get; set; }

            [Option("gen_types", Required = true, HelpText = "code_cs_bin,code_cs_json,code_lua_bin,data_bin,data_lua,data_json can be multi")]
            public string GenType { get; set; }

            [Option('s', "service", Required = true, HelpText = "service")]
            public string Service { get; set; }

            [Option("export_test_data", Required = false, HelpText = "export test data")]
            public bool ExportTestData { get; set; } = false;

            [Option('t', "timezone", Required = false, HelpText = "timezone")]
            public string TimeZone { get; set; }
        }

        private async Task LoadCfgDataAsync(RemoteAgent agent, DefAssembly ass, string dataDir, bool exportTestData)
        {
            var ctx = agent;
            List<DefTable> exportTables = ass.Types.Values.Where(t => t is DefTable ct && ct.NeedExport).Select(t => (DefTable)t).ToList();
            var genDataTasks = new List<Task>();
            var outputDataFiles = new ConcurrentBag<FileInfo>();
            long genDataStartTime = TimeUtil.NowMillis;

            foreach (DefTable c in exportTables)
            {
                genDataTasks.Add(Task.Run(async () =>
                {
                    long beginTime = TimeUtil.NowMillis;
                    await LoadTableAsync(agent, c, dataDir, exportTestData);
                    long endTime = TimeUtil.NowMillis;
                    if (endTime - beginTime > 100)
                    {
                        ctx.Info("====== load {0} cost {1} ms ======", c.FullName, (endTime - beginTime));
                    }
                }));
            }
            await Task.WhenAll(genDataTasks.ToArray());
        }

        private ICodeRender CreateCodeRender(string genType)
        {
            switch (genType)
            {
                case "code_cs_bin": return new CsBinCodeRender();
                case "code_cs_json": return new CsJsonCodeRender();
                case "code_java_bin": return new JavaBinCodeRender();
                case "code_cpp_bin": return new CppBinCodeRender();
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
                case "code_cpp_bin": return ELanguage.CPP;
                case "code_go_bin": return ELanguage.GO;
                case "code_lua_bin": return ELanguage.LUA;
                default: throw new ArgumentException($"not support output data type:{genType}");
            }
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
            else
            {
                result = (parseResult as Parsed<GenArgs>).Value;
                errMsg = null;

                string inputDataDir = result.InputDataDir;
                string outputCodeDir = result.OutputCodeDir;
                string outputDataDir = result.OutputDataDir;

                var genTypes = result.GenType.Split(',').Select(s => s.Trim()).ToList();

                if (genTypes.Any(t => t.StartsWith("code_", StringComparison.Ordinal)) && string.IsNullOrWhiteSpace(outputCodeDir))
                {
                    errMsg = "--outputcodedir missing";
                    return false;
                }
                else if (genTypes.Any(t => t.StartsWith("data_", StringComparison.Ordinal)))
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
                timer.EndPhaseAndLog();

                var rawDefines = loader.BuildDefines();

                TimeZoneInfo timeZoneInfo = string.IsNullOrEmpty(args.TimeZone) ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(args.TimeZone);

                var ass = new DefAssembly(timeZoneInfo);

                ass.Load(args.Service, rawDefines, agent);

                var targetService = ass.CfgTargetService;


                List<DefTable> exportTables = ass.GetExportTables();
                List<DefTypeBase> exportTypes = ass.GetExportTypes();

                bool hasLoadCfgData = false;


                async Task CheckLoadCfgDataAsync()
                {
                    if (!hasLoadCfgData)
                    {
                        hasLoadCfgData = true;
                        var timer = new ProfileTimer();
                        timer.StartPhase("load config data");
                        await LoadCfgDataAsync(agent, ass, args.InputDataDir, args.ExportTestData);
                        timer.EndPhaseAndLog();

                        timer.StartPhase("validate");
                        var validateCtx = new ValidatorContext(ass, args.ValidateRootDir);
                        await validateCtx.ValidateTables(exportTables);
                        timer.EndPhaseAndLog();
                    }
                }

                var tasks = new List<Task>();

                var genCodeFiles = new ConcurrentBag<FileInfo>();
                var genDataFiles = new ConcurrentBag<FileInfo>();

                foreach (var genType in genTypes)
                {
                    switch (genType)
                    {
                        case "code_cs_bin":
                        case "code_cs_json":

                        case "code_java_bin":
                        {
                            ICodeRender render = CreateCodeRender(genType);
                            ELanguage lan = GetLanguage(genType);

                            foreach (var c in exportTypes)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), lan);
                                    var file = RenderFileUtil.GetDefTypePath(c.FullName, lan);
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }

                            tasks.Add(Task.Run(() =>
                            {
                                var module = ass.TopModule;
                                var name = targetService.Manager;
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), lan);
                                var file = RenderFileUtil.GetDefTypePath(name, lan);
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));

                            break;
                        }
                        case "code_lua_bin":
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                var render = new LuaRender();
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAll(ass.Types.Values.ToList()), ELanguage.LUA);
                                var file = "Types.lua";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                            break;
                        }
                        case "code_go_bin":
                        {
                            var render = new GoCodeRender();
                            foreach (var c in exportTypes)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.GO);
                                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ELanguage.GO);
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }

                            tasks.Add(Task.Run(() =>
                            {
                                var module = ass.TopModule;
                                var name = targetService.Manager;
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), ELanguage.GO);
                                var file = RenderFileUtil.GetDefTypePath(name, ELanguage.GO);
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                            break;
                        }
                        case "code_cpp_bin":
                        {
                            var render = new CppBinCodeRender();


                            // 将所有 头文件定义 生成到一个文件
                            // 按照 const,enum,bean,table, service 的顺序生成

                            tasks.Add(Task.Run(() =>
                            {
                                var headerFileContent = new List<string>
                                {
                                    @$"
#pragma once
#include <functional>

#include ""bright/serialization/ByteBuf.h""
#include ""bright/CfgBean.hpp""

using ByteBuf = bright::serialization::ByteBuf;

namespace {ass.TopModule}
{{

"
                                };

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefEnum e)
                                    {
                                        headerFileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefConst c)
                                    {
                                        headerFileContent.Add(render.Render(c));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefBean e)
                                    {
                                        headerFileContent.Add(render.RenderForwardDefine(e));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefBean e)
                                    {
                                        headerFileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTables)
                                {
                                    headerFileContent.Add(render.Render(type));
                                }

                                headerFileContent.Add(render.RenderService("Tables", ass.TopModule, exportTables));

                                headerFileContent.Add("}"); // end of topmodule

                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', headerFileContent), ELanguage.CPP);
                                var file = "gen_types.h";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));

                            var beanTypes = exportTypes.Where(c => c is DefBean).ToList();

                            int TYPE_PER_STUB_FILE = 100;

                            for (int i = 0, n = (beanTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() =>
                                {
                                    int startIndex = index * TYPE_PER_STUB_FILE;
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                                        render.RenderStub(ass.TopModule, beanTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, beanTypes.Count - startIndex))),
                                        ELanguage.CPP);
                                    var file = $"gen_stub_{index}.cpp";
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }

                        case "code_typescript_json":
                        {
                            var render = new TypeScriptJsonCodeRender();
                            tasks.Add(Task.Run(() =>
                            {
                                var fileContent = new List<string>
                                {
                                    @$"
export namespace {ass.TopModule} {{
",

                                    @"
export class Vector2 {
        x: number;
        y: number;
        constructor(x: number, y: number) {
            this.x = x;
            this.y = y;
        }

        static fromJson(_json_: any): Vector2 {
            let x = _json_['x'];
            let y = _json_['y'];
            if (x == null || y == null) {
                throw new Error();
            }
            return new Vector2(x, y);
        }
    }


    export class Vector3 {
        x: number;
        y: number;
        z: number;
        constructor(x: number, y: number, z: number) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        static fromJson(_json_: any): Vector3 {
            let x = _json_['x'];
            let y = _json_['y'];
            let z = _json_['z'];
            if (x == null || y == null || z == null) {
                throw new Error();
            }
            return new Vector3(x, y, z);
        }
    }

    export class Vector4 {
        x: number;
        y: number;
        z: number;
        w: number;
        constructor(x: number, y: number, z: number, w: number) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        static fromJson(_json_: any): Vector4 {
            let x = _json_['x'];
            let y = _json_['y'];
            let z = _json_['z'];
            let w = _json_['w'];
            if (x == null || y == null || z == null || w == null) {
                throw new Error();
            }
            return new Vector4(x, y, z, w);
        }
    }

"
                                };

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefEnum e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefConst c)
                                    {
                                        fileContent.Add(render.Render(c));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefBean e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTables)
                                {
                                    fileContent.Add(render.Render(type));
                                }

                                fileContent.Add(render.RenderService("Tables", ass.TopModule, exportTables));

                                fileContent.Add("}"); // end of topmodule

                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', fileContent), ELanguage.TYPESCRIPT);
                                var file = "Types.ts";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                            break;
                        }

                        case "code_python27_json":
                        {
                            var render = new Python27JsonCodeRender();

                            tasks.Add(Task.Run(() =>
                            {
                                var fileContent = new List<string>
                                {
                                    @"
class Vector2:
        def __init__(self, x, y):
            self.x = x
            self.y = y
            self.a = Vector4(1,2,3,4)
        def __str__(self):
            return '{%g,%g}' % (self.x, self.y)

        @staticmethod
        def fromJson(_json_):
            x = _json_['x']
            y = _json_['y']
            if (x == None or y == None):
                raise Exception()
            return Vector2(x, y)


class Vector3:
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z
    def __str__(self):
        return '{%f,%f,%f}' % (self.x, self.y, self.z)
    @staticmethod
    def fromJson(_json_):
        x = _json_['x']
        y = _json_['y']
        z = _json_['z']
        if (x == None or y == None or z == None):
            raise Exception()
        return Vector3(x, y, z)

class Vector4:
    def __init__(self, x, y, z, w):
        self.x = x
        self.y = y
        self.z = z
        self.w = w
    def __str__(self):
        return '{%g,%g,%g,%g}' % (self.x, self.y, self.z, self.w)
        
    @staticmethod
    def fromJson(_json_):
        x = _json_['x']
        y = _json_['y']
        z = _json_['z']
        w = _json_['w']
        if (x == None or y == None or z == None or w == None):
            raise Exception()
        return Vector4(x, y, z, w)

"
                                };

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefEnum e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefConst c)
                                    {
                                        fileContent.Add(render.Render(c));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefBean e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTables)
                                {
                                    fileContent.Add(render.Render(type));
                                }

                                fileContent.Add(render.RenderService("Tables", ass.TopModule, exportTables));

                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', fileContent), ELanguage.PYTHON);
                                var file = "Types.py";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });


                                {
                                    var moduleInitContent = "";
                                    var initFile = "__init__.py";

                                    var initMd5 = CacheFileUtil.GenMd5AndAddCache(initFile, moduleInitContent);
                                    genCodeFiles.Add(new FileInfo() { FilePath = initFile, MD5 = initMd5 });
                                }
                            }));
                            break;
                        }

                        case "code_python3_json":
                        {
                            var render = new Python3JsonCodeRender();

                            tasks.Add(Task.Run(() =>
                            {
                                var fileContent = new List<string>
                                {
                                    @"
from enum import Enum
import abc

class Vector2:
        def __init__(self, x, y):
            self.x = x
            self.y = y
            self.a = Vector4(1,2,3,4)
        def __str__(self):
            return '{%g,%g}' % (self.x, self.y)

        @staticmethod
        def fromJson(_json_):
            x = _json_['x']
            y = _json_['y']
            if (x == None or y == None):
                raise Exception()
            return Vector2(x, y)


class Vector3:
    def __init__(self, x, y, z):
        self.x = x
        self.y = y
        self.z = z
    def __str__(self):
        return '{%f,%f,%f}' % (self.x, self.y, self.z)
    @staticmethod
    def fromJson(_json_):
        x = _json_['x']
        y = _json_['y']
        z = _json_['z']
        if (x == None or y == None or z == None):
            raise Exception()
        return Vector3(x, y, z)

class Vector4:
    def __init__(self, x, y, z, w):
        self.x = x
        self.y = y
        self.z = z
        self.w = w
    def __str__(self):
        return '{%g,%g,%g,%g}' % (self.x, self.y, self.z, self.w)
        
    @staticmethod
    def fromJson(_json_):
        x = _json_['x']
        y = _json_['y']
        z = _json_['z']
        w = _json_['w']
        if (x == None or y == None or z == None or w == None):
            raise Exception()
        return Vector4(x, y, z, w)

"
                                };

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefEnum e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefConst c)
                                    {
                                        fileContent.Add(render.Render(c));
                                    }
                                }

                                foreach (var type in exportTypes)
                                {
                                    if (type is DefBean e)
                                    {
                                        fileContent.Add(render.Render(e));
                                    }
                                }

                                foreach (var type in exportTables)
                                {
                                    fileContent.Add(render.Render(type));
                                }

                                fileContent.Add(render.RenderService("Tables", ass.TopModule, exportTables));

                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(string.Join('\n', fileContent), ELanguage.PYTHON);
                                var file = "Types.py";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                            break;
                        }

                        case "code_cpp_ue_editor":
                        {
                            var render = new UE4EditorCppRender();

                            var renderTypes = ass.Types.Values.Where(c => c is DefEnum || c is DefBean).ToList();

                            foreach (var c in renderTypes)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                                    var file = "editor_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }

                            int TYPE_PER_STUB_FILE = 200;

                            for (int i = 0, n = (renderTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
                            {
                                int index = i;
                                tasks.Add(Task.Run(() =>
                                {
                                    int startIndex = index * TYPE_PER_STUB_FILE;
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                                        render.RenderStub(renderTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, renderTypes.Count - startIndex))),
                                        ELanguage.CPP);
                                    var file = $"stub_{index}.cpp";
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "code_cs_unity_editor":
                        {
                            var render = new EditorCsRender();
                            foreach (var c in ass.Types.Values)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CS);
                                    var file = RenderFileUtil.GetDefTypePath(c.FullName, ELanguage.GO);
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "code_cpp_ue_bp":
                        {
                            var render = new UE4BpCppRender();
                            foreach (var c in exportTypes)
                            {
                                if (!(c is DefEnum || c is DefBean))
                                {
                                    continue;
                                }

                                tasks.Add(Task.Run(() =>
                                {
                                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                                    var file = "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                    genCodeFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "data_bin":
                        case "data_json":
                        {
                            await CheckLoadCfgDataAsync();
                            foreach (var c in exportTables)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = ToOutputData(c, ass.GetTableDataList(c), genType);
                                    var file = genType.EndsWith("json") ? c.JsonOutputDataFile : c.OutputDataFile;
                                    var md5 = FileUtil.CalcMD5(content);
                                    CacheManager.Ins.AddCache(file, md5, content);
                                    genDataFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "data_lua":
                        {
                            await CheckLoadCfgDataAsync();

                            tasks.Add(Task.Run(() =>
                            {
                                var render = new LuaRender();
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderDefines(ass.Types.Values.ToList()), ELanguage.LUA);
                                var file = "Types.lua";
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genDataFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });

                            }));

                            foreach (var c in exportTables)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = ToOutputData(c, ass.GetTableDataList(c), genType);
                                    var file = $"{c.Name}.lua";
                                    var md5 = FileUtil.CalcMD5(content);
                                    CacheManager.Ins.AddCache(file, md5, content);
                                    genDataFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "data_resources":
                        {
                            await CheckLoadCfgDataAsync();
                            var genDataTasks = new List<Task<List<ResourceInfo>>>();
                            foreach (var c in exportTables)
                            {
                                genDataTasks.Add(Task.Run(() =>
                                {
                                    return ExportResourceList(ass.GetTableDataList(c));
                                }));
                            }

                            tasks.Add(Task.Run(async () =>
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
                                var file = "resources.txt";
                                var contents = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", resourceLines));
                                var md5 = FileUtil.CalcMD5(contents);
                                CacheManager.Ins.AddCache(file, md5, contents);

                                // 不这么处理???
                                genDataFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
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

                if (genCodeFiles.Count > 0)
                {
                    res.FileGroups.Add(new FileGroup() { Dir = outputCodeDir, Files = genCodeFiles.ToList() });
                }
                if (genDataFiles.Count > 0)
                {
                    res.FileGroups.Add(new FileGroup() { Dir = outputDataDir, Files = genDataFiles.ToList() });
                }
            }
            catch (Exception e)
            {
                res.ErrCode = Luban.Common.EErrorCode.JOB_EXCEPTION;
                res.ErrMsg = $"{e.Message} \n {e.StackTrace}";
            }
            timer.EndPhaseAndLog();

            agent.Session.ReplyRpc<GenJob, GenJobArg, GenJobRes>(rpc, res);
        }

        public string GetActualFileName(string file)
        {
            int index = file.IndexOf('@');
            return index >= 0 ? file[(index + 1)..] : file;
        }

        private List<DType> LoadCfgRecords(DefTable table, string originFile, string sheetName, byte[] content, bool multiRecord, bool exportTestData)
        {
            // (md5,sheet,multiRecord,exportTestData) -> (valuetype, List<(datas)>)
            var dataSourc = DataSourceFactory.Create(originFile, sheetName, new MemoryStream(content), exportTestData);
            try
            {
                List<DType> datas;
                if (multiRecord)
                {
                    datas = dataSourc.ReadMulti(table.ValueTType);
                }
                else
                {
                    datas = new List<DType> { dataSourc.ReadOne(table.ValueTType) };
                }
                foreach (var data in datas)
                {
                    data.Source = originFile;
                }
                return datas;
            }
            catch (Exception e)
            {
                throw new Exception($"配置文件:{originFile} 生成失败. ==> {e.Message}", e);
            }
        }

        class InputFileInfo
        {
            public string MD5 { get; set; }

            public string OriginFile { get; set; }

            public string ActualFile { get; set; }

            public string SheetName { get; set; }
        }



        private async Task<List<InputFileInfo>> CollectInputFilesAsync(RemoteAgent agent, DefTable table, string dataDir)
        {
            var collectTasks = new List<Task<List<InputFileInfo>>>();
            foreach (var file in table.InputFiles)
            {
                (var actualFile, var sheetName) = RenderFileUtil.SplitFileAndSheetName(FileUtil.Standardize(file));
                var actualFullPath = FileUtil.Combine(dataDir, actualFile);
                var originFullPath = FileUtil.Combine(dataDir, file);
                //s_logger.Info("== get input file:{file} actualFile:{actual}", file, actualFile);

                collectTasks.Add(Task.Run(async () =>
                {
                    var fileOrDirContent = await agent.GetFileOrDirectoryAsync(actualFullPath);
                    if (fileOrDirContent.IsFile)
                    {
                        return new List<InputFileInfo> { new InputFileInfo() { OriginFile = file, ActualFile = actualFullPath, SheetName = sheetName, MD5 = fileOrDirContent.Md5 } };
                    }
                    else
                    {
                        return fileOrDirContent.SubFiles.Select(f => new InputFileInfo() { OriginFile = f.FilePath, ActualFile = f.FilePath, MD5 = f.MD5 }).ToList();
                    }
                }));
            }

            var allFiles = new List<InputFileInfo>();
            foreach (var t in collectTasks)
            {
                allFiles.AddRange(await t);
            }
            return allFiles;
        }

        public async Task LoadTableAsync(RemoteAgent agent, DefTable table, string dataDir, bool exportTestData)
        {
            var tasks = new List<Task<List<DType>>>();

            var inputFiles = await CollectInputFilesAsync(agent, table, dataDir);

            // check cache (table, exporttestdata) -> (list<InputFileInfo>, List<DType>)
            // (md5, sheetName,exportTestData) -> (value_type, List<DType>)

            foreach (var file in inputFiles)
            {
                var actualFile = file.ActualFile;
                //s_logger.Info("== get input file:{file} actualFile:{actual}", file, actualFile);

                tasks.Add(Task.Run(async () =>
                {
                    if (FileRecordCacheManager.Ins.TryGetCacheLoadedRecords(table, file.MD5, actualFile, file.SheetName, exportTestData, out var cacheRecords))
                    {
                        return cacheRecords;
                    }
                    var res = LoadCfgRecords(table,
                        file.OriginFile,
                        file.SheetName,
                        await agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5),
                        RenderFileUtil.IsExcelFile(file.ActualFile),
                        exportTestData);

                    FileRecordCacheManager.Ins.AddCacheLoadedRecords(table, file.MD5, file.SheetName, exportTestData, res);

                    return res;
                }));
            }

            var records = new List<DType>(tasks.Count);
            foreach (var task in tasks)
            {
                records.AddRange(await task);
            }

            s_logger.Trace("== load recors. count:{count}", records.Count);

            table.Assembly.AddDataTable(table, records);

            s_logger.Trace("table:{name} record num:{num}", table.FullName, records.Count);
        }

        private byte[] ToOutputData(DefTable table, List<DType> records, string dataType)
        {
            switch (dataType)
            {
                case "data_bin":
                {
                    var buf = ThreadLocalTemporalByteBufPool.Alloc(1024 * 1024);
                    BinaryExportor.Ins.WriteList(records, buf);
                    var bytes = buf.CopyData();
                    ThreadLocalTemporalByteBufPool.Free(buf);
                    return bytes;
                }
                case "data_json":
                {
                    var ss = new MemoryStream();
                    var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
                    {
                        Indented = true,
                        SkipValidation = false,
                    });
                    JsonExportor.Ins.WriteList(records, jsonWriter);
                    jsonWriter.Flush();
                    return DataUtil.StreamToBytes(ss);
                }
                case "data_lua":
                {
                    var content = new List<string>();

                    switch (table.Mode)
                    {
                        case ETableMode.ONE:
                        {
                            LuaExportor.Ins.ExportTableOne(table, records, content);
                            break;
                        }
                        case ETableMode.MAP:
                        {
                            LuaExportor.Ins.ExportTableOneKeyMap(table, records, content);
                            break;
                        }
                        case ETableMode.BMAP:
                        {
                            LuaExportor.Ins.ExportTableTwoKeyMap(table, records, content);
                            break;
                        }
                        default:
                        {
                            throw new NotSupportedException();
                        }
                    }
                    return System.Text.Encoding.UTF8.GetBytes(string.Join('\n', content));
                }
                default:
                {
                    throw new ArgumentException($"not support datatype:{dataType}");
                }
            }
        }

        private List<ResourceInfo> ExportResourceList(List<DType> records)
        {
            var resList = new List<ResourceInfo>();
            foreach (DBean res in records)
            {
                ResourceExportor.Ins.Accept(res, null, resList);
            }
            return resList;
        }
    }
}
