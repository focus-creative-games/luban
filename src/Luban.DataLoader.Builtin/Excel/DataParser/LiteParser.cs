using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Excel.DataParser
{
    public class LiteDataCreate
    {

    }

    public class LiteParser : TextParserBase<LiteDataCreate>
    {
        protected override LiteDataCreate CreateRawData(string dataStr)
        {
            throw new NotImplementedException();
        }

        protected override DBean ParseBean(TBean type, LiteDataCreate rawData, TitleRow title)
        {
            throw new NotImplementedException();
        }

        protected override DType ParseCollection(TType collectionType, LiteDataCreate rawData, TitleRow title)
        {
            throw new NotImplementedException();
        }

        protected override DMap ParseMap(TMap type, LiteDataCreate rawData, TitleRow title)
        {
            throw new NotImplementedException();
        }

        protected override KeyValuePair<DType, DType> ParseMapEntry(TMap type, LiteDataCreate rawData, TitleRow title)
        {
            throw new NotImplementedException();
        }
    }
}
