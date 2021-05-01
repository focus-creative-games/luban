using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    class TypescriptBinUnderingConstructorVisitor : TypescriptBinUnderingDeserializeVisitorBase
    {
        public static TypescriptBinUnderingConstructorVisitor Ins { get; } = new TypescriptBinUnderingConstructorVisitor();

        public override string Accept(TBean type, string bufVarName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = {type.Bean.FullName}.constructorFrom({bufVarName});";
            }
            else
            {
                return $"{fieldName} = new {type.Bean.FullName}({bufVarName});";
            }
        }
    }
}
