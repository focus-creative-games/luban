using Luban.DataLoader.Builtin.Excel;
using Luban.DataLoader.Builtin.Lite;
using Luban.DataLoader.Builtin.Utils;
using Luban.Datas;
using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class LiteStreamDataCreator : ITypeFuncVisitor<LiteStream, DType>
{
    public static LiteStreamDataCreator Ins { get; } = new();

    private bool CheckNull(bool nullable, string s)
    {
        return nullable && s == "null";
    }

    private static bool CreateBool(string x)
    {
        var s = x.Trim();
        return LoadDataUtil.ParseExcelBool(s);
    }

    public DType Accept(TBool type, LiteStream x)
    {

        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return DBool.ValueOf(CreateBool(d));
    }

    public DType Accept(TByte type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!byte.TryParse(d, out byte v))
        {
            throw new InvalidExcelDataException($"{d} 不是 byte 类型值");
        }
        return DByte.ValueOf(v);
    }

    public DType Accept(TShort type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!short.TryParse(d, out short v))
        {
            throw new InvalidExcelDataException($"{d} 不是 short 类型值");
        }
        return DShort.ValueOf(v);
    }

    public DType Accept(TInt type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!int.TryParse(d, out var v))
        {
            throw new InvalidExcelDataException($"{d} 不是 int 类型值");
        }
        return DInt.ValueOf(v);
    }

    public DType Accept(TLong type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        //}
        if (!long.TryParse(d, out var v))
        {
            throw new InvalidExcelDataException($"{d} 不是 long 类型值");
        }
        return DLong.ValueOf(v);
    }

    public DType Accept(TFloat type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!float.TryParse(d, out var v))
        {
            throw new InvalidExcelDataException($"{d} 不是 float 类型值");
        }
        return DFloat.ValueOf(v);
    }

    public DType Accept(TDouble type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        if (!double.TryParse(d, out var v))
        {
            throw new InvalidExcelDataException($"{d} 不是 double 类型值");
        }
        return DDouble.ValueOf(v);
    }

    public DType Accept(TEnum type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return new DEnum(type, d.Trim());
    }

    private static string ParseString(string s, bool nullable)
    {
        if (nullable && (string.IsNullOrEmpty(s) || s == "null"))
        {
            return null;
        }
        return DataUtil.RemoveStringQuote(s);
    }

    public DType Accept(TString type, LiteStream x)
    {
        string d = x.ReadData();
        var s = ParseString(d, type.IsNullable);
        if (s == null)
        {
            if (type.IsNullable)
            {
                return null;
            }
            throw new InvalidExcelDataException("字段不是nullable类型，不能为null");
        }
        return DString.ValueOf(type, s);
    }

    public DType Accept(TDateTime type, LiteStream x)
    {
        string d = x.ReadData();
        if (CheckNull(type.IsNullable, d))
        {
            return null;
        }
        return DataUtil.CreateDateTime(d);
    }

    private List<DType> CreateBeanFields(DefBean bean, LiteStream stream)
    {
        var list = new List<DType>();
        foreach (DefField f in bean.HierarchyFields)
        {
            try
            {
                list.Add(f.CType.Apply(this, stream));
            }
            catch (DataCreateException dce)
            {
                dce.Push(bean, f);
                throw;
            }
        }
        return list;
    }

    public DType Accept(TBean type, LiteStream x)
    {
        var originBean = type.DefBean;
        if (type.IsNullable)
        {
            if (!x.IsBeginOfStructOrCollection())
            {
                string subType = x.ReadData().ToLower().Trim();
                if (subType == FieldNames.BeanNullType)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"type:'{originBean.FullName}'的值只能为 `null` 或 `{{,,...}}` 格式");
                }
            }
        }
        x.ReadStructOrCollectionBegin();

        if (originBean.IsAbstractType)
        {
            string subType = x.ReadData();
            if (subType.ToLower().Trim() == FieldNames.BeanNullType)
            {
                if (!type.IsNullable)
                {
                    throw new InvalidExcelDataException($"type:{originBean.FullName}不是可空类型. 不能为空");
                }
                return null;
            }
            DefBean implType = DataUtil.GetImplTypeByNameOrAlias(originBean, subType);
            var fields = CreateBeanFields(implType, x);
            x.ReadStructOrCollectionEnd();
            return new DBean(type, implType, fields);
           
        }
        else
        {
            var fields = CreateBeanFields(originBean, x);
            x.ReadStructOrCollectionEnd();
            return new DBean(type, originBean, fields);
        }
    }

    // 容器类统统不支持 type.IsNullable
    // 因为貌似没意义？
    public List<DType> ReadList(TType type, TType eleType, LiteStream stream)
    {
        stream.ReadStructOrCollectionBegin();
        var datas = new List<DType>();
        while (!stream.IsEndOfStructOrCollection())
        {
            datas.Add(eleType.Apply(this, stream));
        }
        stream.ReadStructOrCollectionEnd();
        return datas;
    }

    public DType Accept(TArray type, LiteStream x)
    {
        return new DArray(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TList type, LiteStream x)
    {
        return new DList(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TSet type, LiteStream x)
    {
        return new DSet(type, ReadList(type, type.ElementType, x));
    }

    public DType Accept(TMap type, LiteStream stream)
    {
        stream.ReadStructOrCollectionBegin();
        var datas = new Dictionary<DType, DType>();
        while (!stream.IsEndOfStructOrCollection())
        {
            stream.ReadStructOrCollectionBegin();
            var key = type.KeyType.Apply(this, stream);
            var value = type.ValueType.Apply(this, stream);
            stream.ReadStructOrCollectionEnd();
            if (!datas.TryAdd(key, value))
            {
                throw new InvalidExcelDataException($"map 的 key:{key} 重复");
            }
        }
        stream.ReadStructOrCollectionEnd();
        return new DMap(type, datas);
    }
}
