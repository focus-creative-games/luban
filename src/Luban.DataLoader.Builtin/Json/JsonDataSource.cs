using System.Text.Json;
using Luban.Core.DataLoader;
using Luban.Core.Datas;
using Luban.Core.Defs;
using Luban.Core.Types;
using Luban.Core.Utils;
using Luban.DataLoader.Builtin.DataVisitors;

namespace Luban.DataLoader.Builtin.Json;

[DataLoader("json")]
public class JsonDataSource : DataLoaderBase
{
    private JsonElement _data;

    public override void Load(DefTable table, string rawUrl, string sheetOrFieldName, Stream stream)
    {
        RawUrl = rawUrl;
        this._data = JsonDocument.Parse(stream).RootElement;

        if (!string.IsNullOrEmpty(sheetOrFieldName))
        {
            if (sheetOrFieldName.StartsWith("*"))
            {
                sheetOrFieldName = sheetOrFieldName.Substring(1);
            }
            if (!string.IsNullOrEmpty(sheetOrFieldName))
            {
                foreach (var subField in sheetOrFieldName.Split('.'))
                {
                    _data = _data.GetProperty(subField);
                }
            }
        }
    }

    public override List<Record> ReadMulti(DefTable table, TBean type)
    {
        var records = new List<Record>();
        foreach (var ele in _data.EnumerateArray())
        {
            Record rec = ReadRecord(ele, type);
            if (rec != null)
            {
                records.Add(rec);
            }
        }
        return records;
    }

    private Record ReadRecord(JsonElement ele, TBean type)
    {
        List<string> tags;
        if (ele.TryGetProperty(FieldNames.TAG_KEY, out var tagEle))
        {
            var tagName = tagEle.GetString();
            if (DataUtil.IsIgnoreTag(tagName))
            {
                return null;
            }
            tags = DataUtil.ParseTags(tagName);
        }
        else
        {
            tags = null;
        }

        var data = (DBean)type.Apply(JsonDataCreator.Ins, ele, type.DefBean.Assembly);
        return new Record(data, RawUrl, tags);
    }

    public override Record ReadOne(DefTable table, TBean type)
    {
        return ReadRecord(_data, type);
    }
}