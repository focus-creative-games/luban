

using Luban.Types;

namespace Luban.Dart.TypeVisitors;

class DartDeclaringBoxTypeNameVisitor: DartDeclaringTypeNameVisitor
{
    public new static DartDeclaringBoxTypeNameVisitor Ins { get; } = new();

    public override string Accept(TBool type)
    {
        return  "bool";
    }

    public override string Accept(TByte type)
    {
        return  "int";
    }

    public override string Accept(TShort type)
    {
        return  "int";
    }

    public override string Accept(TInt type)
    {
        return "int";
    }

    public override string Accept(TLong type)
    {
        return "int";
    }

    public override string Accept(TFloat type)
    {
        return "double";
    }

    public override string Accept(TDouble type)
    {
        return "double";
    }
}
