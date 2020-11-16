using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Proto.TypeVisitors
{
    class CsSerializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static CsSerializeVisitor Ins { get; } = new CsSerializeVisitor();

        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({fieldName} != null){{ {bufName}.WriteBool(true); {type.Apply(CsUnderingSerializeVisitor.Ins, bufName, fieldName)} }} else {{ {bufName}.WriteBool(true); }}";
            }
            else
            {
                return type.Apply(CsUnderingSerializeVisitor.Ins, bufName, fieldName);
            }
        }

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(CsUnderingSerializeVisitor.Ins, bufName, fieldName);
        }
    }
}
