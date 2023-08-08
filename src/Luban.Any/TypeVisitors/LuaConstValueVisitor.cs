namespace Luban.Any.TypeVisitors;

public class LuaConstValueVisitor : CsConstValueVisitor
{
    public static new LuaConstValueVisitor Ins { get; } = new();

    public override string Accept(TLong type, string x)
    {
        return x;
    }

    public override string Accept(TFlong type, string x)
    {
        return x;
    }

    public override string Accept(TFloat type, string x)
    {
        return x;
    }
}