using Luban.DataLoader.Builtin.DataVisitors;
using Luban.DataLoader.Builtin.Lua;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Neo.IronLua;
using System.Data;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public class LuaParser : TextParserBase<LuaTable>
{

    protected override LuaTable CreateRawData(string dataStr)
    {
        var env = LuaDataSource.LuaManager.CreateEnvironment();
        return (LuaTable)env.DoChunk("return " + dataStr, "_")[0];
    }

    protected override DBean ParseBean(TBean type, LuaTable rawData, TitleRow title)
    {
        return (DBean)type.Apply(LuaDataCreator.Ins, rawData, type.DefBean.Assembly);
    }

    protected override DType ParseCollection(TType collectionType, LuaTable rawData, TitleRow title)
    {
        return collectionType.Apply(LuaDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override DMap ParseMap(TMap type, LuaTable rawData, TitleRow title)
    {
        return (DMap)type.Apply(LuaDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, LuaTable rawData, TitleRow title)
    {
        DType key = type.KeyType.Apply(LuaDataCreator.Ins, rawData[0], GenerationContext.Current.Assembly);
        DType value = type.ValueType.Apply(LuaDataCreator.Ins, rawData[1], GenerationContext.Current.Assembly);
        return new KeyValuePair<DType, DType>(key, value);
    }
}
