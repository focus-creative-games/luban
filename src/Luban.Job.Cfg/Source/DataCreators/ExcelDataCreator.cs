using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Luban.Job.Cfg.DataCreators
{
    enum EReadPolicy
    {
        SKIP_NULL_CELL = 0x1,
        SKIP_BLANK_CELL = 0x2,
        NULL_AS_NULL = 0x4,
        NULL_STR_AS_NULL = 0x8,
    }

    class InvalidExcelDataException : Exception
    {
        public InvalidExcelDataException()
        {
        }

        public InvalidExcelDataException(string message) : base(message)
        {
        }

        public InvalidExcelDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidExcelDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    class ExcelDataCreator : ITypeFuncVisitor<object, ExcelStream, DefAssembly, DType>
    {
        public static ExcelDataCreator Ins { get; } = new ExcelDataCreator();

        private bool CheckNull(bool nullable, object o)
        {
            if (o is string s && s == "null")
            {
                if (nullable)
                {
                    return true;
                }
                else
                {
                    throw new InvalidExcelDataException($"单元格没有填有效数据");
                }
            }
            return false;
        }

        private static bool CreateBool(object x)
        {
            if (x is bool b)
            {
                return b;
            }
            var s = x.ToString().ToLower().Trim();
            switch (s)
            {
                case "true":
                case "是": return true;
                case "false":
                case "否": return false;
                default: throw new InvalidExcelDataException($"{s} 不是 bool 类型的值 (true 或 false)");
            }
        }

        public DType Accept(TBool type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return new DBool(CreateBool(d));
        }

        public DType Accept(TByte type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (!byte.TryParse(d.ToString(), out byte v))
            {
                throw new InvalidExcelDataException($"{d} 不是 byte 类型值");
            }
            return new DByte(v);
        }

        public DType Accept(TShort type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (!short.TryParse(d.ToString(), out short v))
            {
                throw new InvalidExcelDataException($"{d} 不是 short 类型值");
            }
            return new DShort(v);
        }

        public DType Accept(TFshort type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (!short.TryParse(d.ToString(), out short v))
            {
                throw new InvalidExcelDataException($"{d} 不是 short 类型值");
            }
            return new DFshort(v);
        }

        public DType Accept(TInt type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            if (converter is TEnum te)
            {
                if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
                {
                    return new DInt(c);
                }
            }
            if (!int.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 int 类型值");
            }
            return new DInt(v);
        }

        public DType Accept(TFint type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            if (converter is TEnum te)
            {
                if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
                {
                    return new DFint(c);
                }
            }
            if (!int.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 int 类型值");
            }
            return new DFint(v);
        }

        public DType Accept(TLong type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            if (converter is TEnum te)
            {
                if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
                {
                    return new DLong(c);
                }
            }
            if (!long.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 long 类型值");
            }
            return new DLong(v);
        }

        public DType Accept(TFlong type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            if (converter is TEnum te)
            {
                if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
                {
                    return new DFlong(c);
                }
            }
            if (!long.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 long 类型值");
            }
            return new DFlong(v);
        }

        public DType Accept(TFloat type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (!float.TryParse(d.ToString(), out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 float 类型值");
            }
            return new DFloat(v);
        }

        public DType Accept(TDouble type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (!double.TryParse(d.ToString(), out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 double 类型值");
            }
            return new DDouble(v);
        }

        public DType Accept(TEnum type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return new DEnum(type, d.ToString().Trim());
        }

        public DType Accept(TString type, object converter, ExcelStream x, DefAssembly ass)
        {
            if (x.NamedMode && x.IncludeNullAndEmptySize != 1)
            {
                throw new InvalidExcelDataException("excel string类型在标题头对应模式下必须正好占据一个单元格");
            }
            var s = ParseString(x.Read(x.NamedMode));
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
            return new DString(s);
        }

        public DType Accept(TBytes type, object converter, ExcelStream x, DefAssembly ass)
        {
            throw new NotImplementedException();
        }

        private static string ParseString(object d)
        {
            if (d == null)
            {
                return string.Empty;
            }
            else if (d is string s)
            {
                return DataUtil.UnEscapeString(s);
            }
            else
            {
                return d.ToString();
            }
        }

        public DType Accept(TText type, object converter, ExcelStream x, DefAssembly ass)
        {
            if (x.NamedMode && x.IncludeNullAndEmptySize != 2)
            {
                throw new InvalidExcelDataException("excel text 类型在标题头对应模式下必须正好占据2个单元格");
            }
            string key = ParseString(x.Read(x.NamedMode));
            if (key == null)
            {
                if (type.IsNullable)
                {
                    return null;
                }
                else
                {
                    throw new InvalidExcelDataException("该字段不是nullable类型，不能为null");
                }
            }

            string text = ParseString(x.Read(x.NamedMode));
            DataUtil.ValidateText(key, text);
            return new DText(key, text);
        }

        private List<DType> CreateBeanFields(DefBean bean, ExcelStream stream, DefAssembly ass)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)
            {
                try
                {
                    string sep = f.ActualSep;
                    if (string.IsNullOrWhiteSpace(sep))
                    {
                        list.Add(f.CType.Apply(this, f.Remapper, stream, ass));
                    }
                    else
                    {
                        list.Add(f.CType.Apply(this, f.Remapper, new ExcelStream(stream.ReadCell(), sep, false), ass));
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidExcelDataException($"读取结构:{bean.FullName} 字段:{f.Name} 出错 ==> {e.Message}", e);
                }
            }
            return list;
        }

        public DType Accept(TBean type, object converter, ExcelStream x, DefAssembly ass)
        {
            var originBean = (DefBean)type.Bean;

            if (originBean.IsAbstractType)
            {
                string subType = x.Read().ToString();
                if (subType.ToLower().Trim() == DefBean.BEAN_NULL_STR)
                {
                    if (!type.IsNullable)
                    {
                        throw new InvalidExcelDataException($"type:{type.Bean.FullName}不是可空类型. 不能为空");
                    }
                    return null;
                }
                string fullType = TypeUtil.MakeFullName(originBean.Namespace, subType);
                DefBean implType = (DefBean)originBean.GetNotAbstractChildType(subType);
                if (implType == null)
                {
                    throw new InvalidExcelDataException($"type:{fullType} 不是bean类型");
                }
                return new DBean(originBean, implType, CreateBeanFields(implType, x, ass));
            }
            else
            {
                if (type.IsNullable)
                {
                    string subType = x.Read().ToString().Trim();
                    if (subType == DefBean.BEAN_NULL_STR)
                    {
                        return null;
                    }
                    else if (subType != DefBean.BEAN_NOT_NULL_STR && subType != originBean.Name)
                    {
                        throw new Exception($"type:{type.Bean.FullName} 可空标识:{subType} 不合法（只能为{DefBean.BEAN_NOT_NULL_STR}或{DefBean.BEAN_NULL_STR}或{originBean.Name})");
                    }
                }
                return new DBean(originBean, originBean, CreateBeanFields(originBean, x, ass));
            }
        }

        // 容器类统统不支持 type.IsNullable
        // 因为貌似没意义？
        public List<DType> ReadList(TType type, object converter, ExcelStream stream, DefAssembly ass)
        {
            stream.NamedMode = false;
            string sep = type is TBean bean ? ((DefBean)bean.Bean).Sep : null;
            var datas = new List<DType>();
            while (!stream.TryReadEOF())
            {
                if (string.IsNullOrWhiteSpace(sep))
                {
                    datas.Add(type.Apply(this, converter, stream, ass));
                }
                else
                {
                    datas.Add(type.Apply(this, converter, new ExcelStream(stream.ReadCell(), sep, false), ass)); ;
                }
            }
            return datas;
        }

        public DType Accept(TArray type, object converter, ExcelStream x, DefAssembly ass)
        {
            return new DArray(type, ReadList(type.ElementType, converter, x, ass));
        }

        public DType Accept(TList type, object converter, ExcelStream x, DefAssembly ass)
        {
            return new DList(type, ReadList(type.ElementType, converter, x, ass));
        }

        public DType Accept(TSet type, object converter, ExcelStream x, DefAssembly ass)
        {
            return new DSet(type, ReadList(type.ElementType, converter, x, ass));
        }

        public DType Accept(TMap type, object converter, ExcelStream x, DefAssembly ass)
        {
            x.NamedMode = false;
            string sep = type.ValueType is TBean bean ? ((DefBean)bean.Bean).Sep : null;

            var datas = new Dictionary<DType, DType>();
            while (!x.TryReadEOF())
            {
                var key = type.KeyType.Apply(this, null, x, ass);
                var value = string.IsNullOrWhiteSpace(sep) ? type.ValueType.Apply(this, null, x, ass) : type.ValueType.Apply(this, null, new ExcelStream(x.ReadCell(), sep, false), ass);
                if (!datas.TryAdd(key, value))
                {
                    throw new InvalidExcelDataException($"map 的 key:{key} 重复");
                }
            }
            return new DMap(type, datas);
        }

        public DType Accept(TVector2 type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector3 type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector4 type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TDateTime type, object converter, ExcelStream x, DefAssembly ass)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateDateTime(d.ToString());
        }
    }
}
