using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Luban.DataLoader.Builtin.Excel.DataParser
{
    public class JsonParser : TextParserBase
    {
        private JsonElement CreateJsonDoc(string text)
        {
            return JsonDocument.Parse(new MemoryStream(Encoding.UTF8.GetBytes(text)), new JsonDocumentOptions()
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            }).RootElement;
        }


        public override DType ParseAny(TType type, List<Cell> cells, TitleRow title)
        {
            if (type is TBean beanType)
            {
                return ParseBean(beanType, cells, title);
            }
            throw new NotSupportedException($"JsonParser 不支持解析类型 {type} 的数据");
        }


        private List<DType> CreateBeanFields(DefBean bean, JsonElement stream, TitleRow title)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)
            {
                list.Add(f.CType.Apply(JsonDataCreator.Ins, stream, bean.Assembly));
            }
            return list;
        }

        public override DType ParseBean(TBean type, List<Cell> cells, TitleRow title)
        {
            string dataStr = CreateString(cells, title);
            if (type.IsNullable && string.IsNullOrWhiteSpace(dataStr))
            {
                return null;
            }
            JsonElement jsonElement = CreateJsonDoc(dataStr);
            return type.Apply(JsonDataCreator.Ins, jsonElement, type.DefBean.Assembly);
        }

        public override DType ParseAbstractBean(TBean type, DefBean implType, List<Cell> cells, TitleRow title)
        {
            string dataStr = CreateString(cells, title);
            if (type.IsNullable && string.IsNullOrWhiteSpace(dataStr))
            {
                return null;
            }
            JsonElement jsonElement = CreateJsonDoc(dataStr);
            return new DBean(type, implType, CreateBeanFields(implType, jsonElement, title));
        }

        public override List<DType> ParseCollectionElements(TType collectionType, TType elementType, List<Cell> cells, TitleRow title)
        {
            var eles = new List<DType>();
            string dataStr = CreateString(cells, title);
            if (string.IsNullOrWhiteSpace(dataStr))
            {
                return eles;
            }
            JsonElement jsonElement = CreateJsonDoc(dataStr);
            if (jsonElement.ValueKind != JsonValueKind.Array)
            {
                throw new Exception($"json array expected, but {jsonElement.ValueKind} found");
            }
            foreach (JsonElement ele in jsonElement.EnumerateArray())
            {
                eles.Add(elementType.Apply(JsonDataCreator.Ins, ele, GenerationContext.Current.Assembly));
            }
            return eles;
        }

        public override DMap ParseMap(TMap type, List<Cell> cells, TitleRow title)
        {
            string dataStr = CreateString(cells, title);
            JsonElement jsonElement = CreateJsonDoc(dataStr);
            return (DMap)type.Apply(JsonDataCreator.Ins, jsonElement, GenerationContext.Current.Assembly);
        }

        public override KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }
    }
}
