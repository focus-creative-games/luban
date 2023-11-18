using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class ConstValueVisitor : ITypeFuncVisitor<string, string>
{
    public static ConstValueVisitor Ins { get; } = new();

    public string Accept(TBool type, string x)
    {
        return x.ToLower();
    }

    public string Accept(TByte type, string x)
    {
        return x;
    }

    public string Accept(TShort type, string x)
    {
        return x;
    }

    public string Accept(TInt type, string x)
    {
        return x;
    }

    public virtual string Accept(TLong type, string x)
    {
        return x + "L";
    }

    public virtual string Accept(TFloat type, string x)
    {
        return x + "f";
    }

    public string Accept(TDouble type, string x)
    {
        return x;
    }

    public string Accept(TEnum type, string x)
    {
        return x;
    }

    public string Accept(TString type, string x)
    {
        return "\"" + x + "\"";
    }

    public string Accept(TDateTime type, string x)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type, string x)
    {
        throw new NotImplementedException();
    }

    public string Accept(TArray type, string x)
    {
        throw new NotImplementedException();
    }

    public string Accept(TList type, string x)
    {
        throw new NotImplementedException();
    }

    public string Accept(TSet type, string x)
    {
        throw new NotImplementedException();
    }

    public string Accept(TMap type, string x)
    {
        throw new NotImplementedException();
    }
}
