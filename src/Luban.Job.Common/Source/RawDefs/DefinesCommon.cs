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

        public HashSet<string> ExternalSelectors { get; set; } = new HashSet<string>();

        public List<Bean> Beans { get; set; } = new List<Bean>();

        public List<PEnum> Enums { get; set; } = new List<PEnum>();
    }
}
