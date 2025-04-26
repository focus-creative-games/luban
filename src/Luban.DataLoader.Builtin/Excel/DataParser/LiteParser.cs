using Luban.Datas;
using Luban.Defs;
using Luban.Types;

namespace Luban.DataLoader.Builtin.Excel.DataParser
{
    public class LiteParser : DataParserBase
    {
        public override DType ParseAbstractBean(TBean type, DefBean implType, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }

        public override DType ParseAny(TType type, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }

        public override DType ParseBean(TBean type, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }

        public override List<DType> ParseCollectionElements(TType collectionType, TType elementType, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }

        public override DMap ParseMap(TMap type, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }

        public override KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title)
        {
            throw new NotImplementedException();
        }
    }
}
