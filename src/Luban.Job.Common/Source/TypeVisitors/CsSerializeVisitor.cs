using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsSerializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static CsSerializeVisitor Ins { get; } = new CsSerializeVisitor();

        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"{{ if({fieldName} is {type.Apply(CsUnderingDefineTypeName.Ins)} __value__){{ {bufName}.WriteBool(true); {type.Apply(CsUnderingSerializeVisitor.Ins, bufName, "__value__" )} }} else {{ {bufName}.WriteBool(false); }} }}";
            }
            else
            {
                return type.Apply(CsUnderingSerializeVisitor.Ins, bufName, fieldName);
            }
        }
    }
}
