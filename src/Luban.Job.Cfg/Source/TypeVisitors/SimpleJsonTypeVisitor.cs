using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class SimpleJsonTypeVisitor : AllTrueVisitor
    {
        public static SimpleJsonTypeVisitor Ins { get; } = new SimpleJsonTypeVisitor();

        public override bool Accept(TEnum type)
        {
            return false;
        }

        public override bool Accept(TVector2 type)
        {
            return false;
        }

        public override bool Accept(TVector3 type)
        {
            return false;
        }

        public override bool Accept(TVector4 type)
        {
            return false;
        }

        public override bool Accept(TBean type)
        {
            //return type.Bean.IsNotAbstractType && type.Bean.HierarchyFields.All(f => f.CType.Apply(this));
            return false;
        }

        public override bool Accept(TArray type)
        {
            return type.ElementType.Apply(this);
        }

        public override bool Accept(TList type)
        {
            return type.ElementType.Apply(this);
        }

        public override bool Accept(TSet type)
        {
            return type.ElementType.Apply(this);
        }

        public override bool Accept(TMap type)
        {
            return false;
        }
    }
}
