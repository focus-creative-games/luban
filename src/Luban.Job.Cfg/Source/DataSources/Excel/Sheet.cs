using ExcelDataReader;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{
    class Sheet
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private bool OrientRow { get; set; } = true; //  以行为数据读取方向

        private int TitleRows { get; set; } = 3; // 默认有三行是标题行. 第一行是字段名，第二行是中文描述，第三行是注释

        public string Name { get; }

        private List<List<Cell>> _rowColumns;

        private Title _rootTitle;

        private bool ExportTestData { get; }

        public class Title
        {
            public int FromIndex { get; set; }

            public int ToIndex { get; set; }

            public string Name { get; set; }

            public Dictionary<string, Title> SubTitles { get; set; } = new Dictionary<string, Title>();

            public List<Title> SubTitleList { get; set; } = new List<Title>();

            public void AddSubTitle(Title title)
            {
                if (!SubTitles.TryAdd(title.Name, title))
                {
                    throw new Exception($"标题:{title.Name} 重复");
                }
                SubTitleList.Add(title);
            }

            // 由于先处理merge再处理只占一列的标题头.
            // sub titles 未必是有序的。对于大多数数据并无影响
            // 但对于 list类型的多级标题头，有可能导致element 数据次序乱了
            public void SortSubTitles()
            {
                SubTitleList.Sort((t1, t2) => t1.FromIndex - t2.FromIndex);
                foreach (var t in SubTitleList)
                {
                    t.SortSubTitles();
                }
            }

            public override string ToString()
            {
                return $"name:{Name} [{FromIndex}, {ToIndex}] sub titles:[{string.Join(",\\n", SubTitleList)}]";
            }
        }

        public struct Cell
        {
            public Cell(int row, int column, object value)
            {
                this.Row = row;
                this.Column = column;
                this.Value = value;
            }
            public int Row { get; } // 从 1 开始

            public int Column { get; } // 从 0 开始，考虑改了它？

            public object Value { get; }


            private static string ToAlphaString(int column)
            {
                int h = column / 26;
                int n = column % 26;
                return $"{(h > 0 ? ((char)('A' + h - 1)).ToString() : "")}{(char)('A' + n)}";
            }

            public override string ToString()
            {
                return $"[{ToAlphaString(Column)}:{Row + 1}] {Value}";
            }
        }

        public class NamedRow
        {
            public Title SelfTitle { get; }

            public List<List<Cell>> Rows { get; }

            public Dictionary<string, Title> Titles => SelfTitle.SubTitles;

            public List<Title> TitleList => SelfTitle.SubTitleList;

            public NamedRow(Title selfTitle, List<Cell> row)
            {
                SelfTitle = selfTitle;
                Rows = new List<List<Cell>>() { row };
            }

            public NamedRow(Title selfTitle, List<List<Cell>> rows)
            {
                SelfTitle = selfTitle;
                Rows = rows;
            }

            public int RowCount => Rows.Count;

            private void CheckEmptySinceSecondRow(string name, int fromIndex, int toIndex)
            {
                for (int i = 1; i < Rows.Count; i++)
                {
                    var row = Rows[i];
                    if (!IsBlankRow(row, fromIndex, toIndex))
                    {
                        throw new Exception($"字段:{name} 不是多行字段,只能第一行填值. {Bright.Common.StringUtil.CollectionToString(row)}");
                    }
                }
            }

            public Title GetTitle(string name)
            {
                return Titles.TryGetValue(name, out var title) ? title : null;
            }

            public ExcelStream GetColumn(string name, string sep)
            {
                if (Titles.TryGetValue(name, out var title))
                {
                    CheckEmptySinceSecondRow(name, title.FromIndex, title.ToIndex);
                    var es = new ExcelStream(Rows[0], title.FromIndex, title.ToIndex, sep);
                    return es;
                }
                else
                {
                    throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
                }
            }

            public NamedRow GetSubTitleNamedRow(string name)
            {
                Title title = this.Titles[name];
                CheckEmptySinceSecondRow(name, title.FromIndex, title.ToIndex);
                return new NamedRow(title, this.Rows[0]);
            }

            public NamedRow GetSubTitleNamedRowOfMultiRows(string name)
            {
                Title title = Titles[name];
                return new NamedRow(title, this.Rows);
            }

            public IEnumerable<NamedRow> GenerateSubNameRows()
            {
                foreach (var row in Rows)
                {
                    if (SelfTitle != null ? IsBlankRow(row, SelfTitle.FromIndex, SelfTitle.ToIndex) : IsBlankRow(row))
                    {
                        continue;
                    }
                    yield return new NamedRow(SelfTitle, row);
                }
            }

            public IEnumerable<ExcelStream> GetColumnOfMultiRows(string name, string sep)
            {
                if (Titles.TryGetValue(name, out var title))
                {
                    foreach (var row in Rows)
                    {
                        if (IsBlankRow(row, title.FromIndex, title.ToIndex))
                        {
                            continue;
                        }
                        yield return new ExcelStream(row, title.FromIndex, title.ToIndex, sep);
                    }
                }
                else
                {
                    throw new Exception($"单元薄 缺失 列:{name}，请检查是否写错或者遗漏");
                }
            }
        }

        public Sheet(string name, bool exportTestData)
        {
            this.Name = name;
            this.ExportTestData = exportTestData;
        }

        public bool Load(IExcelDataReader reader)
        {
            //s_logger.Info("read sheet:{sheet}", reader.Name);
            if (!ParseMeta(reader))
            {
                return false;
            }

            LoadRemainRows(reader);

            return true;
        }

        private bool ParseMeta(IExcelDataReader reader)
        {
            if (!reader.Read() || reader.FieldCount == 0)
            {
                return false;
            }
            // meta 行 必须以 ##为第一个单元格内容,紧接着 key:value 形式 表达meta属性
            if (reader.GetString(0) != "##")
            {
                return false;
            }

            for (int i = 1, n = reader.FieldCount; i < n; i++)
            {
                var attr = reader.GetString(i);
                if (string.IsNullOrWhiteSpace(attr))
                {
                    continue;
                }

                var ss = attr.Split(':');
                if (ss.Length != 2)
                {
                    throw new Exception($"单元薄 meta 定义出错. attribute:{attr}");
                }
                string key = ss[0].ToLower();
                string value = ss[1];
                switch (key)
                {
                    case "row":
                    {
                        if (int.TryParse(value, out var v1))
                        {
                            OrientRow = v1 != 0;
                        }
                        else if (bool.TryParse(value, out var v2))
                        {
                            OrientRow = v2;
                        }
                        else
                        {
                            throw new Exception($"单元薄 meta 定义 row:{value} 属性值只能为true或false或0或1");
                        }
                        break;
                    }
                    case "title_rows":
                    {
                        if (!int.TryParse(value, out var v))
                        {
                            throw new Exception($"单元薄 meta 定义 title_rows:{value} 属性值只能为整数[1,10]");
                        }
                        if (v < 1 || v > 10)
                        {
                            throw new Exception($"单元薄 title_rows 应该在 [1,10] 范围内,默认是3");
                        }
                        TitleRows = v;
                        break;
                    }
                    default:
                    {
                        throw new Exception($"非法单元薄 meta 属性定义 {attr}");
                    }
                }
            }
            return true;
        }

        private bool NotExport(List<Cell> row)
        {
            if (row.Count == 0)
            {
                return true;
            }
            if (row[0].Value == null)
            {
                return false;
            }

            string exportFlag = row[0].Value.ToString().Trim().ToLower();
            switch (exportFlag)
            {
                case "false":
                case "否": return true;
                case "true":
                case "是": return false;
                case "test":
                case "测试":
                {
                    if (!ExportTestData)
                    {
                        s_logger.Debug("忽略测试数据. row:{row}", row);
                    }
                    return !ExportTestData;
                }
                default: throw new Exception($"不支持的excel 导出标记: {exportFlag}");
            }
        }

        private void InitSubTitles(Title parentTitle, List<List<Cell>> rows, CellRange[] mergeCells, int maxDepth, int depth, int fromColumn, int toColumn)
        {
            List<Cell> row = rows[depth];

            //if (row.Count > fromColumn)
            //{
            //    row = row.GetRange(fromColumn, Math.Min(row.Count, toColumn + 1) - fromColumn);
            //}


            foreach (var mergeCell in mergeCells)
            {
                if (mergeCell.FromRow == depth + 1 && mergeCell.FromColumn >= fromColumn && mergeCell.ToColumn <= toColumn)
                {
                    string subTitleName = row[mergeCell.FromColumn].Value?.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(subTitleName))
                    {
                        var newTitle = new Title() { Name = subTitleName, FromIndex = mergeCell.FromColumn, ToIndex = mergeCell.ToColumn };
                        if (depth + 1 < maxDepth)
                        {
                            InitSubTitles(newTitle, rows, mergeCells, maxDepth, depth + 1, mergeCell.FromColumn, mergeCell.ToColumn);
                        }

                        parentTitle.AddSubTitle(newTitle);
                    }
                }
            }

            for (int i = fromColumn; i <= toColumn; i++)
            {
                if (i >= row.Count)
                {
                    break;
                }

                var name = row[i].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (parentTitle.SubTitles.TryGetValue(name, out var oldTitle))
                {
                    if (oldTitle.FromIndex != i)
                    {
                        throw new Exception($"sub title 列:{name} 重复");
                    }
                    else
                    {
                        continue;
                    }
                }
                var newTitle = new Title() { Name = name, FromIndex = i, ToIndex = i };
                if (depth + 1 < maxDepth)
                {
                    InitSubTitles(newTitle, rows, mergeCells, maxDepth, depth + 1, i, i);
                }
                parentTitle.AddSubTitle(newTitle);
            }
        }


        private void LoadRemainRows(IExcelDataReader reader)
        {
            // TODO 优化性能
            // 几个思路
            // 1. 没有 title 的列不加载
            // 2. 空行优先跳过
            // 3. 跳过null或者empty的单元格
            var rows = new List<List<Cell>>();
            int rowIndex = 0;
            while (reader.Read())
            {
                ++rowIndex; // 第一行是 meta ，跳过
                var row = new List<Cell>();
                for (int i = 0, n = reader.FieldCount; i < n; i++)
                {
                    row.Add(new Cell(rowIndex, i, reader.GetValue(i)));
                }
                rows.Add(row);
            }

            if (OrientRow)
            {
                this._rowColumns = rows;
            }
            else
            {
                // 转置这个行列
                int maxColumn = rows.Select(r => r.Count).Max();
                this._rowColumns = new List<List<Cell>>();
                for (int i = 0; i < maxColumn; i++)
                {
                    var row = new List<Cell>();
                    for (int j = 0; j < rows.Count; j++)
                    {
                        row.Add(i < rows[i].Count ? rows[j][i] : new Cell(j + 1, i, null));
                    }
                    this._rowColumns.Add(row);
                }
            }

            if (this._rowColumns.Count < 1)
            {
                throw new Exception($"没有定义字段名行");
            }

            _rootTitle = new Title() { Name = "_root_", FromIndex = 1, ToIndex = rows.Select(r => r.Count).Max() - 1 };

            int titleRowNum = 1;
            if (reader.MergeCells != null)
            {
                if (OrientRow)
                {
                    foreach (var mergeCell in reader.MergeCells)
                    {
                        if (mergeCell.FromRow == 1 && mergeCell.FromColumn == 0 && mergeCell.ToColumn == 0)
                        {
                            titleRowNum = mergeCell.ToRow - mergeCell.FromRow + 1;
                        }
                    }
                }


                foreach (var mergeCell in reader.MergeCells)
                {
                    if (OrientRow)
                    {
                        //if (mergeCell.FromRow <= 1 && mergeCell.ToRow >= 1)
                        if (mergeCell.FromRow == 1)
                        {
                            // 标题 行
                            titleRowNum = Math.Max(titleRowNum, mergeCell.ToRow - mergeCell.FromRow + 1);
                            var titleName = _rowColumns[0][mergeCell.FromColumn].Value?.ToString()?.Trim();
                            if (string.IsNullOrWhiteSpace(titleName))
                            {
                                continue;
                            }

                            var newTitle = new Title() { Name = titleName, FromIndex = mergeCell.FromColumn, ToIndex = mergeCell.ToColumn };
                            if (titleRowNum > 1)
                            {
                                InitSubTitles(newTitle, rows, reader.MergeCells, titleRowNum, 1, mergeCell.FromColumn, mergeCell.ToColumn);
                            }
                            _rootTitle.AddSubTitle(newTitle);
                            //s_logger.Info("=== sheet:{sheet} title:{title}", Name, newTitle);
                        }
                    }
                    else
                    {
                        if (mergeCell.FromColumn <= 0 && mergeCell.ToColumn >= 0)
                        {
                            // 标题 行
                            var titleName = _rowColumns[0][mergeCell.FromRow - 1].Value?.ToString()?.Trim();
                            if (string.IsNullOrWhiteSpace(titleName))
                            {
                                continue;
                            }

                            _rootTitle.AddSubTitle(new Title() { Name = titleName, FromIndex = mergeCell.FromRow - 1, ToIndex = mergeCell.ToRow - 1 });
                        }
                    }

                }
            }

            //TODO 其实有bug. 未处理只占一列的 多级标题头

            // 有一些列不是MergeCell,所以还需要额外处理
            var titleRow = _rowColumns[0];
            for (int i = 0; i < titleRow.Count; i++)
            {
                var name = titleRow[i].Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                if (_rootTitle.SubTitles.TryGetValue(name, out var oldTitle))
                {
                    if (oldTitle.FromIndex != i)
                    {
                        throw new Exception($"列:{name} 重复");
                    }
                    else
                    {
                        continue;
                    }
                }
                _rootTitle.AddSubTitle(new Title() { Name = name, FromIndex = i, ToIndex = i });
            }
            if (_rootTitle.SubTitleList.Count == 0)
            {
                throw new Exception($"没有定义任何有效 列");
            }
            _rootTitle.SortSubTitles();
            foreach (var title in _rootTitle.SubTitleList)
            {
                // s_logger.Info("============ sheet:{sheet} title:{title}", Name, title);
            }

            // 删除标题行
            this._rowColumns.RemoveRange(0, Math.Min(TitleRows + titleRowNum - 1, this._rowColumns.Count));

            this._rowColumns.RemoveAll(row => NotExport(row));
        }


        public static bool IsBlankRow(List<Cell> row)
        {
            // 第一列被策划用于表示是否注释掉此行
            // 忽略此列是否空白
            return row.GetRange(1, row.Count - 1).All(c => c.Value == null || (c.Value is string s && string.IsNullOrWhiteSpace(s)));
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

        private List<Cell> GetNextRecordRow()
        {
            while (curReadIndex < _rowColumns.Count)
            {
                var row = _rowColumns[curReadIndex++];
                if (IsBlankRow(row))
                {
                    continue;
                }

                return row;
            }
            return null;
        }

        private bool HasNotMainKey(List<Cell> row)
        {
            return string.IsNullOrWhiteSpace(row[1].Value?.ToString());
        }

        private List<List<Cell>> GetNextRecordRows()
        {
            List<List<Cell>> rows = null;
            while (curReadIndex < _rowColumns.Count)
            {
                var row = _rowColumns[curReadIndex++];
                if (IsBlankRow(row))
                {
                    continue;
                }

                if (rows == null)
                {
                    rows = new List<List<Cell>>() { row };
                }
                else
                {
                    if (HasNotMainKey(row))
                    {
                        rows.Add(row);
                    }
                    else
                    {
                        --curReadIndex;
                        return rows;
                    }
                }
            }
            return rows;
        }



        public List<DType> ReadMulti(TBean type, bool enableMultiRowRecord)
        {
            var datas = new List<DType>();

            for (DType data; (data = ReadOne(type, enableMultiRowRecord)) != null;)
            {
                datas.Add(data);
            }
            return datas;
        }

        private int curReadIndex = 0;
        public DType ReadOne(TBean type, bool enableMultiRowRecord)
        {
            if (!enableMultiRowRecord)
            {
                List<Cell> row = GetNextRecordRow();
                if (row == null)
                {
                    return null;
                }
                return ExcelNamedRowDataCreator.Ins.ReadExcel(new NamedRow(_rootTitle, row), type);
            }
            else
            {
                List<List<Cell>> rows = GetNextRecordRows();
                if (rows == null)
                {
                    return null;
                }
                return ExcelNamedRowDataCreator.Ins.ReadExcel(new NamedRow(_rootTitle, rows), type);
            }
        }
    }
}
