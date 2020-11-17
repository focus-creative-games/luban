using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db.TypeVisitors
{
    class CompatibleSerializeNeedEmbedVisitor : AllFalseVisitor
    {
        public static CompatibleSerializeNeedEmbedVisitor Ins { get; } = new CompatibleSerializeNeedEmbedVisitor();

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
