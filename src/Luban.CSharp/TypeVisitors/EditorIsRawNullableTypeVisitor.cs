using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class EditorIsRawNullableTypeVisitor : IsRawNullableTypeVisitor
{
    public new static EditorIsRawNullableTypeVisitor Ins { get; } = new();

    public override bool Accept(TBean type)
    {
        return true;
    }

    public override bool Accept(TDateTime type)
    {
        return true;
    }
}
