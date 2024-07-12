using Luban.Types;
using Luban.Typescript.TypeVisitors;
using Luban.TypeVisitors;

namespace Luban.Typescript.TemplateExtensions
{
    class TypescriptBinDeserializeVisitor : DecoratorFuncVisitor<string, string, int, string>
    {
        public static TypescriptBinDeserializeVisitor Ins { get; } = new TypescriptBinDeserializeVisitor();

        public override string DoAccept(TType type, string byteBufName, string fieldName, int depth)
        {
            if (type.IsNullable)
            {
                return $"if({byteBufName}.ReadBool()) {{ {type.Apply(TypescriptBinUnderingDeserializeVisitor.Ins, byteBufName, fieldName, depth)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptBinUnderingDeserializeVisitor.Ins, byteBufName, fieldName, depth);
            }
        }
    }
}
