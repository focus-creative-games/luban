using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class JavaSerializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static JavaSerializeVisitor Ins { get; } = new JavaSerializeVisitor();

        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"{{ if ({fieldName} != null){{ {bufName}.writeBool(true); {type.Apply(JavaUnderingSerializeVisitor.Ins, bufName, fieldName)} }} else {{ {bufName}.writeBool(false); }} }}";
            }
            else
            {
                return type.Apply(JavaUnderingSerializeVisitor.Ins, bufName, fieldName);
            }
        }
    }
}
