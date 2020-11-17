using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db.TypeVisitors
{
    class NeedSetChildrenRootVisitor : AllFalseVisitor
    {
        public static NeedSetChildrenRootVisitor Ins { get; } = new NeedSetChildrenRootVisitor();

        public override bool Accept(TBean type)
        {
            return true;
        }

        public override bool Accept(TList type)
        {
            return type.ElementType is TBean;
        }

        public override bool Accept(TMap type)
        {
            return type.ValueType is TBean;
        }
    }
}
