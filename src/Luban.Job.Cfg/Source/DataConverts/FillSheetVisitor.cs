using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataConverts
{
    public class FillSheetVisitor : IDataFuncVisitor<Title, int>
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

        public int Accept(DBool type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DByte type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DShort type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DFshort type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DInt type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DFint type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DLong type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DFlong type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DFloat type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DDouble type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DEnum type, Title x)
        {
            SetTitleValue(x, type.StrValue);
            return 1;
        }

        public int Accept(DString type, Title x)
        {
            SetTitleValue(x, type.Value);
            return 1;
        }

        public int Accept(DBytes type, Title x)
        {
            throw new NotImplementedException();
        }

        public int Accept(DText type, Title x)
        {
            //if (x.FromIndex == x.ToIndex)
            //{
            //    throw new Exception($"title:{x.Name}为text类型，至少要占两列");
            //}
            SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
            //(_cells[_startRowIndex, x.FromIndex + 1] as Range).Value = type.RawValue;
            return 1;
        }

        public int Accept(DBean type, Title x)
        {

            if (x.SubTitleList.Count > 0)
            {
                if (type.Type.IsAbstractType)
                {
                    if (!x.SubTitles.TryGetValue(DefBean.TYPE_NAME_KEY, out var typeTitle))
                    {
                        throw new Exception($"多态bean:{type.Type.FullName} 缺失 __type__ 标题列");
                    }
                    if (type.ImplType != null)
                    {
                        SetTitleValue(typeTitle, type.ImplType.Name);
                    }
                    else
                    {
                        SetTitleValue(typeTitle, DefBean.BEAN_NULL_STR);
                    }
                }
                else
                {
                    if (type.ImplType != null)
                    {

                    }
                    else
                    {
                        //Current(x).Value = "null";
                        throw new Exception($"title:{x.Name} 不支持 值为null的普通bean");
                    }
                }
                int rowCount = 1;
                if (type.ImplType != null)
                {
                    int index = 0;
                    foreach (var field in type.ImplType.HierarchyFields)
                    {
                        var data = type.Fields[index++];
                        if (!x.SubTitles.TryGetValue(field.Name, out var fieldTitle))
                        {
                            throw new Exception($"title:{x.Name} 子title:{field.Name} 缺失");
                        }

                        if (data != null)
                        {
                            //if (fieldTitle.SubTitleList.Count > 0)
                            //{
                            rowCount = Math.Max(rowCount, data.Apply(this, fieldTitle));
                            //}
                            //else
                            //{

                            //    (_cells[_startRowIndex, fieldTitle.FromIndex] as Range).Value = data.Apply(ToExcelStringVisitor.Ins, fieldTitle.Sep);
                            //}
                        }
                    }
                }
                return rowCount;
            }
            else
            {
                SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
                return 1;
            }
        }

        public int Accept(DArray type, Title x)
        {
            if (x.SelfMultiRows)
            {
                int oldStartRow = _startRowIndex;
                int totalRow = 0;
                try
                {
                    foreach (var ele in type.Datas)
                    {
                        totalRow += ele.Apply(this, x);
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
                SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
                return 1;
            }
        }

        public int Accept(DList type, Title x)
        {
            if (x.SelfMultiRows)
            {
                int oldStartRow = _startRowIndex;
                int totalRow = 0;
                try
                {
                    foreach (var ele in type.Datas)
                    {
                        totalRow += ele.Apply(this, x);
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
                SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
                return 1;
            }
        }

        public int Accept(DSet type, Title x)
        {
            SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
            return 1;
        }

        public int Accept(DMap type, Title x)
        {
            SetTitleValue(x, type.Apply(ToExcelStringVisitor.Ins, x.Sep));
            return 1;
        }

        public int Accept(DVector2 type, Title x)
        {
            var v = type.Value;
            SetTitleValue(x, $"{v.X}, {v.Y}");
            return 1;
        }

        public int Accept(DVector3 type, Title x)
        {
            var v = type.Value;
            SetTitleValue(x, $"{v.X}, {v.Y}, {v.Z}");
            return 1;
        }

        public int Accept(DVector4 type, Title x)
        {
            var v = type.Value;
            SetTitleValue(x, $"{v.X}, {v.Y}, {v.Z}, {v.W}");
            return 1;
        }

        public int Accept(DDateTime type, Title x)
        {
            SetTitleValue(x, type.Time);
            return 1;
        }
    }
}
