using Luban.Cpp.TemplateExtensions;
using Luban.Types;

namespace Luban.Cpp.TypeVisitors;

public class CppSmartPtrUnderlyingDeclaringTypeNameVisitor : CppUnderlyingDeclaringTypeNameVisitor
{
    public new static CppSmartPtrUnderlyingDeclaringTypeNameVisitor Ins { get; } = new();
    
    public override string Accept(TBean type)
    {
        if (type.IsDynamic || type.IsNullable)
            return $"::luban::SharedPtr<{CppTemplateExtension.MakeTypeCppName(type.DefBean)}>";
        
        return  CppTemplateExtension.MakeTypeCppName(type.DefBean);
    }
}
