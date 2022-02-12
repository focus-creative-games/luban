using Bright.Collections;
using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Luban.Job.Cfg.DataCreators
{
    class XmlDataCreator : ITypeFuncVisitor<XElement, DefAssembly, DType>
    {
        public static XmlDataCreator Ins { get; } = new XmlDataCreator();

        public DType Accept(TBool type, XElement x, DefAssembly ass)
        {
            return DBool.ValueOf(bool.Parse(x.Value.Trim().ToLower()));
        }

        public DType Accept(TByte type, XElement x, DefAssembly ass)
        {
            return DByte.ValueOf(byte.Parse(x.Value.Trim()));
        }

        public DType Accept(TShort type, XElement x, DefAssembly ass)
        {
            return DShort.ValueOf(short.Parse(x.Value.Trim()));
        }

        public DType Accept(TFshort type, XElement x, DefAssembly ass)
        {
            return DFshort.ValueOf(short.Parse(x.Value.Trim()));
        }

        public DType Accept(TInt type, XElement x, DefAssembly ass)
        {
            return DInt.ValueOf(int.Parse(x.Value.Trim()));
        }

        public DType Accept(TFint type, XElement x, DefAssembly ass)
        {
            return DFint.ValueOf(int.Parse(x.Value.Trim()));
        }

        public DType Accept(TLong type, XElement x, DefAssembly ass)
        {
            return DLong.ValueOf(long.Parse(x.Value.Trim()));
        }

        public DType Accept(TFlong type, XElement x, DefAssembly ass)
        {
            return DFlong.ValueOf(long.Parse(x.Value.Trim()));
        }

        public DType Accept(TFloat type, XElement x, DefAssembly ass)
        {
            return DFloat.ValueOf(float.Parse(x.Value.Trim()));
        }

        public DType Accept(TDouble type, XElement x, DefAssembly ass)
        {
            return DDouble.ValueOf(double.Parse(x.Value.Trim()));
        }

        public DType Accept(TEnum type, XElement x, DefAssembly ass)
        {
            return new DEnum(type, x.Value.Trim());
        }

        public DType Accept(TString type, XElement x, DefAssembly ass)
        {
            return DString.ValueOf(x.Value);
        }

        public DType Accept(TBytes type, XElement x, DefAssembly ass)
        {
            throw new NotSupportedException();
        }

        public DType Accept(TText type, XElement x, DefAssembly ass)
        {
            var key = x.Element("key").Value;
            var text = x.Element("text").Value;
            DataUtil.ValidateText(key, text);
            return new DText(key, text);
        }

        public DType Accept(TBean type, XElement x, DefAssembly ass)
        {
            var bean = (DefBean)type.Bean;

            DefBean implBean;
            if (bean.IsAbstractType)
            {
                string subType = x.Attribute(DefBean.XML_TYPE_NAME_KEY)?.Value;
                if (string.IsNullOrEmpty(subType))
                {
                    subType = x.Attribute(DefBean.FALLBACK_TYPE_NAME_KEY)?.Value;
                }
                if (string.IsNullOrWhiteSpace(subType))
                {
                    throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{DefBean.XML_TYPE_NAME_KEY}属性.\n xml:{x}");
                }
                implBean = DataUtil.GetImplTypeByNameOrAlias(bean, subType);
            }
            else
            {
                implBean = bean;
            }

            var fields = new List<DType>();
            foreach (DefField f in implBean.HierarchyFields)
            {
                var feles = x.Elements(f.Name);
                XElement fele = feles.FirstOrDefault();
                if (fele == null)
                {
                    if (f.CType.IsNullable)
                    {
                        fields.Add(null);
                        continue;
                    }
                    throw new Exception($"字段:{f.Name} 缺失");
                }
                try
                {
                    fields.Add(f.CType.Apply(this, fele, ass));
                }
                catch (DataCreateException dce)
                {
                    dce.Push(implBean, f);
                    throw;
                }
                catch (Exception e)
                {
                    var dce = new DataCreateException(e, "");
                    dce.Push(bean, f);
                    throw dce;
                }

            }
            return new DBean(type, implBean, fields);
        }

        private List<DType> ReadList(TType type, XElement x, DefAssembly ass)
        {
            var list = new List<DType>();
            foreach (var e in x.Elements())
            {
                list.Add(type.Apply(this, e, ass));
            }
            return list;
        }

        public DType Accept(TArray type, XElement x, DefAssembly ass)
        {
            return new DArray(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TList type, XElement x, DefAssembly ass)
        {
            return new DList(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TSet type, XElement x, DefAssembly ass)
        {
            return new DSet(type, ReadList(type.ElementType, x, ass));
        }

        public DType Accept(TMap type, XElement x, DefAssembly ass)
        {
            var map = new Dictionary<DType, DType>();
            foreach (var e in x.Elements())
            {
                DType key = type.KeyType.Apply(this, e.Element("key"), ass);
                DType value = type.ValueType.Apply(this, e.Element("value"), ass);
                if (!map.TryAdd(key, value))
                {
                    throw new Exception($"map 的 key:{key} 重复");
                }
            }
            return new DMap(type, map);
        }

        public DType Accept(TVector2 type, XElement x, DefAssembly ass)
        {
            return DataUtil.CreateVector(type, x.Value);
        }

        public DType Accept(TVector3 type, XElement x, DefAssembly ass)
        {
            return DataUtil.CreateVector(type, x.Value);
        }

        public DType Accept(TVector4 type, XElement x, DefAssembly ass)
        {
            return DataUtil.CreateVector(type, x.Value);
        }

        public DType Accept(TDateTime type, XElement x, DefAssembly ass)
        {
            return DataUtil.CreateDateTime(x.Value);
        }
    }
}
