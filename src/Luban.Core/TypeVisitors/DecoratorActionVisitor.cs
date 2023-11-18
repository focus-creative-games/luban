using Luban.Types;

namespace Luban.TypeVisitors;

public abstract class DecoratorActionVisitor<T> : ITypeActionVisitor<T>
{
    public abstract void DoAccept(TType type, T x);

    public virtual void Accept(TBool type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TByte type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TShort type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TInt type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TLong type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TFloat type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TDouble type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TEnum type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TString type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TDateTime type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TBean type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TArray type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TList type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TSet type, T x)
    {
        DoAccept(type, x);
    }

    public virtual void Accept(TMap type, T x)
    {
        DoAccept(type, x);
    }

}

public abstract class DecoratorActionVisitor<T1, T2> : ITypeActionVisitor<T1, T2>
{

    public abstract void DoAccept(TType type, T1 x, T2 y);

    public virtual void Accept(TBool type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TByte type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TShort type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TInt type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TLong type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TFloat type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TDouble type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TEnum type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TString type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TDateTime type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TBean type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TArray type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TList type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TSet type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }

    public virtual void Accept(TMap type, T1 x, T2 y)
    {
        DoAccept(type, x, y);
    }
}
