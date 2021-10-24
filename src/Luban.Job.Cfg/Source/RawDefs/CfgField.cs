using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public class CfgField : Field
    {
        public List<string> Groups { get; set; } = new List<string>();
    }
}
