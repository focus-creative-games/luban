using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class TsDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TsDeserializeVisitor Ins { get; } = new TsDeserializeVisitor();

        public override string DoAccept(TType type, string jsonFieldName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({jsonFieldName} != null) {{ {type.Apply(TsUnderingDeserializeVisitor.Ins, jsonFieldName, fieldName)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TsUnderingDeserializeVisitor.Ins, jsonFieldName, fieldName);
            }
        }

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(TsUnderingDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}
