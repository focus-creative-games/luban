using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public interface IDataParser
{
    DType ParseAny(TType type, List<Cell> cells, TitleRow title);

    DBean ParseBean(TBean type, List<Cell> cells, TitleRow title);

    List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title);

    DMap ParseMap(TMap type, List<Cell> cells, TitleRow title);

    KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title);
}

public abstract class DataParserBase : IDataParser
{
    public abstract DType ParseAny(TType type, List<Cell> cells, TitleRow title);
    public abstract DBean ParseBean(TBean type, List<Cell> cells, TitleRow title);
    public abstract List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title);
    public abstract DMap ParseMap(TMap type, List<Cell> cells, TitleRow title);
    public abstract KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title);
}
