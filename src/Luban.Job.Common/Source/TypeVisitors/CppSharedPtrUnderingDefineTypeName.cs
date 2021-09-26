using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CppSharedPtrUnderingDefineTypeName : CppRawUnderingDefineTypeName
    {
        public new static CppSharedPtrUnderingDefineTypeName Ins { get; } = new();

        public override string Accept(TBean type)
        {
            return $"::bright::SharedPtr<{type.Bean.CppFullName}>";
        }
    }
}
