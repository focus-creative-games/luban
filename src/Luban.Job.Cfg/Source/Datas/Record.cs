using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Datas
{
    public class Record
    {
        public DBean Data { get; }

        public string Source { get; }

        public int Index { get; set; }

        public Record(DBean data, string source)
        {
            Data = data;
            Source = source;
        }
    }
}
