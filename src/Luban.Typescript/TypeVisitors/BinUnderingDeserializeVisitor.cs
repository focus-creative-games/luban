using Luban.Types;

namespace Luban.Typescript.TypeVisitors
{
    public class BinUnderingDeserializeVisitor : BinUnderingDeserializeVisitorBase
    {
        public static BinUnderingDeserializeVisitor Ins { get; } = new BinUnderingDeserializeVisitor();

        public override string Accept(TBean type, string bufVarName, string fieldName, int depth)
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
