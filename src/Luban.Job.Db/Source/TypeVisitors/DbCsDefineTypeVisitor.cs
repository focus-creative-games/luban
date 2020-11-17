using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsDefineTypeVisitor : DecoratorFuncVisitor<string>
    {
        public static DbCsDefineTypeVisitor Ins { get; } = new DbCsDefineTypeVisitor();


        public override string DoAccept(TType type)
        {
            return type.Apply(CsDefineTypeName.Ins);
        }

        public override string Accept(TList type)
        {
            return $"Bright.Transaction.Collections.{(type.ElementType is TBean ? " PList2" : "PList1")}<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TSet type)
        {
            return $"Bright.Transaction.Collections.PSet1<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TMap type)
        {
            return $"Bright.Transaction.Collections.{(type.ValueType is TBean ? " PMap2" : "PMap1")}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }
    }
}
