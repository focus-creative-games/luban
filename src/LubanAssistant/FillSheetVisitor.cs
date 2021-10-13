using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LubanAssistant
{
    class FillSheetVisitor : IDataFuncVisitor<Title, int>
    {
        private readonly Worksheet _sheet;

        private readonly Range _cells;

        private readonly int _startRowIndex;

        public FillSheetVisitor(Worksheet sheet, int startRowIndex)
        {
            _sheet = sheet;
            _cells = sheet.Cells;
            _startRowIndex = startRowIndex;
        }

        public int Accept(DBool type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DByte type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DShort type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DFshort type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DInt type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DFint type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DLong type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DFlong type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DFloat type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DDouble type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
            return 1;
        }

        public int Accept(DEnum type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.StrValue;
            return 1;
        }

        public int Accept(DString type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Value;
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
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
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
                        (_cells[_startRowIndex, typeTitle.FromIndex] as Range).Value = type.ImplType.Name;
                    }
                    else
                    {
                        (_cells[_startRowIndex, typeTitle.FromIndex] as Range).Value = DefBean.BEAN_NULL_STR;
                    }
                }
                else
                {
                    if (type.ImplType != null)
                    {

                    }
                    else
                    {
                        //(_cells[_startRowIndex, x.FromIndex] as Range).Value = "null";
                        throw new Exception($"title:{x.Name} 不支持 值为null的普通bean");
                    }
                }
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
                            data.Apply(this, fieldTitle);
                            //}
                            //else
                            //{

                            //    (_cells[_startRowIndex, fieldTitle.FromIndex] as Range).Value = data.Apply(ToExcelStringVisitor.Ins, fieldTitle.Sep);
                            //}
                        }
                    }
                }
            }
            else
            {
                (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
            }
            return 1;
        }

        public int Accept(DArray type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
            return 1;
        }

        public int Accept(DList type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
            return 1;
        }

        public int Accept(DSet type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
            return 1;
        }

        public int Accept(DMap type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Apply(ToExcelStringVisitor.Ins, x.Sep);
            return 1;
        }

        public int Accept(DVector2 type, Title x)
        {
            var v = type.Value;
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = $"{v.X},{v.Y}";
            return 1;
        }

        public int Accept(DVector3 type, Title x)
        {
            var v = type.Value;
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = $"{v.X},{v.Y},{v.Z}";
            return 1;
        }

        public int Accept(DVector4 type, Title x)
        {
            var v = type.Value;
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = $"{v.X},{v.Y},{v.Z},{v.W}";
            return 1;
        }

        public int Accept(DDateTime type, Title x)
        {
            (_cells[_startRowIndex, x.FromIndex] as Range).Value = type.Time;
            return 1;
        }
    }
}
