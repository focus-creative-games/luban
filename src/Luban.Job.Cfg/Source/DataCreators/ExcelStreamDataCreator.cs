using Bright.Collections;
using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataCreators
{

    class ExcelStreamDataCreator : ITypeFuncVisitor<ExcelStream, DType>
    {
        public static ExcelStreamDataCreator Ins { get; } = new ExcelStreamDataCreator();

        private bool CheckNull(bool nullable, object o)
        {
            return nullable && (o == null || (o is string s && s == "null"));
        }

        private static bool CreateBool(object x)
        {
            if (x is bool b)
            {
                return b;
            }
            var s = x.ToString().ToLower().Trim();
            return DataUtil.ParseExcelBool(s);
        }

        public DType Accept(TBool type, ExcelStream x)
        {

            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DBool.ValueOf(CreateBool(d));
        }

        public DType Accept(TByte type, ExcelStream x)
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
            return DByte.ValueOf(v);
        }

        public DType Accept(TShort type, ExcelStream x)
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
            return DShort.ValueOf(v);
        }

        public DType Accept(TFshort type, ExcelStream x)
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
            return DFshort.ValueOf(v);
        }

        public DType Accept(TInt type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            //if (field?.Remapper is TEnum te)
            //{
            //    if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
            //    {
            //        return DInt.ValueOf(c);
            //    }
            //}
            if (!int.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 int 类型值");
            }
            return DInt.ValueOf(v);
        }

        public DType Accept(TFint type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            //if (field?.Remapper is TEnum te)
            //{
            //    if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
            //    {
            //        return new DFint(c);
            //    }
            //}
            if (!int.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 int 类型值");
            }
            return DFint.ValueOf(v);
        }

        public DType Accept(TLong type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            //if (field?.Remapper is TEnum te)
            //{
            //    if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
            //    {
            //        return DLong.ValueOf(c);
            //    }
            //}
            if (!long.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 long 类型值");
            }
            return DLong.ValueOf(v);
        }

        public DType Accept(TFlong type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            var ds = d.ToString();
            //if (field?.Remapper is TEnum te)
            //{
            //    if (te.DefineEnum.TryValueByNameOrAlias(ds, out var c))
            //    {
            //        return new DFlong(c);
            //    }
            //}
            if (!long.TryParse(ds, out var v))
            {
                throw new InvalidExcelDataException($"{d} 不是 long 类型值");
            }
            return DFlong.ValueOf(v);
        }

        public DType Accept(TFloat type, ExcelStream x)
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
            return DFloat.ValueOf(v);
        }

        public DType Accept(TDouble type, ExcelStream x)
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
            return DDouble.ValueOf(v);
        }

        public DType Accept(TEnum type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            if (d == null)
            {
                throw new InvalidExcelDataException($"枚举值不能为空");
            }
            return new DEnum(type, d.ToString().Trim());
        }

        public DType Accept(TString type, ExcelStream x)
        {
            var d = x.Read();
            var s = ParseString(d);
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

        public DType Accept(TBytes type, ExcelStream x)
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
                return DataUtil.UnEscapeRawString(s);
            }
            else
            {
                return d.ToString();
            }
        }

        public DType Accept(TText type, ExcelStream x)
        {
            //x = SepIfNeed(type, x);
            x = TrySep(type, x);

            string key = ParseString(x.Read());
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

            string text = ParseString(x.Read());
            DataUtil.ValidateText(key, text);
            return new DText(key, text);
        }

        public DType Accept(TDateTime type, ExcelStream x)
        {
            var d = x.Read();
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

        public DType Accept(TVector2 type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector3 type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        public DType Accept(TVector4 type, ExcelStream x)
        {
            var d = x.Read();
            if (CheckNull(type.IsNullable, d))
            {
                return null;
            }
            return DataUtil.CreateVector(type, d.ToString());
        }

        private List<DType> CreateBeanFields(DefBean bean, ExcelStream stream)
        {
            var list = new List<DType>();
            foreach (DefField f in bean.HierarchyFields)
            {
                try
                {
                    //string sep = f.Tags.TryGetValue("tag", out var s) ? s : null;
                    //if (string.IsNullOrWhiteSpace(sep))
                    //{
                    list.Add(f.CType.Apply(this, stream));
                    //}
                    //else
                    //{
                    //    list.Add(f.CType.Apply(this, new ExcelStream(stream.ReadCell(), sep)));
                    //}
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
            return list;
        }

        public DType Accept(TBean type, ExcelStream x)
        {
            var originBean = (DefBean)type.Bean;
            if (!string.IsNullOrEmpty(originBean.Sep))
            {
                x = new ExcelStream(x.ReadCell(), originBean.Sep);
            }
            else
            {
                x = TrySep(type, x);
            }

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
                DefBean implType = DataUtil.GetImplTypeByNameOrAlias(originBean, subType);
                return new DBean(type, implType, CreateBeanFields(implType, x));
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
                        throw new Exception($"type:'{type.Bean.FullName}' 可空标识:'{subType}' 不合法（只能为{DefBean.BEAN_NOT_NULL_STR}或{DefBean.BEAN_NULL_STR}或{originBean.Name})");
                    }
                }
                return new DBean(type, originBean, CreateBeanFields(originBean, x));
            }
        }

        private static ExcelStream TrySep(TType type, ExcelStream stream)
        {
            string sep = type.GetTag("sep");
            
            if (!string.IsNullOrEmpty(sep) && !stream.TryReadEOF())
            {
                stream = new ExcelStream(stream.ReadCell(), sep);
            }
            return stream;
        }

        // 容器类统统不支持 type.IsNullable
        // 因为貌似没意义？
        public List<DType> ReadList(TType type, TType eleType, ExcelStream stream)
        {
            var datas = new List<DType>();
            stream = TrySep(type, stream);
            while (!stream.TryReadEOF())
            {
                datas.Add(eleType.Apply(this, stream));
            }
            return datas;
        }

        public DType Accept(TArray type, ExcelStream x)
        {
            return new DArray(type, ReadList(type, type.ElementType, x));
        }

        public DType Accept(TList type, ExcelStream x)
        {
            return new DList(type, ReadList(type, type.ElementType, x));
        }

        public DType Accept(TSet type, ExcelStream x)
        {
            return new DSet(type, ReadList(type, type.ElementType, x));
        }

        public DType Accept(TMap type, ExcelStream stream)
        {
            //x = SepIfNeed(type, x);
            stream = TrySep(type, stream);

            var datas = new Dictionary<DType, DType>();
            while (!stream.TryReadEOF())
            {
                var key = type.KeyType.Apply(this, stream);
                var value = type.ValueType.Apply(this, stream);
                if (!datas.TryAdd(key, value))
                {
                    throw new InvalidExcelDataException($"map 的 key:{key} 重复");
                }
            }
            return new DMap(type, datas);
        }
    }
}
