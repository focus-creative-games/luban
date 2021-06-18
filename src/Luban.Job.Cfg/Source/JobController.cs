using Bright.Time;
using CommandLine;
using Luban.Common.Protos;
using Luban.Common.Utils;
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

            [Option("output_data_resource_list_file", Required = false, HelpText = "output resource list file")]
            public string OutputDataResourceListFile { get; set; }

            [Option("output_data_json_monolithic_file", Required = false, HelpText = "output monolithic json file")]
            public string OutputDataJsonMonolithicFile { get; set; }

            [Option("gen_types", Required = true, HelpText = "code_cs_bin,code_cs_json,code_lua_bin,data_bin,data_lua,data_json,data_json_monolithic . can be multi")]
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
                    if (genTypes.Contains("data_json_monolithic") && string.IsNullOrWhiteSpace(options.OutputDataJsonMonolithicFile))
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

                ass.Load(args.Service, args.BranchName, rawDefines, agent);

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
                            await DataLoaderUtil.LoadTextTablesAsync(agent, ass, args.InputDataDir, args.InputTextTableFiles);
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }

                            tasks.Add(Task.Run(() =>
                            {
                                var module = ass.TopModule;
                                var name = targetService.Manager;
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), lan);
                                var file = RenderFileUtil.GetDefTypePath(name, lan);
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }

                            tasks.Add(Task.Run(() =>
                            {
                                var module = ass.TopModule;
                                var name = targetService.Manager;
                                var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderService(name, module, exportTables), ELanguage.GO);
                                var file = RenderFileUtil.GetDefTypePath(name, ELanguage.GO);
                                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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

        static from(_json_: any): Vector2 {
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

        static from(_json_: any): Vector3 {
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

        static from(_json_: any): Vector4 {
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                            }));
                            break;
                        }

                        case "code_typescript_bin":
                        {
                            var render = new TypeScriptBinCodeRender();
                            tasks.Add(Task.Run(() =>
                            {
                                var fileContent = new List<string>
                                {
                                    @$"
import {{Bright}} from 'csharp'

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

        static from(_buf_: Bright.Serialization.ByteBuf): Vector2 {
            let x = _buf_.ReadFloat();
            let y = _buf_.ReadFloat();
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

        static from(_buf_: Bright.Serialization.ByteBuf): Vector3 {
            let x = _buf_.ReadFloat();
            let y = _buf_.ReadFloat();
            let z = _buf_.ReadFloat();
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

        static from(_buf_: Bright.Serialization.ByteBuf): Vector4 {
            let x = _buf_.ReadFloat();
            let y = _buf_.ReadFloat();
            let z = _buf_.ReadFloat();
            let w = _buf_.ReadFloat();
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });


                                {
                                    var moduleInitContent = "";
                                    var initFile = "__init__.py";

                                    var initMd5 = CacheFileUtil.GenMd5AndAddCache(initFile, moduleInitContent);
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = initFile, MD5 = initMd5 });
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
                                genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    genCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    var content = DataExporterUtil.ToOutputData(c, ass.GetTableDataList(c), genType);
                                    var file = genType.EndsWith("json") ? c.JsonOutputDataFile : c.OutputDataFile;
                                    var md5 = FileUtil.CalcMD5(content);
                                    CacheManager.Ins.AddCache(file, md5, content);
                                    genDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                                }));
                            }
                            break;
                        }
                        case "data_json_monolithic":
                        {
                            await CheckLoadCfgDataAsync();
                            List<Task<byte[]>> allJsonTask = new List<Task<byte[]>>();
                            foreach (var c in exportTables)
                            {
                                allJsonTask.Add(Task.Run(() =>
                                {
                                    return DataExporterUtil.ToOutputData(c, ass.GetTableDataList(c), "data_json");
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
                            var md5 = FileUtil.CalcMD5(content);
                            var outputFile = args.OutputDataJsonMonolithicFile;
                            CacheManager.Ins.AddCache(outputFile, md5, content);
                            genScatteredFiles.Add(new FileInfo() { FilePath = outputFile, MD5 = md5 });
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
                                genDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });

                            }));

                            foreach (var c in exportTables)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    var content = DataExporterUtil.ToOutputData(c, ass.GetTableDataList(c), genType);
                                    var file = $"{c.Name}.lua";
                                    var md5 = FileUtil.CalcMD5(content);
                                    CacheManager.Ins.AddCache(file, md5, content);
                                    genDataFilesInOutputDataDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
                                    return DataExporterUtil.ExportResourceList(ass.GetTableDataList(c));
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
                                var file = args.OutputDataResourceListFile;
                                var contents = System.Text.Encoding.UTF8.GetBytes(string.Join("\n", resourceLines));
                                var md5 = FileUtil.CalcMD5(contents);
                                CacheManager.Ins.AddCache(file, md5, contents);

                                genScatteredFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
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
