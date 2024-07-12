using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors
{
    class TypescriptBinSerializeVisitor : DecoratorFuncVisitor<string, string, int, string>
    {
        public static TypescriptBinSerializeVisitor Ins { get; } = new TypescriptBinSerializeVisitor();

        public override string DoAccept(TType type, string bytebufName, string fieldName, int depth)
        {
            if (type.IsNullable)
            {
                return $"if({bytebufName}.ReadBool()) {{{type.Apply(TypescriptBinUnderingSerializeVisitor.Ins, bytebufName, fieldName, depth)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptBinUnderingSerializeVisitor.Ins, bytebufName, fieldName, depth);
            }
        }
    }
}
