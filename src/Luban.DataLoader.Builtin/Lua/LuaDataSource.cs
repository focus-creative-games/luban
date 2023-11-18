using System.Text;
using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Neo.IronLua;

namespace Luban.DataLoader.Builtin.Lua;

[DataLoader("lua")]
public class LuaDataSource : DataLoaderBase
{
    private static Neo.IronLua.Lua LuaManager { get; } = new();

    private LuaGlobal _env;
    private LuaTable _dataTable;

    public override void Load(string rawUrl, string sheetName, Stream stream)
    {
        RawUrl = rawUrl;
        _env = LuaManager.CreateEnvironment();
        _dataTable = (LuaTable)_env.DoChunk(new StreamReader(stream, Encoding.UTF8), rawUrl)[0];

        if (!string.IsNullOrEmpty(sheetName))
        {
            if (sheetName.StartsWith("*"))
            {
                sheetName = sheetName.Substring(1);
            }
            if (!string.IsNullOrEmpty(sheetName))
            {
                foreach (var subField in sheetName.Split('.'))
                {
                    _dataTable = (LuaTable)_dataTable[subField];
                }
            }
        }
    }

    public override List<Record> ReadMulti(TBean type)
    {
        var records = new List<Record>();

        foreach (LuaTable t in _dataTable.Values.Values)
        {
            Record r = ReadRecord(t, type);
            if (r != null)
            {
                records.Add(r);
            }
        }

        return records;
    }

    public override Record ReadOne(TBean type)
    {
        return ReadRecord(_dataTable, type);
    }

    private Record ReadRecord(LuaTable table, TBean type)
    {
        string tagName = table.GetValue(FieldNames.TagKey)?.ToString();
        if (DataUtil.IsIgnoreTag(tagName))
        {
            return null;
        }
        var data = (DBean)type.Apply(LuaDataCreator.Ins, table, type.DefBean.Assembly);
        var tags = DataUtil.ParseTags(tagName);
        return new Record(data, RawUrl, tags);
    }
}
