using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static CsDeserializeVisitor Ins { get; } = new CsDeserializeVisitor();
        public override string DoAccept(TType type, string bufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({bufName}.ReadBool()){{ {type.Apply(CsUnderingDeserializeVisitor.Ins, bufName, fieldName, 0)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(CsUnderingDeserializeVisitor.Ins, bufName, fieldName, 0);
            }
        }

        //public override string Accept(TBean type, string bufName, string fieldName)
        //{
        //    return type.Apply(CsUnderingDeserializeVisitor.Ins, bufName, fieldName);
        //}
    }
}
