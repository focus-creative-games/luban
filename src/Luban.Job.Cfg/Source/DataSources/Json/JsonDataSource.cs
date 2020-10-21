using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Luban.Job.Cfg.DataSources.Json
{
    class JsonDataSource : AbstractDataSource
    {
        JsonElement _data;

        public override void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData)
        {
            this._data = JsonDocument.Parse(stream).RootElement;
        }

        public override List<DType> ReadMulti(TBean type)
        {
            throw new NotImplementedException();
        }

        public override DType ReadOne(TBean type)
        {
            return type.Apply(JsonDataCreator.Ins, _data, (DefAssembly)type.Bean.AssemblyBase);
        }
    }
}
