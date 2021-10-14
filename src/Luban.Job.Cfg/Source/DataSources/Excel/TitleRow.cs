using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class TitleRow
    {
        public List<string> Tags { get; }


        //public static IEnumerable<TitleRow> CreateMultiRowNamedRow(List<List<Cell>> rows, Title title, TBean bean)
        //{
        //    if (!((DefBean)bean.Bean).IsMultiRow)
        //    {
        //        foreach (var row in rows)
        //        {
        //            if (Sheet.IsBlankRow(row, title.FromIndex, title.ToIndex))
        //            {
        //                continue;
        //            }
        //            yield return new TitleRow(title, row);
        //        }
        //    }
        //    else
        //    {
        //        List<DefField> notMultiRowFields = bean.Bean.HierarchyFields.Select(f => (DefField)f).Where(f => !f.IsMultiRow && f.IsRowOrient).ToList();
        //        List<List<Cell>> recordRows = null;
        //        foreach (var row in rows)
        //        {
        //            // 忽略全空的行
        //            if (Sheet.IsBlankRow(row, title.FromIndex, title.ToIndex))
        //            {
        //                continue;
        //            }
        //            // 如果非多行数据全空，或者跟记录第一行完全相同说明该行属于多行数据
        //            if (notMultiRowFields.All(f =>
        //            {
        //                var fieldTitle = title.SubTitles[f.Name];
        //                return Sheet.IsBlankRow(row, fieldTitle.FromIndex, fieldTitle.ToIndex);
        //            }) || (title.Root && recordRows != null && notMultiRowFields.All(f =>
        //            {
        //                var fieldTitle = title.SubTitles[f.Name];
        //                return Sheet.IsSameRow(row, recordRows[0], fieldTitle.FromIndex, fieldTitle.ToIndex);
        //            })))
        //            {
        //                if (recordRows == null)
        //                {
        //                    recordRows = new List<List<Cell>>();
        //                }
        //                recordRows.Add(row);
        //            }
        //            else
        //            {
        //                if (recordRows != null)
        //                {
        //                    yield return new TitleRow(title, recordRows);
        //                }
        //                recordRows = new List<List<Cell>>();
        //                recordRows.Add(row);
        //            }
        //        }
        //        if (recordRows != null)
        //        {
        //            yield return new TitleRow(title, recordRows);
        //        }
        //    }
        //}

        public Title SelfTitle { get; }

        public Cell Current => Row[0];

        public List<Cell> Row { get; }

        public List<List<Cell>> Rows { get; }

        public Dictionary<string, TitleRow> Fields { get; }

        public List<TitleRow> Elements { get; }

        public ExcelStream AsStream(string sep) => new ExcelStream(Row, 0, Row.Count - 1, sep);

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

        //private void CheckEmptySinceSecondRow(string name, int fromIndex, int toIndex)
        //{
        //    for (int i = 1; i < Rows.Count; i++)
        //    {
        //        var row = Rows[i];
        //        if (!IsBlankRow(row, fromIndex, toIndex))
        //        {
        //            throw new Exception($"字段:{name} 不是多行字段,只能第一行填值. {Bright.Common.StringUtil.CollectionToString(row)}");
        //        }
        //    }
        //}

        public Title GetTitle(string name)
        {
            return SelfTitle.SubTitles.TryGetValue(name, out var title) ? title : null;
        }

        //public ExcelStream GetColumn(string name)
        //{
        //    var field = GetSubTitleNamedRow(name);
        //    if (field != null)
        //    {
        //        return field.AsStream;
        //    }
        //    else
        //    {
        //        throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
        //    }
        //    //if (Titles.TryGetValue(name, out var title))
        //    //{
        //    //    // 只有顶级root支持才允许非multi_rows字段与第一行相同时，判定为同个记录
        //    //    if (!this.SelfTitle.Root)
        //    //    {
        //    //        CheckEmptySinceSecondRow(name, title.FromIndex, title.ToIndex);
        //    //    }
        //    //    var es = new ExcelStream(Rows[0], title.FromIndex, title.ToIndex, sep, namedMode);
        //    //    return es;
        //    //}
        //    //else
        //    //{
        //    //    throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
        //    //}
        //}

        public TitleRow GetSubTitleNamedRow(string name)
        {
            //Title title = Titles[name];
            //return new TitleRow(title, this.Rows);
            return Fields.TryGetValue(name, out var r) ? r : null;
        }

        //public IEnumerable<TitleRow> GenerateSubNameRows(TBean bean)
        //{
        //    foreach (var row in Rows)
        //    {
        //        if (SelfTitle != null ? IsBlankRow(row, SelfTitle.FromIndex, SelfTitle.ToIndex) : IsBlankRow(row))
        //        {
        //            continue;
        //        }
        //        yield return new TitleRow(SelfTitle, row);
        //    }
        //}

        public IEnumerable<ExcelStream> GetColumnOfMultiRows(string name, string sep)
        {
            foreach (var ele in GetSubTitleNamedRow(name).Elements)
            {
                yield return ele.AsStream(sep);
            }
            //if (Titles.TryGetValue(name, out var title))
            //{
            //    if (isRowOrient)
            //    {
            //        foreach (var row in Rows)
            //        {
            //            if (IsBlankRow(row, title.FromIndex, title.ToIndex))
            //            {
            //                continue;
            //            }
            //            yield return new ExcelStream(row, title.FromIndex, title.ToIndex, sep, false);
            //        }
            //    }
            //    else
            //    {
            //        for (int i = title.FromIndex; i <= title.ToIndex; i++)
            //        {
            //            if (!IsBlankColumn(Rows, i))
            //            {
            //                var cells = Rows.Where(r => r.Count > i).Select(r => r[i]).Where(v => !(v.Value == null || (v.Value is string s && string.IsNullOrEmpty(s)))).ToList();
            //                yield return new ExcelStream(cells, 0, cells.Count - 1, sep, false);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
            //}
        }


        public IEnumerable<ExcelStream> AsMultiRowStream(string sep)
        {
            //if (Titles.TryGetValue(name, out var title))
            //{
            //    if (isRowOrient)
            //    {
            //        var totalCells = Rows.SelectMany(r => r.GetRange(title.FromIndex, title.ToIndex - title.FromIndex + 1))
            //            .Where(c => c.Value != null && !(c.Value is string s && string.IsNullOrWhiteSpace(s))).ToList();
            //        return new ExcelStream(totalCells, 0, totalCells.Count - 1, sep, false);
            //    }
            //    else
            //    {
            //        throw new NotSupportedException($"bean类型多行数据不支持纵向填写");
            //    }
            //}
            //else
            //{
            //    throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
            //}
            throw new NotSupportedException();
        }
    }
}
