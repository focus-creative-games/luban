using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class FieldInfo
    {
        public string Type { get; init; }

        public string BriefDesc { get; init; }

        public string DetailDesc { get; init; }
    }

    class RawSheetTableDefInfo
    {
        public Dictionary<string, FieldInfo> FieldInfos { get; init; }
    }
}
