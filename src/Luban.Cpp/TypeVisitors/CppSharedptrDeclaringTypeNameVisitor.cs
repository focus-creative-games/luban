using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppSharedptrDeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static CppSharedptrDeclaringTypeNameVisitor Ins { get; } = new CppSharedptrDeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        return type.IsNullable ? $"::luban::SharedPtr<{type.Apply(CppSharedptrUnderlyingDeclaringTypeNameVisitor.Ins)}>" : type.Apply(CppSharedptrUnderlyingDeclaringTypeNameVisitor.Ins);
    }

    public override string Accept(TBean type)
    {
        return type.Apply(CppSharedptrUnderlyingDeclaringTypeNameVisitor.Ins);
    }
}
