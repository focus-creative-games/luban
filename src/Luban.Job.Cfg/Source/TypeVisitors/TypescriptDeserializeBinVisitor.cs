using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class TypescriptDeserializeBinVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TypescriptDeserializeBinVisitor Ins { get; } = new TypescriptDeserializeBinVisitor();

        public override string DoAccept(TType type, string byteBufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({byteBufName}.ReadBool()) {{ {type.Apply(TypescriptUnderingDeserializeBinVisitor.Ins, byteBufName, fieldName)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptUnderingDeserializeBinVisitor.Ins, byteBufName, fieldName);
            }
        }

        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(TypescriptUnderingDeserializeBinVisitor.Ins, bufName, fieldName);
        }
    }
}
