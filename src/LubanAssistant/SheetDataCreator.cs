using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LubanAssistant
{
    class SheetDataCreator : ITypeFuncVisitor<Title, DefField, DType>
    {
        private readonly Worksheet _sheet;
        private readonly int _rowIndex;
        private readonly DefAssembly _defAss;

        public SheetDataCreator(Worksheet sheet, int rowIndex, DefAssembly ass)
        {
            _sheet = sheet;
            _rowIndex = rowIndex;
            _defAss = ass;
        }


        private ExcelStream CreateStream(Title title, bool nameMode = true)
        {
            return new ExcelStream(new Cell(title.FromIndex, title.ToIndex, _sheet.Cells[_rowIndex, title.FromIndex].Value), title.Sep, nameMode);
        }

        public DType Accept(TBool type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TByte type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TShort type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TFshort type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TInt type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TFint type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TLong type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TFlong type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TFloat type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TDouble type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TEnum type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TString type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TBytes type, Title x, DefField y)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TText type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TBean type, Title title, DefField defField)
        {
            if (title.HasSubTitle)
            {
                var originBean = (DefBean)type.Bean;
                DefBean implType;
                if (originBean.IsAbstractType)
                {
                    if (!title.SubTitles.TryGetValue(DefBean.TYPE_NAME_KEY, out var typeTitle))
                    {
                        throw new Exception($"title:{title.Name} 缺失  子title:{DefBean.TYPE_NAME_KEY}");
                    }

                    string subType = (_sheet.Cells[_rowIndex, typeTitle.FromIndex] as Range).Value.ToString().Trim();
                    if (subType.ToLower() == DefBean.BEAN_NULL_STR)
                    {
                        if (!type.IsNullable)
                        {
                            throw new Exception($"type:'{type}' 不是可空类型 '{type.Bean.FullName}?' , 不能为空");
                        }
                        return null;
                    }
                    string fullType = TypeUtil.MakeFullName(originBean.Namespace, subType);
                    implType = (DefBean)originBean.GetNotAbstractChildType(subType);
                    if (implType == null)
                    {
                        throw new Exception($"type:'{fullType}' 不是 bean 类型");
                    }


                }
                else
                {
                    implType = originBean;
                }
                var fields = new List<DType>();
                foreach (var f in implType.HierarchyFields)
                {
                    if (!title.SubTitles.TryGetValue(f.Name, out var subTitle))
                    {
                        throw new Exception($"title:{title.Name} 缺失子title:{f.Name}");
                    }
                    fields.Add(f.CType.Apply(this, subTitle, (DefField)f));
                }

                return new DBean(originBean, implType, fields);
            }
            else
            {
                return type.Apply(ExcelDataCreator.Ins, defField, CreateStream(title, false), _defAss);
            }
        }

        public DType Accept(TArray type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TList type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TSet type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TMap type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TVector2 type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TVector3 type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TVector4 type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }

        public DType Accept(TDateTime type, Title x, DefField y)
        {
            return type.Apply(ExcelDataCreator.Ins, y, CreateStream(x), _defAss);
        }


    }
}
