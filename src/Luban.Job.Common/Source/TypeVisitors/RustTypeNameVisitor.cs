using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class RustTypeNameVisitor : DecoratorFuncVisitor<string>
    {
        public static RustTypeNameVisitor Ins { get; } = new();

        public override string DoAccept(TType type)
        {
            if (type.IsNullable)
            {
                if (type.IsBean)
                {
                    return $"std::option::Option<std::rc::Rc<{type.Apply(RustTypeUnderlyingNameVisitor.Ins)}>>";
                }
                else
                {
                    return $"std::option::Option<{type.Apply(RustTypeUnderlyingNameVisitor.Ins)}>";
                }
            }
            else
            {
                if (type.IsBean)
                {
                    return $"std::rc::Rc<{type.Apply(RustTypeUnderlyingNameVisitor.Ins)}>";
                }
                else
                {
                    return type.Apply(RustTypeUnderlyingNameVisitor.Ins);
                }
            }
        }
    }
}
