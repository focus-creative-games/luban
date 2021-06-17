using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public class Service
    {
        public string Name { get; set; }

        public string Manager { get; set; }

        public List<string> Groups { get; set; } = new List<string>();

        public List<string> Refs { get; set; } = new List<string>();
    }
}
