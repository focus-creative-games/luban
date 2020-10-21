using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using Neo.IronLua;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Luban.Job.Cfg.DataSources.Lua
{
    class LuaDataSource : AbstractDataSource
    {
        private static Neo.IronLua.Lua LuaManager { get; } = new Neo.IronLua.Lua();

        private LuaGlobal _env;
        private LuaTable _dataTable;

        public override void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData)
        {
            _env = LuaManager.CreateEnvironment();
            _dataTable = (LuaTable)_env.DoChunk(new StreamReader(stream, Encoding.UTF8), rawUrl)[0];
        }

        public override List<DType> ReadMulti(TBean type)
        {
            var records = new List<DType>();

            foreach (LuaTable t in _dataTable.Values.Values)
            {
                records.Add(type.Apply(LuaDataCreator.Ins, t, (DefAssembly)type.Bean.AssemblyBase));
            }

            return records;
        }

        public override DType ReadOne(TBean type)
        {
            return type.Apply(LuaDataCreator.Ins, _dataTable, (DefAssembly)type.Bean.AssemblyBase);
        }
    }
}
