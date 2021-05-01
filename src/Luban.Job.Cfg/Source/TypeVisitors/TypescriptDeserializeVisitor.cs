using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class TypescriptDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TypescriptDeserializeVisitor Ins { get; } = new TypescriptDeserializeVisitor();

        public override string DoAccept(TType type, string jsonFieldName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({jsonFieldName} != null) {{ {type.Apply(TypescriptUnderingDeserializeVisitor.Ins, jsonFieldName, fieldName)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptUnderingDeserializeVisitor.Ins, jsonFieldName, fieldName);
            }
        }

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(TypescriptUnderingDeserializeVisitor.Ins, bufName, fieldName);
        }
    }
}
