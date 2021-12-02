using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public class Defines : DefinesCommon
    {
        public List<Patch> Patches { get; set; } = new List<Patch>();

        public List<Table> Tables { get; set; } = new List<Table>();

        public List<Group> Groups { get; set; } = new List<Group>();

        public List<Service> Services { get; set; } = new List<Service>();
    }
}
