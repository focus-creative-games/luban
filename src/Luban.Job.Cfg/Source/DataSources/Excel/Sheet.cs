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

    class Sheet
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; }

        public string RawUrl { get; }

        public Sheet(string rawUrl, string name)
        {
            this.RawUrl = rawUrl;
            this.Name = name;
        }

        public void Load(RawSheet rawSheet)
        {

        }

        public IEnumerable<TitleRow> GetRows()
        {
            yield return null;
        }
    }
}
