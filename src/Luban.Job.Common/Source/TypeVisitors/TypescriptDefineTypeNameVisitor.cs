using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class TypescriptDefineTypeNameVisitor : DecoratorFuncVisitor<string>
    {
        public static TypescriptDefineTypeNameVisitor Ins { get; } = new TypescriptDefineTypeNameVisitor();

        public override string DoAccept(TType type)
        {
            return type.IsNullable ? $"{type.Apply(TypescriptDefineTypeNameUnderlyingVisitor.Ins)}|undefined" : type.Apply(TypescriptDefineTypeNameUnderlyingVisitor.Ins);
        }
    }
}
