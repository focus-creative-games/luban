using Luban.DataLoader.Builtin.DataVisitors;
using Luban.DataLoader.Builtin.Lite;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Excel.DataParser;


public class LiteParser : TextParserBase<LiteStream>
{
    protected override LiteStream CreateRawData(string dataStr)
    {
        return new LiteStream(dataStr);
    }

    protected override DBean ParseBean(TBean type, LiteStream rawData, TitleRow title)
    {
        return (DBean)type.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override DType ParseCollection(TType collectionType, LiteStream rawData, TitleRow title)
    {
        return collectionType.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override DMap ParseMap(TMap type, LiteStream rawData, TitleRow title)
    {
        return (DMap)type.Apply(LiteStreamDataCreator.Ins, rawData);
    }

    protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, LiteStream rawData, TitleRow title)
    {
        rawData.ReadStructOrCollectionBegin();
        var key = type.KeyType.Apply(LiteStreamDataCreator.Ins, rawData);
        var value = type.ValueType.Apply(LiteStreamDataCreator.Ins, rawData);
        rawData.ReadStructOrCollectionEnd();
        return new KeyValuePair<DType, DType>(key, value);
    }
}
