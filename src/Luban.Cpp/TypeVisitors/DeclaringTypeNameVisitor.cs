using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class DeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static DeclaringTypeNameVisitor Ins { get; } = new DeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        if (type.IsNullable)
            return $"::luban::SharedPtr<{type.Apply(CppSmartPtrUnderlyingDeclaringTypeNameVisitor.Ins)}>";
        
        return type.Apply(CppSmartPtrUnderlyingDeclaringTypeNameVisitor.Ins);
    }
}
