namespace Luban.Any.TypeVisitors;

public class TypescriptDefineTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static TypescriptDefineTypeNameVisitor Ins { get; } = new();

    public override string DoAccept(TType type)
    {
        return type.IsNullable ? $"{type.Apply(TypescriptDefineTypeNameUnderlyingVisitor.Ins)}|undefined" : type.Apply(TypescriptDefineTypeNameUnderlyingVisitor.Ins);
    }
}