using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    public class GoSerializeBinVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static GoSerializeBinVisitor Ins { get; } = new();

        public override string DoAccept(TType type, string fieldName, string bufName)
        {
            if (type.IsNullable)
            {
                return $"if {bufName} != nil {{ {bufName}.WriteBool(true); {type.Apply(GoSerializeUnderingVisitor.Ins, (type.Apply(GoIsPointerTypeVisitor.Ins) ? $"*{fieldName}" : fieldName), bufName)}  }} else {{ {bufName}.WriteBool(false) }}";
            }
            else
            {
                return type.Apply(GoSerializeUnderingVisitor.Ins, fieldName, bufName);
            }
        }
    }
}
