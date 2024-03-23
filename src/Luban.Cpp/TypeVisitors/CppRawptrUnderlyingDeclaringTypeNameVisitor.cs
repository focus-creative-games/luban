using Luban.Cpp.TemplateExtensions;
using Luban.Types;

namespace Luban.Cpp.TypeVisitors;

public class CppRawptrUnderlyingDeclaringTypeNameVisitor : CppUnderlyingDeclaringTypeNameVisitor
{
    public new static CppRawptrUnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public override string Accept(TBean type)
    {
        string typeName = CppTemplateExtension.MakeTypeCppName(type.DefBean);
        return $"{typeName}*";
    }
}
