using Luban.Types;

namespace Luban.CSharp.TypeVisitors;

public class EditorNeedInitVisitor : NeedInitFieldVisitor
{
    public new static EditorNeedInitVisitor Ins { get; } = new();

    public override bool Accept(TEnum type)
    {
        return true;
    }

    public override bool Accept(TDateTime type)
    {
        return true;
    }
}
