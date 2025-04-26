using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public abstract class TextParserBase : DataParserBase
{
    protected string CreateString(List<Cell> cells, TitleRow title)
    {
        Title selfTitle = title.SelfTitle;
        var sb = new StringBuilder();
        for (int i = selfTitle.FromIndex; i <= selfTitle.ToIndex; i++)
        {
            Cell cell = cells[i];
            if (cell.Value == null)
            {
                continue;
            }
            string s = cell.Value.ToString();
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }
            sb.Append(s);
        }
        return sb.ToString();
    }

    protected List<string> CreateStrings(List<Cell> cells, TitleRow title)
    {
        Title selfTitle = title.SelfTitle;
        var ss = new List<string>();
        for (int i = selfTitle.FromIndex; i <= selfTitle.ToIndex; i++)
        {
            Cell cell = cells[i];
            if (cell.Value == null)
            {
                continue;
            }
            string s = cell.Value.ToString();
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }
            ss.Add(s);
        }
        return ss;
    }

    public override DType ParseAny(TType type, List<Cell> cells, TitleRow title)
    {
        return type switch
        {
            TBean beanType => ParseBean(beanType, cells, title),
            TArray arrType => new DArray(arrType, ParseCollectionElements(arrType, cells, title)),
            TList listType => new DList(listType, ParseCollectionElements(listType, cells, title)),
            TSet setType => new DSet(setType, ParseCollectionElements(setType, cells, title)),
            TMap mapType => ParseMap(mapType, cells, title),
            _ => type.Apply(StringDataCreator.Ins, CreateString(cells, title)),
        };
    }
}

public abstract class TextParserBase<T> : TextParserBase
{
    protected abstract T CreateRawData(string dataStr);

    protected abstract DBean ParseBean(TBean type, T rawData, TitleRow title);

    protected abstract DType ParseCollection(TType collectionType, T rawData, TitleRow title);

    protected abstract DMap ParseMap(TMap type, T rawData, TitleRow title);

    protected abstract KeyValuePair<DType, DType> ParseMapEntry(TMap type, T rawData, TitleRow title);


    public override DBean ParseBean(TBean type, List<Cell> cells, TitleRow title)
    {
        string dataStr = CreateString(cells, title);
        if (type.IsNullable && string.IsNullOrWhiteSpace(dataStr))
        {
            return null;
        }
        T rawData = CreateRawData(dataStr);
        return ParseBean(type, rawData, title);
    }

    public override List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title)
    {
        string dataStr = CreateString(cells, title);
        if (string.IsNullOrWhiteSpace(dataStr))
        {
            return new List<DType>();
        }
        T rawData = CreateRawData(dataStr);
        return ParseCollection(collectionType, rawData, title).Datas;
    }

    public override DMap ParseMap(TMap type, List<Cell> cells, TitleRow title)
    {
        string dataStr = CreateString(cells, title);
        if (string.IsNullOrWhiteSpace(dataStr))
        {
            return new DMap(type, new Dictionary<DType, DType>());
        }
        T rawData = CreateRawData(dataStr);
        return ParseMap(type, rawData, title);
    }

    public override KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title)
    {
        string dataStr = CreateString(cells, title);
        T rawData = CreateRawData(dataStr);
        return ParseMapEntry(type, rawData, title);
    }
}
