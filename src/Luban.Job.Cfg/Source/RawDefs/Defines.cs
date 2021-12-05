using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public class Defines : DefinesCommon
    {
        public List<Patch> Patches { get; set; } = new();

        public List<Table> Tables { get; set; } = new();

        public List<Group> Groups { get; set; } = new();

        public List<Service> Services { get; set; } = new();

        public List<RefGroup> RefGroups { get; set; } = new();
    }
}
