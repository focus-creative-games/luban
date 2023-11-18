using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class EditorDeclaringTypeNameVisitor : DeclaringTypeNameVisitor
{
    public new static EditorDeclaringTypeNameVisitor Ins { get; } = new();

    protected override ITypeFuncVisitor<string> UnderlyingVisitor => EditorUnderlyingTypeNameVisitor.Ins;

    public override string Accept(TDateTime type)
    {
        return "string";
    }
}
