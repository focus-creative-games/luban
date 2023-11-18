namespace Luban.DataLoader.Builtin.Excel;

class TitleRow
{
    public List<string> Tags { get; }

    public Title SelfTitle { get; }

    public object Current
    {
        get
        {
            if (Row != null)
            {
                var v = Row[SelfTitle.FromIndex].Value;
                if (v == null || (v is string s && string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(SelfTitle.Default)))
                {
                    return SelfTitle.Default;
                }
                else
                {
                    return v;
                }
            }
            else
            {
                throw new Exception($"简单数据类型字段 不支持子列名或者多行");
            }
        }
    }

    public List<Cell> Row { get; }

    public int CellCount => SelfTitle.ToIndex - SelfTitle.FromIndex + 1;

    public List<List<Cell>> Rows { get; }

    public Dictionary<string, TitleRow> Fields { get; }

    public List<TitleRow> Elements { get; }

    public string Location
    {
        get
        {
            if (Row != null)
            {
                return Row[SelfTitle.FromIndex].ToString();
            }
            if (Rows != null)
            {
                return Rows[0][SelfTitle.FromIndex].ToString();
            }
            if (Fields != null)
            {
                return Fields[SelfTitle.SubTitleList[0].Name].Location;
            }
            if (Elements != null)
            {
                return Elements.Count > 0 ? Elements[0].Location : "无法定位";
            }
            return "无法定位";
        }
    }

    public bool IsBlank
    {
        get
        {
            if (Row != null)
            {
                return RowColumnSheet.IsBlankRow(Row, SelfTitle.FromIndex, SelfTitle.ToIndex);
            }
            if (Rows != null)
            {
                return RowColumnSheet.IsBlankRow(Rows[0], SelfTitle.FromIndex, SelfTitle.ToIndex);
            }
            if (Fields != null)
            {
                return Fields.Values.All(f => f.IsBlank);
            }
            if (Elements != null)
            {
                return Elements.All(e => e.IsBlank);
            }
            throw new Exception();
        }
    }

    public ExcelStream AsStream(string sep)
    {
        if (string.IsNullOrEmpty(sep))
        {
            return new ExcelStream(Row, SelfTitle.FromIndex, SelfTitle.ToIndex, "", SelfTitle.Default);
        }
        else
        {
            return new ExcelStream(Row, SelfTitle.FromIndex, SelfTitle.ToIndex, sep, SelfTitle.Default);
        }
    }

    public bool HasSubFields => Fields != null || Elements != null;

    public TitleRow(Title selfTitle, List<Cell> row)
    {
        SelfTitle = selfTitle;
        Row = row;
    }

    public TitleRow(Title selfTitle, List<List<Cell>> rows)
    {
        SelfTitle = selfTitle;
        Rows = rows;
    }

    public TitleRow(Title selfTitle, Dictionary<string, TitleRow> fields)
    {
        SelfTitle = selfTitle;
        Fields = fields;
    }

    public TitleRow(Title selfTitle, List<TitleRow> elements)
    {
        SelfTitle = selfTitle;
        Elements = elements;
    }

    public int RowCount => Rows.Count;

    public Title GetTitle(string name)
    {
        return SelfTitle.SubTitles.TryGetValue(name, out var title) ? title : null;
    }


    public TitleRow GetSubTitleNamedRow(string name)
    {
        //Title title = Titles[name];
        //return new TitleRow(title, this.Rows);
        return Fields.TryGetValue(name, out var r) ? r : null;
    }

    public IEnumerable<ExcelStream> GetColumnOfMultiRows(string name, string sep)
    {
        foreach (var ele in GetSubTitleNamedRow(name).Elements)
        {
            yield return ele.AsStream(sep);
        }
    }


    public IEnumerable<ExcelStream> AsMultiRowStream(string sep)
    {
        throw new NotSupportedException();
    }

    public ExcelStream AsMultiRowConcatStream(string sep)
    {
        return new ExcelStream(Rows, SelfTitle.FromIndex, SelfTitle.ToIndex, sep, SelfTitle.Default);
    }

    public ExcelStream AsMultiRowConcatElements(string sep)
    {
        return new ExcelStream(Elements.Select(e => e.Row).ToList(), SelfTitle.FromIndex, SelfTitle.ToIndex, sep, SelfTitle.Default);
    }
}
