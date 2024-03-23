using Luban.Cpp.TemplateExtensions;
using Luban.Types;

namespace Luban.Cpp.TypeVisitors;

public class CppSharedptrUnderlyingDeclaringTypeNameVisitor : CppUnderlyingDeclaringTypeNameVisitor
{
    public new static CppSharedptrUnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public override string Accept(TBean type)
    {
        return $"::luban::SharedPtr<{CppTemplateExtension.MakeTypeCppName(type.DefBean)}>";
    }
}
