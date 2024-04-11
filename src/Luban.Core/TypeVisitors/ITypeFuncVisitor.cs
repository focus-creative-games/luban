using Luban.Types;

namespace Luban.TypeVisitors;

public interface ITypeFuncVisitor<TR>
{
    TR Accept(TBool type);

    TR Accept(TByte type);

    TR Accept(TShort type);

    TR Accept(TInt type);

    TR Accept(TLong type);

    TR Accept(TFloat type);

    TR Accept(TDouble type);

    TR Accept(TEnum type);

    TR Accept(TString type);

    TR Accept(TDateTime type);

    TR Accept(TBean type);

    TR Accept(TArray type);

    TR Accept(TList type);

    TR Accept(TSet type);

    TR Accept(TMap type);
}

public interface ITypeFuncVisitor<T, TR>
{
    TR Accept(TBool type, T x);

    TR Accept(TByte type, T x);

    TR Accept(TShort type, T x);

    TR Accept(TInt type, T x);

    TR Accept(TLong type, T x);

    TR Accept(TFloat type, T x);

    TR Accept(TDouble type, T x);

    TR Accept(TEnum type, T x);

    TR Accept(TString type, T x);

    TR Accept(TDateTime type, T x);

    TR Accept(TBean type, T x);

    TR Accept(TArray type, T x);

    TR Accept(TList type, T x);

    TR Accept(TSet type, T x);

    TR Accept(TMap type, T x);
}

public interface ITypeFuncVisitor<T, T2, TR>
{
    TR Accept(TBool type, T x, T2 y);

    TR Accept(TByte type, T x, T2 y);

    TR Accept(TShort type, T x, T2 y);

    TR Accept(TInt type, T x, T2 y);

    TR Accept(TLong type, T x, T2 y);

    TR Accept(TFloat type, T x, T2 y);

    TR Accept(TDouble type, T x, T2 y);

    TR Accept(TEnum type, T x, T2 y);

    TR Accept(TString type, T x, T2 y);

    TR Accept(TDateTime type, T x, T2 y);

    TR Accept(TBean type, T x, T2 y);

    TR Accept(TArray type, T x, T2 y);

    TR Accept(TList type, T x, T2 y);

    TR Accept(TSet type, T x, T2 y);

    TR Accept(TMap type, T x, T2 y);
}

public interface ITypeFuncVisitor<T, T2, T3, TR>
{
    TR Accept(TBool type, T x, T2 y, T3 z);

    TR Accept(TByte type, T x, T2 y, T3 z);

    TR Accept(TShort type, T x, T2 y, T3 z);

    TR Accept(TInt type, T x, T2 y, T3 z);

    TR Accept(TLong type, T x, T2 y, T3 z);

    TR Accept(TFloat type, T x, T2 y, T3 z);

    TR Accept(TDouble type, T x, T2 y, T3 z);

    TR Accept(TEnum type, T x, T2 y, T3 z);

    TR Accept(TString type, T x, T2 y, T3 z);

    TR Accept(TDateTime type, T x, T2 y, T3 z);

    TR Accept(TBean type, T x, T2 y, T3 z);

    TR Accept(TArray type, T x, T2 y, T3 z);

    TR Accept(TList type, T x, T2 y, T3 z);

    TR Accept(TSet type, T x, T2 y, T3 z);

    TR Accept(TMap type, T x, T2 y, T3 z);
}

public interface ITypeFuncVisitor<T, T2, T3, T4, TR>
{
    TR Accept(TBool type, T x, T2 y, T3 z, T4 w);

    TR Accept(TByte type, T x, T2 y, T3 z, T4 w);

    TR Accept(TShort type, T x, T2 y, T3 z, T4 w);

    TR Accept(TInt type, T x, T2 y, T3 z, T4 w);

    TR Accept(TLong type, T x, T2 y, T3 z, T4 w);

    TR Accept(TFloat type, T x, T2 y, T3 z, T4 w);

    TR Accept(TDouble type, T x, T2 y, T3 z, T4 w);

    TR Accept(TEnum type, T x, T2 y, T3 z, T4 w);

    TR Accept(TString type, T x, T2 y, T3 z, T4 w);

    TR Accept(TDateTime type, T x, T2 y, T3 z, T4 w);

    TR Accept(TBean type, T x, T2 y, T3 z, T4 w);

    TR Accept(TArray type, T x, T2 y, T3 z, T4 w);

    TR Accept(TList type, T x, T2 y, T3 z, T4 w);

    TR Accept(TSet type, T x, T2 y, T3 z, T4 w);

    TR Accept(TMap type, T x, T2 y, T3 z, T4 w);
}
