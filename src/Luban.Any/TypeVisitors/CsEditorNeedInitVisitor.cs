namespace Luban.Any.TypeVisitors;

public class CsEditorNeedInitVisitor : CsNeedInitVisitor
{
    public static new CsEditorNeedInitVisitor Ins { get; } = new CsEditorNeedInitVisitor();

    public override bool Accept(TEnum type)
    {
        return true;
    }

    public override bool Accept(TDateTime type)
    {
        return true;
    }
}