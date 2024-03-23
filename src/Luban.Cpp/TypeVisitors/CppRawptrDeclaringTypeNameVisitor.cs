using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppRawptrDeclaringTypeNameVisitor : DecoratorFuncVisitor<string>
{
    public static CppRawptrDeclaringTypeNameVisitor Ins { get; } = new CppRawptrDeclaringTypeNameVisitor();

    public override string DoAccept(TType type)
    {
        return type.IsNullable && !type.IsBean ? $"{type.Apply(CppRawptrUnderlyingDeclaringTypeNameVisitor.Ins)}*" : type.Apply(CppRawptrUnderlyingDeclaringTypeNameVisitor.Ins);
    }
}
