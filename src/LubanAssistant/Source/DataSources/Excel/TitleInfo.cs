using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class TitleInfo
    {
        public Title RootTitle { get; }

        public int RowNum { get; }

        public TitleInfo(Title rootTitle, int rowNum)
        {
            RootTitle = rootTitle;
            RowNum = rowNum;
        }
    }
}
