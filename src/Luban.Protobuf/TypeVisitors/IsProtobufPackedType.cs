using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Protobuf.TypeVisitors;

public class IsProtobufPackedType : AllTrueVisitor
{
    public static IsProtobufPackedType Ins { get; } = new();


    public override bool Accept(TString type)
    {
        return false;
    }

    public override bool Accept(TEnum type)
    {
        return false;
    }

    public override bool Accept(TBean type)
    {
        return false;
    }

    public override bool Accept(TArray type)
    {
        return false;
    }

    public override bool Accept(TList type)
    {
        return false;
    }

    public override bool Accept(TSet type)
    {
        return false;
    }

    public override bool Accept(TMap type)
    {
        return false;
    }
}
