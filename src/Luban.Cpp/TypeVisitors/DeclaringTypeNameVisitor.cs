using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class DeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new DeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        return type.IsNullable ? $"::luban::SharedPtr<{type.Apply(CppSharedPtrUnderlyingDeclaringTypeNameVisitor.Ins)}>" : type.Apply(CppSharedPtrUnderlyingDeclaringTypeNameVisitor.Ins);
    }

    public override string Accept(TBean type)
    {
        return type.Apply(CppSharedPtrUnderlyingDeclaringTypeNameVisitor.Ins);
    }
}