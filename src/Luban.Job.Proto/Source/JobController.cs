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

                        tasks.Add(Task.Run(() =>
                        {
                            var fileContent = new List<string>
                                {
                                    @$"
import {{Bright}} from 'csharp'

export namespace {ass.TopModule} {{
",

                                    @"
export interface ISerializable {
    serialize(_buf_: Bright.Serialization.ByteBuf): void
    deserialize(_buf_: Bright.Serialization.ByteBuf): void
}

export abstract class BeanBase implements ISerializable {
    abstract getTypeId(): number
    abstract serialize(_buf_: Bright.Serialization.ByteBuf): void
    abstract deserialize(_buf_: Bright.Serialization.ByteBuf): void
}

export abstract class Protocol implements ISerializable {
    abstract getTypeId(): number
    abstract serialize(_buf_: Bright.Serialization.ByteBuf): void
    abstract deserialize(_buf_: Bright.Serialization.ByteBuf): void
}

export class Vector2 {
        x: number
        y: number
        constructor(x: number, y: number) {
            this.x = x
            this.y = y
        }

        to(_buf_: Bright.Serialization.ByteBuf) {
            _buf_.WriteFloat(this.x)
            _buf_.WriteFloat(this.y)
        }

        static from(_buf_: Bright.Serialization.ByteBuf): Vector2 {
            let x = _buf_.ReadFloat()
            let y = _buf_.ReadFloat()
            return new Vector2(x, y)
        }
    }


    export class Vector3 {
        x: number
        y: number
        z: number
        constructor(x: number, y: number, z: number) {
            this.x = x
            this.y = y
            this.z = z
        }

        to(_buf_: Bright.Serialization.ByteBuf) {
            _buf_.WriteFloat(this.x)
            _buf_.WriteFloat(this.y)
            _buf_.WriteFloat(this.z)
        }

        static from(_buf_: Bright.Serialization.ByteBuf): Vector3 {
            let x = _buf_.ReadFloat()
            let y = _buf_.ReadFloat()
            let z = _buf_.ReadFloat()
            return new Vector3(x, y, z)
        }
    }

    export class Vector4 {
        x: number
        y: number
        z: number
        w: number
        constructor(x: number, y: number, z: number, w: number) {
            this.x = x
            this.y = y
            this.z = z
            this.w = w
        }

        to(_buf_: Bright.Serialization.ByteBuf) {
            _buf_.WriteFloat(this.x)
            _buf_.WriteFloat(this.y)
            _buf_.WriteFloat(this.z)
            _buf_.WriteFloat(this.w)
        }

        static from(_buf_: Bright.Serialization.ByteBuf): Vector4 {
            let x = _buf_.ReadFloat()
            let y = _buf_.ReadFloat()
            let z = _buf_.ReadFloat()
            let w = _buf_.ReadFloat()
            return new Vector4(x, y, z, w)
        }
    }

"
                                };

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
