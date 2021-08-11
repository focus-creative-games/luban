using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CppDefineTypeName : DecoratorFuncVisitor<string>
    {
        public static CppDefineTypeName Ins { get; } = new CppDefineTypeName();

        public override string DoAccept(TType type)
        {
            //return type.IsNullable ? $"std::optional<{type.Apply(CppUnderingDefineTypeName.Ins)}>" : type.Apply(CppUnderingDefineTypeName.Ins);
            return type.Apply(CppUnderingDefineTypeName.Ins) + (type.IsNullable ? "*" : "");
        }

        public override string Accept(TBean type)
        {
            return type.Apply(CppUnderingDefineTypeName.Ins);
        }
    }
}
