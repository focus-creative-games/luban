using Luban.DataLoader.Builtin.DataVisitors;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataLoader.Builtin.Excel.DataParser;

public class StreamParser : DataParserBase
{
    private ExcelStream AsStream(TitleRow row, List<Cell> cells, string sep)
    {
        Title selfTitle = row.SelfTitle;
        if (string.IsNullOrEmpty(sep))
        {
            return new ExcelStream(cells, selfTitle.FromIndex, selfTitle.ToIndex, "", selfTitle.Default);
        }
        else
        {
            return new ExcelStream(cells, selfTitle.FromIndex, selfTitle.ToIndex, sep, selfTitle.Default);
        }
    }

    public override DType ParseAny(TType type, List<Cell> cells, TitleRow title)
    {
        ExcelStream stream = AsStream(title, cells, title.SelfTitle.Sep);
        return type.Apply(ExcelStreamDataCreator.Ins, stream);
    }

    public override DBean ParseBean(TBean type, List<Cell> cells, TitleRow title)
    {
        var s = AsStream(title, cells, title.SelfTitle.Sep);
        if (type.IsNullable && s.TryReadEOF())
        {
            return null;
        }
        return (DBean)type.Apply(ExcelStreamDataCreator.Ins, s);
    }

    public override List<DType> ParseCollectionElements(TType collectionType, List<Cell> cells, TitleRow title)
    {
        return ExcelStreamDataCreator.Ins.ReadList(collectionType, collectionType.ElementType, AsStream(title, cells, title.SelfTitle.Sep));
    }

    public override DMap ParseMap(TMap type, List<Cell> cells, TitleRow title)
    {
        var s = AsStream(title, cells, title.SelfTitle.Sep);
        return (DMap)type.Apply(ExcelStreamDataCreator.Ins, s);
    }

    public override KeyValuePair<DType, DType> ParseMapEntry(TMap type, List<Cell> cells, TitleRow title)
    {
        var s = AsStream(title, cells, title.SelfTitle.Sep);
        var keyData = type.KeyType.Apply(ExcelStreamDataCreator.Ins, s);
        var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, s);
        return new KeyValuePair<DType, DType>(keyData, valueData);
    }
}
