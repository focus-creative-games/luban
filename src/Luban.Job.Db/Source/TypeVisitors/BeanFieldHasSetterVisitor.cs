using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class BeanFieldHasSetterVisitor : AllTrueVisitor
    {
        public static BeanFieldHasSetterVisitor Ins { get; } = new BeanFieldHasSetterVisitor();


        public override bool Accept(TBean type)
        {
            return type.IsDynamic;
        }

        public override bool Accept(TBytes type)
        {
            return false;
        }

        public override bool Accept(TArray type)
        {
            throw new NotSupportedException();
        }

        public override bool Accept(TList type)
        {
            return false;
        }

        public override bool Accept(TSet type)
        {
            return false;
        }

        public override bool Accept(TMap type)
        {
            return false;
        }
    }
}
