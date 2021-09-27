using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class RustJsonConstructorVisitor : DecoratorFuncVisitor<string, string>
    {
        public static RustJsonConstructorVisitor Ins { get; } = new();

        public override string DoAccept(TType type, string jsonFieldName)
        {
            if (type.IsNullable)
            {
                return $"if !{jsonFieldName}.is_null() {{ Some({type.Apply(RustJsonUnderingConstructorVisitor.Ins, jsonFieldName)}) }} else {{ None }}";
            }
            else
            {
                return type.Apply(RustJsonUnderingConstructorVisitor.Ins, jsonFieldName);
            }
        }

        //public override string Accept(TBean type, string bytebufName, string fieldName)
        //{
        //    return type.Apply(TypescriptJsonUnderingConstructorVisitor.Ins, bytebufName, fieldName);
        //}
    }
}
