using System.Numerics;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using Neo.IronLua;

namespace Luban.DataLoader.Builtin.DataVisitors;

class LuaDataCreator : ITypeFuncVisitor<object, DefAssembly, DType>
{
    public static LuaDataCreator Ins { get; } = new();

    public DType Accept(TBool type, object x, DefAssembly ass)
    {
        return DBool.ValueOf((bool)x);
    }

    public DType Accept(TByte type, object x, DefAssembly ass)
    {
        return DByte.ValueOf((byte)(int)x);
    }

    public DType Accept(TShort type, object x, DefAssembly ass)
    {
        return DShort.ValueOf((short)(int)x);
    }

    public DType Accept(TInt type, object x, DefAssembly ass)
    {
        return DInt.ValueOf((int)x);
    }

    private long ToLong(object x)
    {
        return x switch
        {
            int a => a,
            long b => b,
            double c => (long)c,
            float d => (long)d,
            _ => throw new Exception($"{x} 不是 long 类型数据"),
        };
    }

    private float ToFloat(object x)
    {
        return x switch
        {
            int a => a,
            long b => b,
            double c => (float)c,
            float d => d,
            _ => throw new Exception($"{x} 不是 float 类型数据"),
        };
    }

    private double ToDouble(object x)
    {
        return x switch
        {
            int a => a,
            long b => b,
            double c => c,
            float d => d,
            _ => throw new Exception($"{x} 不是 double 类型数据"),
        };
    }

    public DType Accept(TLong type, object x, DefAssembly ass)
    {
        return DLong.ValueOf(ToLong(x));
    }

    public DType Accept(TFloat type, object x, DefAssembly ass)
    {
        return DFloat.ValueOf(ToFloat(x));
    }

    public DType Accept(TDouble type, object x, DefAssembly ass)
    {
        return DDouble.ValueOf(ToDouble(x));
    }

    public DType Accept(TEnum type, object x, DefAssembly ass)
    {
        return new DEnum(type, x?.ToString());
    }

    public DType Accept(TString type, object x, DefAssembly ass)
    {
        if (x is string s)
        {
            return DString.ValueOf(type, s);
        }
        else
        {
            throw new Exception($"{x} 不是 string 类型数据");
        }
    }

    private object GetBeanField(LuaTable table, DefField f)
    {
        object ele;
        if (!string.IsNullOrEmpty(f.CurrentVariantNameWithoutFieldName))
        {
            ele = table[f.CurrentVariantNameWithFieldName];
            if (ele != null)
            {
                return ele;
            }
        }
        ele = table[f.Name];
        if (ele == null && !string.IsNullOrEmpty(f.Alias))
        {
            ele = table[f.Alias];
        }
        return ele;
    }

    public DType Accept(TBean type, object x, DefAssembly ass)
    {
        var table = (LuaTable)x;
        var bean = type.DefBean;

        DefBean implBean;
        if (bean.IsAbstractType)
        {
            string subType;
            if (table.ContainsKey(FieldNames.LuaTypeNameKey))
            {
                subType = (string)(table[FieldNames.LuaTypeNameKey]);
            }
            else if (table.ContainsKey(FieldNames.FallbackTypeNameKey))
            {
                subType = (string)table[FieldNames.FallbackTypeNameKey];
            }
            else
            {
                throw new Exception($"结构:{bean.FullName} 是多态类型，必须用 {FieldNames.LuaTypeNameKey} 字段指定 子类名");
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
            var ele = GetBeanField(table, f);

            if (ele != null)
            {
                try
                {
                    // Console.WriteLine("field:{0} type:{1} value:{2}", field.Name, ele.GetType(), ele);
                    fields.Add(f.CType.Apply(this, ele, ass));
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
            else if (f.CType.IsNullable)
            {
                fields.Add(null);
            }
            else
            {
                throw new Exception($"结构:{implBean.FullName} 字段:{f.Name} 缺失");
            }
        }
        return new DBean(type, implBean, fields);
    }

    private List<DType> ReadList(TType type, LuaTable e, DefAssembly ass)
    {
        var list = new List<DType>();
        foreach (var c in e.ArrayList)
        {
            list.Add(type.Apply(this, c, ass));
        }
        return list;
    }

    public DType Accept(TArray type, object x, DefAssembly ass)
    {
        return new DArray(type, ReadList(type.ElementType, (LuaTable)x, ass));
    }

    public DType Accept(TList type, object x, DefAssembly ass)
    {
        return new DList(type, ReadList(type.ElementType, (LuaTable)x, ass));
    }

    public DType Accept(TSet type, object x, DefAssembly ass)
    {
        return new DSet(type, ReadList(type.ElementType, (LuaTable)x, ass));
    }

    public DType Accept(TMap type, object x, DefAssembly ass)
    {
        var table = (LuaTable)x;
        var map = new Dictionary<DType, DType>();
        foreach (var e in table.Values)
        {
            DType key = type.KeyType.Apply(this, e.Key, ass);
            DType value = type.ValueType.Apply(this, e.Value, ass);
            if (!map.TryAdd(key, value))
            {
                throw new Exception($"map 的 key:{key} 重复");
            }
        }
        return new DMap(type, map);
    }

    public DType Accept(TDateTime type, object x, DefAssembly ass)
    {
        return DataUtil.CreateDateTime(x.ToString());
    }
}
