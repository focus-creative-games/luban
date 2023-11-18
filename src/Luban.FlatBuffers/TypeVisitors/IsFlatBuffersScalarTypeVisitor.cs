using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.FlatBuffers.TypeVisitors;

public class IsFlatBuffersScalarTypeVisitor : AllTrueVisitor
{
    public static IsFlatBuffersScalarTypeVisitor Ins { get; } = new();

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
