using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class ImmutableTypeName : DecoratorFuncVisitor<string>
    {
        public static ImmutableTypeName Ins { get; } = new ImmutableTypeName();


        public override string DoAccept(TType type)
        {
            return "";
        }

        public override string Accept(TList type)
        {
            return $"System.Collections.Immutable.ImmutableList<{type.ElementType.Apply(DbCsDefineTypeVisitor.Ins)}>";
        }

        public override string Accept(TSet type)
        {
            return $"System.Collections.Immutable.ImmutableHashSet<{type.ElementType.Apply(DbCsDefineTypeVisitor.Ins)}>";
        }

        public override string Accept(TMap type)
        {
            return $"System.Collections.Immutable.ImmutableDictionary<{type.KeyType.Apply(DbCsDefineTypeVisitor.Ins)}, {type.ValueType.Apply(DbCsDefineTypeVisitor.Ins)}>";
        }
    }
}
