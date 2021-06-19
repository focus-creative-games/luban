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

namespace Luban.Job.Cfg.TypeVisitors
{
    class XmlDataCreator : ITypeFuncVisitor<XElement, DefAssembly, DType>
    {
        public static XmlDataCreator Ins { get; } = new XmlDataCreator();

        public DType Accept(TBool type, XElement x, DefAssembly ass)
        {
            return new DBool(bool.Parse(x.Value.Trim().ToLower()));
        }

        public DType Accept(TByte type, XElement x, DefAssembly ass)
        {
            return new DByte(byte.Parse(x.Value.Trim()));
        }

        public DType Accept(TShort type, XElement x, DefAssembly ass)
        {
            return new DShort(short.Parse(x.Value.Trim()));
        }

        public DType Accept(TFshort type, XElement x, DefAssembly ass)
        {
            return new DFshort(short.Parse(x.Value.Trim()));
        }

        public DType Accept(TInt type, XElement x, DefAssembly ass)
        {
            return new DInt(int.Parse(x.Value.Trim()));
        }

        public DType Accept(TFint type, XElement x, DefAssembly ass)
        {
            return new DFint(int.Parse(x.Value.Trim()));
        }

        public DType Accept(TLong type, XElement x, DefAssembly ass)
        {
            return new DLong(long.Parse(x.Value.Trim()));
        }

        public DType Accept(TFlong type, XElement x, DefAssembly ass)
        {
            return new DFlong(long.Parse(x.Value.Trim()));
        }

        public DType Accept(TFloat type, XElement x, DefAssembly ass)
        {
            return new DFloat(float.Parse(x.Value.Trim()));
        }

        public DType Accept(TDouble type, XElement x, DefAssembly ass)
        {
            return new DDouble(double.Parse(x.Value.Trim()));
        }

        public DType Accept(TEnum type, XElement x, DefAssembly ass)
        {
            return new DEnum(type, x.Value.Trim());
        }

        public DType Accept(TString type, XElement x, DefAssembly ass)
        {
            return new DString(x.Value);
        }

        public DType Accept(TBytes type, XElement x, DefAssembly ass)
        {
            throw new NotImplementedException();
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
                string subType = x.Attribute(DefBean.TYPE_NAME_KEY)?.Value;
                if (string.IsNullOrWhiteSpace(subType))
                {
                    throw new Exception($"bean:{bean.FullName}是多态，需要指定{DefBean.TYPE_NAME_KEY}属性.\n xml:{x}");
                }
                var fullName = TypeUtil.MakeFullName(bean.Namespace, subType);
                var defType = (DefBean)bean.GetNotAbstractChildType(subType);
                //if (defType.IsAbstractType)
                //{
                //    throw new Exception($"type:{fullName} 是抽象类. 不能创建实例");
                //}
                implBean = defType ?? throw new Exception($"type:{fullName} 不是合法类型");
            }
            else
            {
                implBean = bean;
            }

            var fields = new List<DType>();
            foreach (var field in implBean.HierarchyFields)
            {
                var feles = x.Elements(field.Name);
                XElement fele = feles.FirstOrDefault();
                if (fele == null)
                {
                    throw new Exception($"字段:{field.Name} 缺失");
                }

                try
                {
                    fields.Add(field.CType.Apply(this, fele, ass));
                }
                catch (Exception e)
                {
                    throw new Exception($"结构:{implBean.FullName} 字段:{field.Name} 读取失败 => {e.Message}", e);
                }

            }
            return new DBean(bean, implBean, fields);
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
