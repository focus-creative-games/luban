using Luban.Javascript.TemplateExtensions;
using Luban.Types;

namespace Luban.Javascript.TypeVisitors;

public class BinUnderlyingDeserializeVisitor : BinUnderingDeserializeVisitorBase
{
    public static BinUnderlyingDeserializeVisitor Ins { get; } = new BinUnderlyingDeserializeVisitor();

    public override string Accept(TBean type, string bufVarName, string fieldName, int depth)
    {
        string fullName = JavascriptCommonTemplateExtension.FullName(type.DefBean);
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {fullName}.constructorFrom({bufVarName})";
        }
        else
        {
            return $"{fieldName} = new {fullName}({bufVarName})";
        }
    }
}
