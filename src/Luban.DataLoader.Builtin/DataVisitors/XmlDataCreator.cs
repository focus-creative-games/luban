using System.Xml.Linq;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class XmlDataCreator : ITypeFuncVisitor<XElement, DefAssembly, DType>
{
    public static XmlDataCreator Ins { get; } = new();

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

    public DType Accept(TInt type, XElement x, DefAssembly ass)
    {
        return DInt.ValueOf(int.Parse(x.Value.Trim()));
    }

    public DType Accept(TLong type, XElement x, DefAssembly ass)
    {
        return DLong.ValueOf(long.Parse(x.Value.Trim()));
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
        return DString.ValueOf(type, x.Value);
    }

    public DType Accept(TBean type, XElement x, DefAssembly ass)
    {
        var bean = type.DefBean;

        DefBean implBean;
        if (bean.IsAbstractType)
        {
            string subType = x.Attribute(FieldNames.XmlTypeNameKey)?.Value;
            if (string.IsNullOrEmpty(subType))
            {
                subType = x.Attribute(FieldNames.FallbackTypeNameKey)?.Value;
            }
            if (string.IsNullOrWhiteSpace(subType))
            {
                throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{FieldNames.XmlTypeNameKey}属性.\n xml:{x}");
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

    public DType Accept(TDateTime type, XElement x, DefAssembly ass)
    {
        return DataUtil.CreateDateTime(x.Value);
    }
}
