using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    class TypescriptBinConstructorVisitor : DecoratorFuncVisitor<string, string, string>
    {
        public static TypescriptBinConstructorVisitor Ins { get; } = new TypescriptBinConstructorVisitor();

        public override string DoAccept(TType type, string byteBufName, string fieldName)
        {
            if (type.IsNullable)
            {
                return $"if({byteBufName}.ReadBool()) {{ {type.Apply(TypescriptBinUnderingConstructorVisitor.Ins, byteBufName, fieldName)} }} else {{ {fieldName} = null; }}";
            }
            else
            {
                return type.Apply(TypescriptBinUnderingConstructorVisitor.Ins, byteBufName, fieldName);
            }
        }

        // TODO 设计需要简化，现在造成多态bean的可空与其他字段类型不一样，而需要单独处理
        // 多态bean不浪费一个字段，直接用typeid==0表示空
        // 因此不跟普通字段一样，需要 ReadBool()来区别是否为空
        public override string Accept(TBean type, string bufName, string fieldName)
        {
            return type.Apply(TypescriptBinUnderingConstructorVisitor.Ins, bufName, fieldName);
        }
    }
}
