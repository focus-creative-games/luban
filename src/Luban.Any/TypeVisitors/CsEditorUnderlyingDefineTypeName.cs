namespace Luban.Any.TypeVisitors;

public class CsEditorUnderlyingDefineTypeName : CsUnderingDefineTypeName
{
    public static new CsEditorUnderlyingDefineTypeName Ins { get; } = new CsEditorUnderlyingDefineTypeName();

    public override string Accept(TText type)
    {
        return CfgConstStrings.EditorTextTypeName;
    }

    public override string Accept(TDateTime type)
    {
        return "string";
    }
}