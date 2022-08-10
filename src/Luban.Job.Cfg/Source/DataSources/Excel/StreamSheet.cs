using Bright.Collections;
using ExcelDataReader;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{

    class StreamSheet
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; }

        public string RawUrl { get; }

        public ExcelStream Stream { get; private set; }

        public StreamSheet(string rawUrl, string name)
        {
            this.RawUrl = rawUrl;
            this.Name = name;
        }

        public void Load(RawSheet rawSheet)
        {
            Title title = rawSheet.Title;
            Stream = new ExcelStream(rawSheet.Cells, 1, title.ToIndex, "", "");
        }
    }
}
