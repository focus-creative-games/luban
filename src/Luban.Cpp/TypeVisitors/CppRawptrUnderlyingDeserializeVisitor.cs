using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppRawptrUnderlyingDeserializeVisitor : CppUnderlyingDeserializeVisitorBase
{
    public static CppRawptrUnderlyingDeserializeVisitor Ins { get; } = new();
}
