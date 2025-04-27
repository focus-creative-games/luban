using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.DataLoader.Builtin.DataVisitors;

class StringDataCreator : ITypeFuncVisitor<string, DType>
{
    public static StringDataCreator Ins { get; } = new();

    public DType Accept(TBool type, string x)
    {
        if (bool.TryParse(x, out var b))
        {
            return DBool.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是bool类型");
        }
    }

    public DType Accept(TByte type, string x)
    {
        if (byte.TryParse(x, out var b))
        {
            return DByte.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是byte类型");
        }
    }

    public DType Accept(TShort type, string x)
    {
        if (short.TryParse(x, out var b))
        {
            return DShort.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是short类型");
        }
    }

    public DType Accept(TInt type, string x)
    {
        if (int.TryParse(x, out var b))
        {
            return DInt.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是int类型");
        }
    }

    public DType Accept(TLong type, string x)
    {
        if (long.TryParse(x, out var b))
        {
            return DLong.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是long类型");
        }
    }

    public DType Accept(TFloat type, string x)
    {
        if (float.TryParse(x, out var b))
        {
            return DFloat.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是float类型");
        }
    }

    public DType Accept(TDouble type, string x)
    {
        if (double.TryParse(x, out var b))
        {
            return DDouble.ValueOf(b);
        }
        else
        {
            throw new Exception($"{x} 不是double类型");
        }
    }

    public DType Accept(TEnum type, string x)
    {
        return new DEnum(type, x);
    }

    public DType Accept(TString type, string x)
    {
        x = DataUtil.RemoveStringQuote(x);
        return DString.ValueOf(type, x);
    }

    public DType Accept(TBean type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TArray type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TList type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TSet type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TMap type, string x)
    {
        throw new NotSupportedException();
    }

    public DType Accept(TDateTime type, string x)
    {
        throw new NotSupportedException();
    }
}
