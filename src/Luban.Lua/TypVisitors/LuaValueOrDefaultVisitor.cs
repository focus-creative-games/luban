using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class LuaValueOrDefaultVisitor : ITypeFuncVisitor<string, string>
{
    public static LuaValueOrDefaultVisitor Ins { get; } = new();

    public string Accept(TBool type, string x)
    {
        return $"{x} == true";
    }

    public string Accept(TByte type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TShort type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TInt type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TLong type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TFloat type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TDouble type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TEnum type, string x)
    {
        return $"{x} or 0";
    }

    public string Accept(TString type, string x)
    {
        return $"{x} or \"\"";
    }

    public string Accept(TBean type, string x)
    {
        return $"{x} or {{}}";
    }

    public string Accept(TArray type, string x)
    {
        return $"{x} or {{}}";
    }

    public string Accept(TList type, string x)
    {
        return $"{x} or {{}}";
    }

    public string Accept(TSet type, string x)
    {
        return $"{x} or {{}}";
    }

    public string Accept(TMap type, string x)
    {
        return $"{x} or {{}}";
    }

    public string Accept(TDateTime type, string x)
    {
        return $"{x} or 0";
    }
}
