using Bright.Common;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Server.Common;
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
        public static async Task LoadDataToCurrentDoc(string rootDefineFile, string inputDataDir, string tableName)
        {
            IAgent agent = new LocalAgent();
            var loader = new CfgDefLoader(agent);
            await loader.LoadAsync(rootDefineFile);

            var rawDefines = loader.BuildDefines();

            TimeZoneInfo timeZoneInfo = null;

            var excludeTags = new List<string>();
            var ass = new DefAssembly("", timeZoneInfo, excludeTags, agent);

            ass.Load(rawDefines);

            DefAssemblyBase.LocalAssebmly = ass;

            var table = ass.GetCfgTable(tableName);

            if (table == null)
            {
                throw new Exception($"table:{tableName}不存在");
            }
            await DataLoaderUtil.LoadTableAsync(agent, table, inputDataDir, "", "");

            var datas = ass.GetTableAllDataList(table);
            MessageBox.Show($"table:{table.FullName} input:{StringUtil.CollectionToString(table.InputFiles)} record num:{datas.Count}");
        }
    }
}
