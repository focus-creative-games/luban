using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Proto.RawDefs
{
    public class Defines : DefinesCommon
    {
        public List<Service> ProtoServices { get; set; } = new List<Service>();

        public List<PProto> Protos { get; set; } = new List<PProto>();

        public List<PRpc> Rpcs { get; set; } = new List<PRpc>();
    }
}
