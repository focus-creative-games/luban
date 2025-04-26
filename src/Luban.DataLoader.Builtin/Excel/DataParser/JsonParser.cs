using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public class JsonParser : TextParserBase<JsonElement>
{
    protected override JsonElement CreateRawData(string dataStr)
    {
        return JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(dataStr)), new JsonDocumentOptions()
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        }).RootElement;
    }

    protected override DBean ParseBean(TBean type, JsonElement rawData, TitleRow title)
    {
        return (DBean)type.Apply(JsonDataCreator.Ins, rawData, type.DefBean.Assembly);
    }

    protected override DType ParseCollection(TType collectionType, JsonElement rawData, TitleRow title)
    {
        return collectionType.Apply(JsonDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override DMap ParseMap(TMap type, JsonElement rawData, TitleRow title)
    {
        return (DMap)type.Apply(JsonDataCreator.Ins, rawData, GenerationContext.Current.Assembly);
    }

    protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, JsonElement rawData, TitleRow title)
    {
        if (rawData.ValueKind != JsonValueKind.Array)
        {
            throw new Exception($"json map entry must be array");
        }
        if (rawData.GetArrayLength() != 2)
        {
            throw new Exception($"json map entry must be [key,value] array");
        }
        DType key = type.KeyType.Apply(JsonDataCreator.Ins, rawData[0], GenerationContext.Current.Assembly);
        DType value = type.ValueType.Apply(JsonDataCreator.Ins, rawData[1], GenerationContext.Current.Assembly);
        return new KeyValuePair<DType, DType>(key, value);
    }
}
