using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataSources.Excel
{


    class RawSheet
    {
        public Title Title { get; set; }

        public string TableName { get; set; }

        public List<List<Cell>> Cells { get; set; }
    }
}
