using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataCreators
{
    class ExcelNamedRowDataCreator : ITypeFuncVisitor<Sheet.NamedRow, bool, bool, DType>
    {
        public static ExcelNamedRowDataCreator Ins { get; } = new ExcelNamedRowDataCreator();


        public DType ReadExcel(Sheet.NamedRow row, TBean btype)
        {
            return Accept(btype, row, false, false);
        }

        public DType Accept(TBool type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TByte type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TShort type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TFshort type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TInt type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TFint type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TLong type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TFlong type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TFloat type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TDouble type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TEnum type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TString type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TBytes type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TText type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        private static bool IsContainerAndElementNotSepType(TType type)
        {
            switch (type)
            {
                case TArray ta: return ta.ElementType.Apply(IsNotSepTypeVisitor.Ins);
                case TList tl: return tl.ElementType.Apply(IsNotSepTypeVisitor.Ins);
                case TSet ts: return ts.ElementType.Apply(IsNotSepTypeVisitor.Ins);
                case TMap tm: return tm.KeyType.Apply(IsNotSepTypeVisitor.Ins) && tm.ValueType.Apply(IsNotSepTypeVisitor.Ins);
                default: return false;
            }
            throw new NotImplementedException();
        }

        private List<DType> CreateBeanFields(DefBean bean, Sheet.NamedRow row)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)
            {
                string fname = f.Name;
                Sheet.Title title = row.GetTitle(fname);
                if (title == null)
                {
                    throw new Exception($"bean:'{bean.FullName}' 缺失 列:'{fname}'，请检查是否写错或者遗漏");
                }
                // 多级标题
                if (title.SubTitles.Count > 0)
                {
                    try
                    {
                        list.Add(f.CType.Apply(this, row.GetSubTitleNamedRow(fname), f.IsMultiRow, f.IsNullable));
                    }
                    catch (DataCreateException dce)
                    {
                        dce.Push(bean, f);
                        throw;
                    }
                    catch (Exception e)
                    {
                        var dce = new DataCreateException(e, $"列：{fname}");
                        dce.Push(bean, f);
                        throw dce;
                    }
                }
                else
                {
                    string sep = f.ActualSep;
                    if (string.IsNullOrWhiteSpace(sep) && IsContainerAndElementNotSepType(f.CType))
                    {
                        sep = ";,";
                    }

                    if (f.IsMultiRow)
                    {
                        try
                        {
                            if (f.CType.IsCollection)
                            {
                                list.Add(f.CType.Apply(MultiRowExcelDataCreator.Ins, row.GetColumnOfMultiRows(f.Name, sep, f.IsRowOrient), f.IsNullable, (DefAssembly)bean.AssemblyBase));
                            }
                            else
                            {
                                list.Add(f.CType.Apply(ExcelDataCreator.Ins, null, row.GetMultiRowStream(f.Name, sep, f.IsRowOrient), (DefAssembly)bean.AssemblyBase));
                            }
                        }
                        catch (DataCreateException dce)
                        {
                            dce.Push(bean, f);
                            throw;
                        }
                        catch (Exception e)
                        {
                            var dce = new DataCreateException(e, "");
                            dce.Push(bean, f);
                            throw dce;
                        }
                    }
                    else
                    {
                        ExcelStream stream = row.GetColumn(f.Name, sep, !f.CType.Apply(IsMultiData.Ins));
                        try
                        {
                            list.Add(f.CType.Apply(ExcelDataCreator.Ins, f, stream, (DefAssembly)bean.AssemblyBase));
                        }
                        catch (DataCreateException dce)
                        {
                            dce.Push(bean, f);
                            throw;
                        }
                        catch (Exception e)
                        {
                            var dce = new DataCreateException(e, stream.LastReadDataInfo);
                            dce.Push(bean, f);
                            throw dce;
                        }
                    }
                }
            }
            return list;
        }


        public DType Accept(TBean type, Sheet.NamedRow row, bool multirow, bool nullable)
        {
            var originBean = (DefBean)type.Bean;
            if (originBean.IsAbstractType)
            {
                string subType = row.GetColumn(DefBean.TYPE_NAME_KEY, null, true).Read().ToString().Trim();
                if (subType.ToLower() == DefBean.BEAN_NULL_STR)
                {
                    if (!type.IsNullable)
                    {
                        throw new Exception($"type:'{type}' 不是可空类型 '{type.Bean.FullName}?' , 不能为空");
                    }
                    return null;
                }
                string fullType = TypeUtil.MakeFullName(originBean.Namespace, subType);
                DefBean implType = (DefBean)originBean.GetNotAbstractChildType(subType);
                if (implType == null)
                {
                    throw new Exception($"type:'{fullType}' 不是 bean 类型");
                }
                return new DBean(originBean, implType, CreateBeanFields(implType, row));
            }
            else
            {
                if (type.IsNullable)
                {
                    string subType = row.GetColumn(DefBean.TYPE_NAME_KEY, null, true).Read().ToString().Trim();
                    if (subType == DefBean.BEAN_NULL_STR)
                    {
                        return null;
                    }
                    else if (subType != DefBean.BEAN_NOT_NULL_STR && subType != originBean.Name)
                    {
                        throw new Exception($"type:'{type.Bean.FullName}' 可空标识:'{subType}' 不合法（只能为{DefBean.BEAN_NOT_NULL_STR}或{DefBean.BEAN_NULL_STR}或{originBean.Name})");
                    }
                }

                return new DBean(originBean, originBean, CreateBeanFields(originBean, row));
            }
        }


        private List<DType> ReadList(TBean elementType, Sheet.NamedRow row, bool multirow)
        {
            var list = new List<DType>();
            // 如果是多行数据，以当前title为title,每行读入一个element
            if (multirow)
            {
                //foreach (var sub in row.GenerateSubNameRows(elementType))
                foreach (var sub in Sheet.NamedRow.CreateMultiRowNamedRow(row.Rows, row.SelfTitle, elementType))
                {
                    list.Add(this.Accept(elementType, sub, false, false));
                }
            }
            else
            {
                // 如果不是多行，并且定义了子标题的话。以一个子标题所占的列，读入一个数据

                //foreach (var sub in row.SelfTitle.SubTitleList)
                //{
                //    list.Add(this.Accept(elementType, new Sheet.NamedRow(sub, row.Rows), false, false));
                //}
                throw new NotSupportedException("只有multi_rows=1的list,bean类型才允许有子title");
            }
            return list;
        }

        public DType Accept(TArray type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            if (type.ElementType is not TBean bean)
            {
                throw new Exception($"NamedRow 只支持 bean 类型的容器");
            }
            else
            {
                return new DArray(type, ReadList(bean, x, multirow));
            }
        }

        public DType Accept(TList type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            if (type.ElementType is not TBean bean)
            {
                throw new Exception($"NamedRow 只支持 bean 类型的容器");
            }
            else
            {
                return new DList(type, ReadList(bean, x, multirow));
            }
        }

        public DType Accept(TSet type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }


        private bool TryCreateColumnStream(Sheet.NamedRow x, Sheet.Title title, out ExcelStream stream)
        {
            var cells = new List<Sheet.Cell>();
            for (int i = title.FromIndex; i <= title.ToIndex; i++)
            {
                foreach (var row in x.Rows)
                {
                    if (row.Count > i)
                    {
                        var value = row[i].Value;
                        if (!(value == null || (value is string s && string.IsNullOrEmpty(s))))
                        {
                            cells.Add(row[i]);
                        }
                    }
                }
            }
            if (cells.Count > 0)
            {
                stream = new ExcelStream(cells, 0, cells.Count - 1, "", false);
                return true;
            }
            stream = null;
            return false;
        }

        public DType Accept(TMap type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            var map = new Dictionary<DType, DType>();
            foreach (var (key, keyTitle) in x.Titles)
            {
                if (TryCreateColumnStream(x, keyTitle, out var stream))
                {
                    var keyData = type.KeyType.Apply(StringDataCreator.Ins, key);
                    var valueData = type.ValueType.Apply(ExcelDataCreator.Ins, null, stream, DefAssembly.LocalAssebmly);
                    map.Add(keyData, valueData);
                }
            }
            return new DMap(type, map);
        }

        public DType Accept(TVector2 type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TVector3 type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TVector4 type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TDateTime type, Sheet.NamedRow x, bool multirow, bool nullable)
        {
            throw new NotSupportedException();
        }
    }
}
