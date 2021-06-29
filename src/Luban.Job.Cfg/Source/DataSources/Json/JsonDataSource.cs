using Luban.Job.Cfg.DataCreators;
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
            RawUrl = rawUrl;
            this._data = JsonDocument.Parse(stream).RootElement;
        }

        public override List<Record> ReadMulti(TBean type)
        {
            throw new NotImplementedException();
        }

        public override Record ReadOne(TBean type)
        {
            bool isTest = false;
            if (_data.TryGetProperty(TAG_KEY, out var tagEle))
            {
                var tagName = tagEle.GetString();
                if (IsIgnoreTag(tagName))
                {
                    return null;
                }
                isTest = IsTestTag(tagName);
            }

            var data = (DBean)type.Apply(JsonDataCreator.Ins, _data, (DefAssembly)type.Bean.AssemblyBase);
            return new Record(data, RawUrl, isTest);
        }
    }
}
