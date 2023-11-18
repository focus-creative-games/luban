using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Lua.TypVisitors;

public class LuaDeserializeMethodNameVisitor : ITypeFuncVisitor<string>
{
    public static LuaDeserializeMethodNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "readBool";
    }

    public string Accept(TByte type)
    {
        return "readByte";
    }

    public string Accept(TShort type)
    {
        return "readShort";
    }

    public string Accept(TInt type)
    {
        return "readInt";
    }

    public string Accept(TLong type)
    {
        return "readLong";
    }

    public string Accept(TFloat type)
    {
        return "readFloat";
    }

    public string Accept(TDouble type)
    {
        return "readDouble";
    }

    public string Accept(TEnum type)
    {
        return "readInt";
    }

    public string Accept(TString type)
    {
        return "readString";
    }

    public string Accept(TBean type)
    {
        return $"beans['{type.DefBean.FullName}']._deserialize";
    }

    public string Accept(TArray type)
    {
        return "readList";
    }

    public string Accept(TList type)
    {
        return "readList";
    }

    public string Accept(TSet type)
    {
        return "readSet";
    }

    public string Accept(TMap type)
    {
        return "readMap";
    }

    public string Accept(TDateTime type)
    {
        return "readLong";
    }
}
