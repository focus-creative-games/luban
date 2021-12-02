using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.DataCreators
{
    class SheetDataCreator : ITypeFuncVisitor<Sheet, TitleRow, DType>
    {
        public static SheetDataCreator Ins { get; } = new();

        private bool CheckNull(bool nullable, object o)
        {
            return nullable && (o == null || (o is string s && s == "null"));
        }

        private bool CheckDefault(object o)
        {
            return o == null || (o is string s && s.Length == 0);
        }

        private void ThrowIfNonEmpty(TitleRow row)
        {
            if (row.SelfTitle.NonEmpty)
            {
                throw new Exception($"字段不允许为空");
            }
        }

        public DType Accept(TBool type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DBool.ValueOf(false);
            }
            if (x is bool v)
            {
                return DBool.ValueOf(v);
            }
            return DBool.ValueOf(bool.Parse(x.ToString()));
        }

        public DType Accept(TByte type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                return DByte.Default;
            }
            return DByte.ValueOf(byte.Parse(x.ToString()));
        }

        public DType Accept(TShort type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DShort.Default;
            }
            return DShort.ValueOf(short.Parse(x.ToString()));
        }

        public DType Accept(TFshort type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                return DFshort.Default;
            }
            return DFshort.ValueOf(short.Parse(x.ToString()));
        }

        public DType Accept(TInt type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DInt.Default;
            }
            return DInt.ValueOf(int.Parse(x.ToString()));
        }

        public DType Accept(TFint type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DFint.Default;
            }
            return DFint.ValueOf(int.Parse(x.ToString()));
        }

        public DType Accept(TLong type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DLong.Default;
            }
            return DLong.ValueOf(long.Parse(x.ToString()));
        }

        public DType Accept(TFlong type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DFlong.Default;
            }
            return DFlong.ValueOf(long.Parse(x.ToString()));
        }

        public DType Accept(TFloat type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DFloat.Default;
            }
            return DFloat.ValueOf(float.Parse(x.ToString()));
        }

        public DType Accept(TDouble type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
                return DDouble.Default;
            }
            return DDouble.ValueOf(double.Parse(x.ToString()));
        }

        public DType Accept(TEnum type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckNull(type.IsNullable, x))
            {
                return null;
            }
            if (x == null)
            {
                throw new InvalidExcelDataException($"枚举值不能为空");
            }
            return new DEnum(type, x.ToString());
        }


        private static string ParseString(object d)
        {
            if (d == null)
            {
                return string.Empty;
            }
            else if (d is string s)
            {
                return DataUtil.UnEscapeRawString(s);
            }
            else
            {
                return d.ToString();
            }
        }

        public DType Accept(TString type, Sheet sheet, TitleRow row)
        {
            object x = row.Current;
            if (CheckDefault(x))
            {
                ThrowIfNonEmpty(row);
            }
            var s = ParseString(x);
            if (s == null)
            {
                if (type.IsNullable)
                {
                    return null;
                }
                else
                {
                    throw new InvalidExcelDataException("字段不是nullable类型，不能为null");
                }
            }
            return DString.ValueOf(s);
        }

        public DType Accept(TBytes type, Sheet sheet, TitleRow row)
        {
            throw new NotImplementedException();
        }

        public DType Accept(TText type, Sheet sheet, TitleRow row)
        {
            if (string.IsNullOrEmpty(row.SelfTitle.Sep))
            {
                if (row.CellCount != 2)
                {
                    throw new Exception($"text 要求两个字段");
                }
                int startIndex = row.SelfTitle.FromIndex;
                var key = ParseString(row.Row[startIndex].Value);
                var text = ParseString(row.Row[startIndex + 1].Value);
                if (type.IsNullable && key == null && text == null)
                {
                    return null;
                }
                DataUtil.ValidateText(key, text);
                return new DText(key, text);
            }
            else
            {
                var s = row.AsStream("");
                return type.Apply(ExcelStreamDataCreator.Ins, s);
            }
        }

        private List<DType> CreateBeanFields(DefBean bean, Sheet sheet, TitleRow row)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)

            {
                string fname = f.Name;
                TitleRow field = row.GetSubTitleNamedRow(fname);
                if (field == null)
                {
                    throw new Exception($"bean:'{bean.FullName}' 缺失 列:'{fname}'，请检查是否写错或者遗漏");
                }
                try
                {
                    list.Add(f.CType.Apply(this, sheet, field));
                }
                catch (DataCreateException dce)
                {
                    dce.Push(bean, f);
                    throw;
                }
                catch (Exception e)
                {
                    var dce = new DataCreateException(e, $"字段：{fname} 位置:{field.Location}");
                    dce.Push(bean, f);
                    throw dce;
                }
            }
            return list;
        }

        public DType Accept(TBean type, Sheet sheet, TitleRow row)
        {
            //string sep = DataUtil.GetSep(type);

            if (row.Row != null)
            {
                var s = row.AsStream("");
                if (type.IsNullable && s.TryReadEOF())
                {
                    return null;
                }
                return type.Apply(ExcelStreamDataCreator.Ins, s);
            }
            else if (row.Rows != null)
            {
                var s = row.AsMultiRowConcatStream("");
                if (type.IsNullable && s.TryReadEOF())
                {
                    return null;
                }
                return type.Apply(ExcelStreamDataCreator.Ins, s);
            }
            else if (row.Fields != null)
            {
                var originBean = (DefBean)type.Bean;
                if (originBean.IsAbstractType)
                {
                    string subType = row.GetSubTitleNamedRow(DefBean.TYPE_NAME_KEY).Current.ToString().Trim();
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
                    return new DBean(type, implType, CreateBeanFields(implType, sheet, row));
                }
                else
                {
                    if (type.IsNullable)
                    {
                        string subType = row.GetSubTitleNamedRow(DefBean.TYPE_NAME_KEY).Current?.ToString()?.Trim();
                        if (subType == null || subType == DefBean.BEAN_NULL_STR)
                        {
                            return null;
                        }
                        else if (subType != DefBean.BEAN_NOT_NULL_STR && subType != originBean.Name)
                        {
                            throw new Exception($"type:'{type.Bean.FullName}' 可空标识:'{subType}' 不合法（只能为{DefBean.BEAN_NOT_NULL_STR}或{DefBean.BEAN_NULL_STR}或{originBean.Name})");
                        }
                    }

                    return new DBean(type, originBean, CreateBeanFields(originBean, sheet, row));
                }
            }
            else if (row.Elements != null)
            {
                var s = row.AsMultiRowConcatElements();
                return type.Apply(ExcelStreamDataCreator.Ins, s);
            }
            else
            {
                throw new Exception();
            }
        }

        public List<DType> ReadList(TType type, ExcelStream stream)
        {
            var datas = new List<DType>();
            while (!stream.TryReadEOF())
            {
                datas.Add(type.Apply(ExcelStreamDataCreator.Ins, stream));
            }
            return datas;
        }

        private static List<DType> ReadList(TType type, IEnumerable<ExcelStream> streams)
        {
            var datas = new List<DType>();
            foreach (var stream in streams)
            {
                while (!stream.TryReadEOF())
                {
                    datas.Add(type.Apply(ExcelStreamDataCreator.Ins, stream));
                }
            }
            return datas;
        }

        private List<DType> ReadCollectionDatas(TType elementType, Sheet sheet, TitleRow row)
        {
            if (row.Row != null)
            {
                var s = row.AsStream(DataUtil.GetTypeSep(elementType));
                return ReadList(elementType, s);
            }
            else if (row.Rows != null)
            {
                var s = row.AsMultiRowStream(DataUtil.GetTypeSep(elementType));
                return ReadList(elementType, s);
            }
            else if (row.Fields != null)
            {
                //throw new Exception($"array 不支持 子字段. 忘记将字段设为多行模式?  {row.SelfTitle.Name} => *{row.SelfTitle.Name}");

                var datas = new List<DType>(row.Fields.Count);
                var sortedFields = row.Fields.Values.ToList();
                sortedFields.Sort((a, b) => a.SelfTitle.FromIndex - b.SelfTitle.FromIndex);
                foreach (var field in sortedFields)
                {
                    if (field.IsBlank)
                    {
                        continue;
                    }
                    datas.Add(elementType.Apply(this, sheet, field));
                }
                return datas;
            }
            else if (row.Elements != null)
            {
                return row.Elements.Select(e => elementType.Apply(this, sheet, e)).ToList();
            }
            else
            {
                throw new Exception();
            }
        }

        public DType Accept(TArray type, Sheet sheet, TitleRow row)
        {
            //string sep = DataUtil.GetSep(type);
            return new DArray(type, ReadCollectionDatas(type.ElementType, sheet, row));
        }

        public DType Accept(TList type, Sheet sheet, TitleRow row)
        {
            return new DList(type, ReadCollectionDatas(type.ElementType, sheet, row));
        }

        public DType Accept(TSet type, Sheet sheet, TitleRow row)
        {
            return new DSet(type, ReadCollectionDatas(type.ElementType, sheet, row));
        }

        public DType Accept(TMap type, Sheet sheet, TitleRow row)
        {
            string sep = "";

            if (row.Row != null)
            {
                var s = row.AsStream(sep);
                var datas = new Dictionary<DType, DType>();

                while (!s.TryReadEOF())
                {
                    var key = type.KeyType.Apply(ExcelStreamDataCreator.Ins, s);
                    var value = type.ValueType.Apply(ExcelStreamDataCreator.Ins, s);
                    datas.Add(key, value);
                }

                return new DMap(type, datas);
            }
            else if (row.Rows != null)
            {
                var datas = new Dictionary<DType, DType>();
                foreach (ExcelStream s in row.AsMultiRowStream(sep))
                {
                    while (!s.TryReadEOF())
                    {
                        var key = type.KeyType.Apply(ExcelStreamDataCreator.Ins, s);
                        var value = type.ValueType.Apply(ExcelStreamDataCreator.Ins, s);
                        datas.Add(key, value);
                    }
                }
                return new DMap(type, datas);
            }
            else if (row.Fields != null)
            {
                var datas = new Dictionary<DType, DType>();
                foreach (var e in row.Fields)
                {
                    var keyData = type.KeyType.Apply(StringDataCreator.Ins, e.Key);
                    if (Sheet.IsBlankRow(e.Value.Row, e.Value.SelfTitle.FromIndex, e.Value.SelfTitle.ToIndex))
                    {
                        continue;
                    }
                    var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, e.Value.AsStream(sep));
                    datas.Add(keyData, valueData);
                }
                return new DMap(type, datas);
            }
            else if (row.Elements != null)
            {
                var datas = new Dictionary<DType, DType>();
                foreach (var e in row.Elements)
                {
                    var stream = e.AsStream("");
                    var keyData = type.KeyType.Apply(ExcelStreamDataCreator.Ins, stream);
                    var valueData = type.ValueType.Apply(ExcelStreamDataCreator.Ins, stream);
                    datas.Add(keyData, valueData);
                }
                return new DMap(type, datas);
            }
            else
            {
                throw new Exception();
            }
        }

        public DType Accept(TVector2 type, Sheet sheet, TitleRow row)
        {
            var d = row.Current;
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (CheckDefault(d))
            {
                ThrowIfNonEmpty(row);
                return DVector2.Default;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector3 type, Sheet sheet, TitleRow row)
        {
            var d = row.Current;
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (CheckDefault(d))
            {
                ThrowIfNonEmpty(row);
                return DVector3.Default;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector4 type, Sheet sheet, TitleRow row)
        {
            var d = row.Current;
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (CheckDefault(d))
            {
                ThrowIfNonEmpty(row);
                return DVector4.Default;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TDateTime type, Sheet sheet, TitleRow row)
        {
            var d = row.Current;
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (d is System.DateTime datetime)
            {
                return new DDateTime(datetime);
            }
            return DataUtil.CreateDateTime(d.ToString());
        }
    }
}
