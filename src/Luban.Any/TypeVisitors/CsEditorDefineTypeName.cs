namespace Luban.Any.TypeVisitors;

public class CsEditorDefineTypeName : CsDefineTypeName
{
    public static new CsEditorDefineTypeName Ins { get; } = new CsEditorDefineTypeName();

    protected override ITypeFuncVisitor<string> UnderlyingVisitor => CsEditorUnderlyingDefineTypeName.Ins;

    public override string Accept(TDateTime type)
    {
        return "string";
    }

    public override string Accept(TText type)
    {
        return CfgConstStrings.EditorTextTypeName;
    }
}