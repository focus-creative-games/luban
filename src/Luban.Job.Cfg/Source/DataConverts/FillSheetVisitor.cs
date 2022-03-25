using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    public class FillSheetVisitor : IDataFuncVisitor<TType, Title, int>
    {

        private readonly List<object[]> _cells;

        private readonly int _columnSize;
        private int _startRowIndex;

        public FillSheetVisitor(List<object[]> cells, int columnSize, int startRowIndex)
        {
            _cells = cells;
            _columnSize = columnSize;
            _startRowIndex = startRowIndex;
        }

        void SetTitleValue(Title title, object value)
        {
            while (_cells.Count <= _startRowIndex)
            {
                var row = new object[_columnSize];
                _cells.Add(row);
            }
            _cells[_startRowIndex][title.FromIndex] = value;
        }

        public int Accept(DBool data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DByte data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DShort data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DFshort data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DInt data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DFint data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DLong data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DFlong data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DFloat data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DDouble data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DEnum data, TType type, Title x)
        {
            SetTitleValue(x, data.StrValue);
            return 1;
        }

        public int Accept(DString data, TType type, Title x)
        {
            SetTitleValue(x, data.Value);
            return 1;
        }

        public int Accept(DBytes data, TType type, Title x)
        {
            throw new NotImplementedException();
        }

        public int Accept(DText data, TType type, Title x)
        {
            //if (x.FromIndex == x.ToIndex)
            //{
            //    throw new Exception($"title:{x.Name}为text类型，至少要占两列");
            //}
            SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.OrTag("sep", "#")));
            //(_cells[_startRowIndex, x.FromIndex + 1] as Range).Value = type.RawValue;
            return 1;
        }

        public int Accept(DBean data, TType type, Title x)
        {
            if (x.SubTitleList.Count > 0)
            {
                if (data.Type.IsAbstractType)
                {
                    if (!x.SubTitles.TryGetValue(DefBean.EXCEL_TYPE_NAME_KEY, out var typeTitle) && !x.SubTitles.TryGetValue(DefBean.FALLBACK_TYPE_NAME_KEY, out typeTitle))
                    {
                        throw new Exception($"多态bean:{data.Type.FullName} 缺失 {DefBean.EXCEL_TYPE_NAME_KEY} 标题列");
                    }
                    if (data.ImplType != null)
                    {
                        SetTitleValue(typeTitle, data.ImplType.Name);
                    }
                    else
                    {
                        SetTitleValue(typeTitle, DefBean.BEAN_NULL_STR);
                    }
                }
                else
                {
                    if (data.ImplType != null)
                    {

                    }
                    else
                    {
                        //Current(x).Value = "null";
                        throw new Exception($"title:{x.Name} 不支持 值为null的普通bean");
                    }
                }
                int rowCount = 1;
                if (data.ImplType != null)
                {
                    int index = 0;
                    foreach (var field in data.ImplType.HierarchyFields)
                    {
                        var fdata = data.Fields[index++];
                        if (!x.SubTitles.TryGetValue(field.Name, out var fieldTitle))
                        {
                            throw new Exception($"title:{x.Name} 子title:{field.Name} 缺失");
                        }

                        if (fdata != null)
                        {
                            //if (fieldTitle.SubTitleList.Count > 0)
                            //{
                            rowCount = Math.Max(rowCount, fdata.Apply(this, field.CType, fieldTitle));
                            //}
                            //else
                            //{

                            //    (_cells[_startRowIndex, fieldTitle.FromIndex] as Range).Value = data.Apply(ToExcelStringVisitor.Ins, fieldTitle.Sep);
                            //}
                        }
                        else if (field.CType is TText)
                        {
                            SetTitleValue(fieldTitle, $"null{(field.CType.HasTag("sep") ? field.CType.GetTag("sep") : "#")}null");
                        }
                    }
                }
                return rowCount;
            }
            else
            {
                SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.GetTag("sep")));
                return 1;
            }
        }

        public int Accept(DArray data, TType type, Title x)
        {
            if (x.SelfMultiRows)
            {
                int oldStartRow = _startRowIndex;
                int totalRow = 0;
                try
                {
                    var elementType = data.Type.ElementType;
                    foreach (var ele in data.Datas)
                    {
                        totalRow += ele.Apply(this, elementType, x);
                        _startRowIndex = oldStartRow + totalRow;
                    }
                    return totalRow;
                }
                finally
                {
                    _startRowIndex = oldStartRow;
                }
            }
            else
            {
                SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.GetTag("sep")));
                return 1;
            }
        }

        public int Accept(DList data, TType type, Title x)
        {
            if (x.SelfMultiRows)
            {
                int oldStartRow = _startRowIndex;
                int totalRow = 0;
                try
                {
                    var elementType = data.Type.ElementType;
                    foreach (var ele in data.Datas)
                    {
                        totalRow += ele.Apply(this, elementType, x);
                        _startRowIndex = oldStartRow + totalRow;
                    }
                    return totalRow;
                }
                finally
                {
                    _startRowIndex = oldStartRow;
                }
            }
            else
            {
                SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.GetTag("sep")));
                return 1;
            }
        }

        public int Accept(DSet data, TType type, Title x)
        {
            SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.GetTag("sep")));
            return 1;
        }

        public int Accept(DMap data, TType type, Title x)
        {
            if (x.SelfMultiRows)
            {
                int oldStartRow = _startRowIndex;
                int totalRow = 0;
                try
                {
                    var elementType = data.Type.ElementType;
                    foreach (var ele in data.Datas)
                    {
                        int row = Math.Max(ele.Key.Apply(this, elementType, x), ele.Value.Apply(this, elementType, x));
                        totalRow += row;
                        _startRowIndex = oldStartRow + totalRow;
                    }
                    return totalRow;
                }
                finally
                {
                    _startRowIndex = oldStartRow;
                }
            }
            else
            {
                SetTitleValue(x, data.Apply(ToExcelStringVisitor.Ins, type.GetTag("sep")));
                return 1;
            }
        }

        public int Accept(DVector2 data, TType type, Title x)
        {
            var v = data.Value;
            SetTitleValue(x, $"{v.X}, {v.Y}");
            return 1;
        }

        public int Accept(DVector3 data, TType type, Title x)
        {
            var v = data.Value;
            SetTitleValue(x, $"{v.X},{v.Y},{v.Z}");
            return 1;
        }

        public int Accept(DVector4 data, TType type, Title x)
        {
            var v = data.Value;
            SetTitleValue(x, $"{v.X},{v.Y},{v.Z},{v.W}");
            return 1;
        }

        public int Accept(DDateTime data, TType type, Title x)
        {
            SetTitleValue(x, data.Time);
            return 1;
        }
    }
}
