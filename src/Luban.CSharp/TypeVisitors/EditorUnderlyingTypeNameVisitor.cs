using Luban.Types;

namespace Luban.CSharp.TypeVisitors;

public class EditorUnderlyingTypeNameVisitor : UnderlyingDeclaringTypeNameVisitor
{
    public new static EditorUnderlyingTypeNameVisitor Ins { get; } = new();

    public override string Accept(TDateTime type)
    {
        return "string";
    }
}
