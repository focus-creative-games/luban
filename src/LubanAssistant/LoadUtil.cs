using Bright.Common;
using Bright.Time;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LubanAssistant
{
    static class LoadUtil
    {
        public static async Task LoadTableDataToCurrentWorkSheetAsync(string rootDefineFile, string inputDataDir, string tableName)
        {
            var tableDataInfo = await DataLoaderUtil.LoadTableDataAsync(rootDefineFile, inputDataDir, tableName);

            Console.WriteLine("load record num:{0}", tableDataInfo.MainRecords.Count);
        }
    }
}
