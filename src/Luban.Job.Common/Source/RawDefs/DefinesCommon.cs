using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.RawDefs
{
    public class DefinesCommon
    {
        public string TopModule { get; set; } = "";

        public Dictionary<string, string> Options { get; set; } = new();

        public HashSet<string> ExternalSelectors { get; set; } = new();

        public Dictionary<string, ExternalType> ExternalTypes { get; set; } = new();

        public List<Bean> Beans { get; set; } = new();

        public List<PEnum> Enums { get; set; } = new();
    }
}
