using Bright.Collections;
using ExcelDataReader;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{

    class Sheet
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; }

        public string RawUrl { get; }

        public List<TitleRow> Rows { get; } = new();

        public Sheet(string rawUrl, string name)
        {
            this.RawUrl = rawUrl;
            this.Name = name;
        }

        public void Load(RawSheet rawSheet)
        {
            bool anyMultiRows = rawSheet.Title.SubTitleList.Any(t => t.SelfMultiRows);

            var cells = rawSheet.Cells;
            Title title = rawSheet.Title;

            if (!anyMultiRows)
            {
                foreach (var row in cells)
                {
                    if (IsBlankRow(row, title.FromIndex, title.ToIndex))
                    {
                        continue;
                    }
                    Rows.Add(ParseOneLineTitleRow(title, row));
                }
            }
            else
            {
                foreach (var oneRecordRows in SplitRows(title, cells))
                {
                    Rows.Add(ParseMultiLineTitleRow(title, oneRecordRows));
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
                    if (title.SubTitleList.All(t => !t.SelfMultiRows && IsBlankRow(row, t.FromIndex, t.ToIndex)))
                    {
                        oneRecordRows.Add(row);
                    }
                    else
                    {
                        yield return oneRecordRows;
                        oneRecordRows = null;
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
                if (title.SelfMultiRows)
                {
                    var eles = new List<TitleRow>();
                    foreach (var eleRow in SplitRows(title, rows))
                    {
                        var fields = new Dictionary<string, TitleRow>();
                        foreach (var subTitle in title.SubTitleList)
                        {
                            if (subTitle.SelfMultiRows)
                            {
                                fields.Add(subTitle.Name, ParseMultiLineTitleRow(title, eleRow));
                            }
                            else
                            {
                                fields.Add(subTitle.Name, ParseOneLineTitleRow(title, eleRow[0]));
                            }
                        }
                        eles.Add(new TitleRow(title, fields));
                    }
                    return new TitleRow(title, eles);
                }
                else
                {
                    var fields = new Dictionary<string, TitleRow>();
                    foreach (var subTitle in title.SubTitleList)
                    {
                        if (subTitle.SelfMultiRows)
                        {
                            fields.Add(subTitle.Name, ParseMultiLineTitleRow(title, rows));
                        }
                        else
                        {
                            fields.Add(subTitle.Name, ParseOneLineTitleRow(title, rows[0]));
                        }
                    }
                    return new TitleRow(title, fields);
                }
            }
        }

        public IEnumerable<TitleRow> GetRows()
        {
            return Rows;
        }

        private static bool IsBlankRow(List<Cell> row, int fromIndex, int toIndex)
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

        private static bool IsSameRow(List<Cell> row1, List<Cell> row2, int fromIndex, int toIndex)
        {
            if (row2.Count < toIndex - 1)
            {
                return false;
            }
            for (int i = Math.Max(1, fromIndex), n = Math.Min(toIndex, row1.Count - 1); i <= n; i++)
            {
                var v1 = row1[i].Value;
                var v2 = row2[i].Value;
                if (v1 != v2)
                {
                    if (v1 == null)
                    {
                        if (!(v2 is string s && string.IsNullOrWhiteSpace(s)))
                        {
                            return false;
                        }
                    }
                    else if (v2 == null)
                    {
                        if (!(v1 is string s && string.IsNullOrWhiteSpace(s)))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return v1.ToString() == v2.ToString();
                    }
                }
            }
            return true;
        }
    }
}
