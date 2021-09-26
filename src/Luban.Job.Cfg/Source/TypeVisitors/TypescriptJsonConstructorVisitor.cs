using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class TypescriptJsonConstructorVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TypescriptJsonConstructorVisitor Ins { get; } = new TypescriptJsonConstructorVisitor();

        public override string DoAccept(TType type, string jsonFieldName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({jsonFieldName} != undefined) {{ {type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, jsonFieldName, fieldName)} }} else {{ {fieldName} = undefined }}";
            }
            else
            {
                return type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, jsonFieldName, fieldName);
            }
        }

        //public override string Accept(TBean type, string bytebufName, string fieldName)
        //{
        //    return type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, bytebufName, fieldName);
        //}
    }
}
