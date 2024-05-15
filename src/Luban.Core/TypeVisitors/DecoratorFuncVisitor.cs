using Luban.Types;

namespace Luban.TypeVisitors;

public abstract class DecoratorFuncVisitor<TR> : ITypeFuncVisitor<TR>
{
    public abstract TR DoAccept(TType type);

    public virtual TR Accept(TBool type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TByte type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TShort type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TInt type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TLong type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TFloat type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TDouble type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TEnum type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TString type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TDateTime type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TBean type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TArray type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TList type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TSet type)
    {
        return DoAccept(type);
    }

    public virtual TR Accept(TMap type)
    {
        return DoAccept(type);
    }
}

public abstract class DecoratorFuncVisitor<T1, TR> : ITypeFuncVisitor<T1, TR>
{
    public abstract TR DoAccept(TType tpye, T1 x);

    public virtual TR Accept(TBool type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TByte type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TShort type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TInt type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TLong type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TFloat type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TDouble type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TEnum type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TString type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TDateTime type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TBean type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TArray type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TList type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TSet type, T1 x)
    {
        return DoAccept(type, x);
    }

    public virtual TR Accept(TMap type, T1 x)
    {
        return DoAccept(type, x);
    }
}

public abstract class DecoratorFuncVisitor<T1, T2, TR> : ITypeFuncVisitor<T1, T2, TR>
{
    public abstract TR DoAccept(TType tpye, T1 x, T2 y);

    public virtual TR Accept(TBool type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TByte type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TShort type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TInt type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TLong type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TFloat type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TDouble type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TEnum type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TString type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TDateTime type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TBean type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TArray type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TList type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TSet type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }

    public virtual TR Accept(TMap type, T1 x, T2 y)
    {
        return DoAccept(type, x, y);
    }
}

public abstract class DecoratorFuncVisitor<T1, T2, T3, TR> : ITypeFuncVisitor<T1, T2, T3, TR>
{

    public abstract TR DoAccept(TType tpye, T1 x, T2 y, T3 z);

    public virtual TR Accept(TBool type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TByte type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TShort type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TInt type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TLong type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TFloat type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TDouble type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TEnum type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TString type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TDateTime type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TBean type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TArray type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TList type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TSet type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

    public virtual TR Accept(TMap type, T1 x, T2 y, T3 z)
    {
        return DoAccept(type, x, y, z);
    }

}

public abstract class DecoratorFuncVisitor<T1, T2, T3, T4, TR> : ITypeFuncVisitor<T1, T2, T3, T4, TR>
{

    public abstract TR DoAccept(TType tpye, T1 a, T2 b, T3 c, T4 d);

    public virtual TR Accept(TBool type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TByte type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TShort type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TInt type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TLong type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TFloat type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TDouble type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TEnum type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TString type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TDateTime type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TBean type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TArray type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TList type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TSet type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

    public virtual TR Accept(TMap type, T1 a, T2 b, T3 c, T4 d)
    {
        return DoAccept(type, a, b, c, d);
    }

}
