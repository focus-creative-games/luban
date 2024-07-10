using Luban.Types;

namespace Luban.Typescript.TypeVisitors
{
    public class TypescriptBinUnderingDeserializeVisitor : TypescriptBinUnderingDeserializeVisitorBase
    {
        public static TypescriptBinUnderingDeserializeVisitor Ins { get; } = new TypescriptBinUnderingDeserializeVisitor();

        public override string Accept(TBean type, string bufVarName, string fieldName)
        {
            if (type.DefBean.IsAbstractType)
            {
                return $"{fieldName} = {type.DefBean.FullName}.constructorFrom({bufVarName})";
            }
            else
            {
                return $"{fieldName} = new {type.DefBean.FullName}({bufVarName})";
            }
        }
    }
}
