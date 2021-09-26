using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class CompatibleSerializeNeedEmbedVisitor : AllFalseVisitor
    {
        public static CompatibleSerializeNeedEmbedVisitor Ins { get; } = new();

        public override bool Accept(TBean type)
        {
            return true;
        }

        public override bool Accept(TArray type)
        {
            return true;
        }

        public override bool Accept(TList type)
        {
            return true;
        }

        public override bool Accept(TSet type)
        {
            return true;
        }

        public override bool Accept(TMap type)
        {
            return true;
        }
    }
}
