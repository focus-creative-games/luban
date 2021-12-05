using Luban.Job.Cfg.RawDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Defs
{
    public class DefRefGroup
    {
        public string Name { get; }

        public List<string> Refs { get; }

        public DefRefGroup(RefGroup group)
        {
            this.Name = group.Name;
            this.Refs = group.Refs;
        }
    }
}
