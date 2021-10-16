using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class FieldInfo
    {
        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public string Type { get; set; }

        public string BriefDesc { get; set; }

        public string DetailDesc { get; set; }
    }

    class RawSheetTableDefInfo
    {
        public Dictionary<string, FieldInfo> FieldInfos { get; set; }
    }
}
