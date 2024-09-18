using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using YamlDotNet.RepresentationModel;

namespace Luban.DataLoader.Builtin.DataVisitors;

class YamlDataCreator : ITypeFuncVisitor<YamlNode, DefAssembly, DType>
{
    public static YamlDataCreator Ins { get; } = new();

    private static string GetLowerTextValue(YamlNode node)
    {
        return ((string)node).Trim().ToLower();
    }

    private static string GetTextValue(YamlNode node)
    {
        return node.ToString();
    }

    public DType Accept(TBool type, YamlNode x, DefAssembly y)
    {
        return DBool.ValueOf(bool.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TByte type, YamlNode x, DefAssembly y)
    {
        return DByte.ValueOf(byte.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TShort type, YamlNode x, DefAssembly y)
    {
        return DShort.ValueOf(short.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TInt type, YamlNode x, DefAssembly y)
    {
        return DInt.ValueOf(int.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TLong type, YamlNode x, DefAssembly y)
    {
        return DLong.ValueOf(long.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TFloat type, YamlNode x, DefAssembly y)
    {
        return DFloat.ValueOf(float.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TDouble type, YamlNode x, DefAssembly y)
    {
        return DDouble.ValueOf(double.Parse(GetLowerTextValue(x)));
    }

    public DType Accept(TEnum type, YamlNode x, DefAssembly y)
    {
        return new DEnum(type, GetTextValue(x));
    }

    public DType Accept(TString type, YamlNode x, DefAssembly y)
    {
        return DString.ValueOf(type, GetTextValue(x));
    }

    private static readonly YamlScalarNode s_typeNodeName = new(FieldNames.JsonTypeNameKey);
    private static readonly YamlScalarNode s_typeNodeNameFallback = new(FieldNames.FallbackTypeNameKey);

    private bool TryGetBeanField(YamlMappingNode x, DefField field, out YamlNode ele)
    {
        if (!string.IsNullOrEmpty(field.CurrentVariantNameWithFieldName))
        {
            if (x.Children.TryGetValue(new YamlScalarNode(field.CurrentVariantNameWithFieldName), out ele))
            {
                return true;
            }
        }
        if (x.Children.TryGetValue(new YamlScalarNode(field.Name), out ele))
        {
            return true;
        }
        if (!string.IsNullOrEmpty(field.Alias) && x.Children.TryGetValue(new YamlScalarNode(field.Alias), out ele))
        {
            return true;
        }
        return false;
    }

    public DType Accept(TBean type, YamlNode x, DefAssembly y)
    {
        var m = (YamlMappingNode)x;
        var bean = type.DefBean;

        DefBean implBean;
        if (bean.IsAbstractType)
        {
            if (!m.Children.TryGetValue(s_typeNodeName, out var typeNode) && !m.Children.TryGetValue(s_typeNodeNameFallback, out typeNode))
            {
                throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{FieldNames.JsonTypeNameKey}属性.\n xml:{x}");
            }
            string subType = (string)typeNode;
            if (string.IsNullOrWhiteSpace(subType))
            {
                throw new Exception($"bean:'{bean.FullName}'是多态，需要指定{FieldNames.JsonTypeNameKey}属性.\n xml:{x}");
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
            if (!TryGetBeanField(m, f, out var fele))
            {
                if (f.CType.IsNullable)
                {
                    fields.Add(null);
                    continue;
                }
                throw new Exception($"bean:{implBean.FullName} 字段:{f.Name} 缺失");
            }
            try
            {
                fields.Add(f.CType.Apply(this, fele, y));
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

    private List<DType> ReadList(TType type, YamlSequenceNode x, DefAssembly ass)
    {
        var list = new List<DType>();
        foreach (var e in x)
        {
            list.Add(type.Apply(this, e, ass));
        }
        return list;
    }

    public DType Accept(TArray type, YamlNode x, DefAssembly y)
    {
        return new DArray(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
    }

    public DType Accept(TList type, YamlNode x, DefAssembly y)
    {
        return new DList(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
    }

    public DType Accept(TSet type, YamlNode x, DefAssembly y)
    {
        return new DSet(type, ReadList(type.ElementType, (YamlSequenceNode)x, y));
    }

    public DType Accept(TMap type, YamlNode x, DefAssembly y)
    {
        var m = (YamlSequenceNode)x;
        var map = new Dictionary<DType, DType>();
        foreach (var e in m)
        {
            var kv = (YamlSequenceNode)e;
            if (kv.Count() != 2)
            {
                throw new ArgumentException($"yaml map 类型的 成员数据项:{e} 必须是 [key,value] 形式的列表");
            }
            DType key = type.KeyType.Apply(this, kv[0], y);
            DType value = type.ValueType.Apply(this, kv[1], y);
            if (!map.TryAdd(key, value))
            {
                throw new Exception($"map 的 key:{key} 重复");
            }
        }
        return new DMap(type, map);
    }

    public DType Accept(TDateTime type, YamlNode x, DefAssembly y)
    {
        return DataUtil.CreateDateTime(GetLowerTextValue(x));
    }
}
