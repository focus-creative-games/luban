using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public class Defines
    {
        public string TopModule { get; set; } = "";

        public List<Patch> Patches { get; set; } = new List<Patch>();

        public List<Bean> Beans { get; set; } = new List<Bean>();

        public List<Const> Consts { get; set; } = new List<Const>();

        public List<PEnum> Enums { get; set; } = new List<PEnum>();

        public List<Table> Tables { get; set; } = new List<Table>();

        public List<Group> Groups { get; set; } = new List<Group>();

        public List<Service> Services { get; set; } = new List<Service>();
    }
}
