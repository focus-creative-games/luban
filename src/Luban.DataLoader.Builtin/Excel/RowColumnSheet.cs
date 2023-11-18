namespace Luban.DataLoader.Builtin.Excel;

class RowColumnSheet
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public string Name { get; }

    public string RawUrl { get; }

    public List<(string Tag, TitleRow Row)> Rows { get; } = new();

    public RowColumnSheet(string rawUrl, string name)
    {
        this.RawUrl = rawUrl;
        this.Name = name;
    }

    private string GetRowTag(List<Cell> row)
    {
        return row.Count > 0 ? row[0].Value?.ToString() ?? "" : "";
    }

    public void Load(RawSheet rawSheet)
    {
        var cells = rawSheet.Cells;
        Title title = rawSheet.Title;

        if (!title.HierarchyMultiRows)
        {
            foreach (var row in cells)
            {
                if (IsBlankRow(row, title))
                {
                    continue;
                }
                Rows.Add((GetRowTag(row), ParseOneLineTitleRow(title, row)));
            }
        }
        else
        {
            foreach (var oneRecordRows in SplitRows(title, cells))
            {
                Rows.Add((GetRowTag(oneRecordRows[0]), ParseMultiLineTitleRow(title, oneRecordRows)));
            }
        }
    }

    private TitleRow ParseOneLineTitleRow(Title title, List<Cell> row)
    {
        if (title.SubTitleList.Count == 0)
        {
            return new TitleRow(title, row);
        }

        Dictionary<string, TitleRow> fields = new();

        foreach (var subTitle in title.SubTitleList)
        {
            fields.Add(subTitle.Name, ParseOneLineTitleRow(subTitle, row));
        }
        return new TitleRow(title, fields);
    }

    private static bool IsMultiRowsExtendRow(List<Cell> row, Title title)
    {
        if (title.HasSubTitle)
        {
            foreach (var t in title.SubTitleList)
            {
                if (!t.SelfMultiRows && !IsMultiRowsExtendRow(row, t))
                {
                    return false;
                }
            }
            //return title.SubTitleList.All(t => t.SelfMultiRows || IsMultiRowsExtendRow(row, t));
            return true;
        }
        else
        {
            return IsBlankRow(row, title.FromIndex, title.ToIndex);
        }
    }

    private IEnumerable<List<List<Cell>>> SplitRows(Title title, List<List<Cell>> rows)
    {
        List<List<Cell>> oneRecordRows = null;
        foreach (var row in rows)
        {
            if (IsBlankRow(row, title.FromIndex, title.ToIndex))
            {
                continue;
            }
            if (oneRecordRows == null)
            {
                oneRecordRows = new List<List<Cell>>() { row };
            }
            else
            {
                if (IsMultiRowsExtendRow(row, title))
                {
                    oneRecordRows.Add(row);
                }
                else
                {
                    yield return oneRecordRows;
                    oneRecordRows = new List<List<Cell>>() { row };
                }
            }
        }
        if (oneRecordRows != null)
        {
            yield return oneRecordRows;
        }
    }

    private TitleRow ParseMultiLineTitleRow(Title title, List<List<Cell>> rows)
    {
        if (title.SubTitleList.Count == 0)
        {
            if (title.SelfMultiRows)
            {
                return new TitleRow(title, rows);
            }
            else
            {
                return new TitleRow(title, rows[0]);
            }
        }
        else
        {
            var fields = new Dictionary<string, TitleRow>();
            foreach (var subTitle in title.SubTitleList)
            {
                if (subTitle.SelfMultiRows)
                {
                    var eles = new List<TitleRow>();
                    if (subTitle.SubHierarchyMultiRows)
                    {
                        foreach (var eleRows in SplitRows(subTitle, rows))
                        {
                            eles.Add(ParseMultiLineTitleRow(subTitle, eleRows));
                        }
                    }
                    else
                    {
                        foreach (var eleRow in rows)
                        {
                            if (IsBlankRow(eleRow, subTitle.FromIndex, subTitle.ToIndex))
                            {
                                continue;
                            }
                            eles.Add(ParseOneLineTitleRow(subTitle, eleRow));
                        }
                    }
                    fields.Add(subTitle.Name, new TitleRow(subTitle, eles));
                }
                else
                {
                    if (subTitle.SubHierarchyMultiRows)
                    {
                        fields.Add(subTitle.Name, ParseMultiLineTitleRow(subTitle, rows));
                    }
                    else
                    {
                        fields.Add(subTitle.Name, ParseOneLineTitleRow(subTitle, rows[0]));
                    }
                }
            }
            return new TitleRow(title, fields);
        }
    }

    public IEnumerable<(string Tag, TitleRow Row)> GetRows()
    {
        return Rows;
    }

    public static bool IsBlankRow(List<Cell> row, Title title)
    {
        if (title.SubTitleList.Count == 0)
        {
            return IsBlankRow(row, title.FromIndex, title.ToIndex);
        }
        return title.SubTitleList.All(t => IsBlankRow(row, t));
    }

    public static bool IsBlankRow(List<Cell> row, int fromIndex, int toIndex)
    {
        for (int i = Math.Max(1, fromIndex), n = Math.Min(toIndex, row.Count - 1); i <= n; i++)
        {
            var v = row[i].Value;
            if (v != null && !(v is string s && string.IsNullOrEmpty(s)))
            {
                return false;
            }
        }
        return true;
    }
}
