namespace Luban.Any.TypeVisitors;

class TypescriptBinConstructorVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static TypescriptBinConstructorVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string byteBufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({byteBufName}.ReadBool()) {{ {type.Apply(TypescriptBinUnderingConstructorVisitor.Ins, byteBufName, fieldName)} }} else {{ {fieldName} = undefined }}";
        }
        else
        {
            return type.Apply(TypescriptBinUnderingConstructorVisitor.Ins, byteBufName, fieldName);
        }
    }
}