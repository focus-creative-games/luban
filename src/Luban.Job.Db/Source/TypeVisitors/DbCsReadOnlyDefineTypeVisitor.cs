using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsReadOnlyDefineTypeVisitor : DecoratorFuncVisitor<string>
    {
        public static DbCsReadOnlyDefineTypeVisitor Ins { get; } = new DbCsReadOnlyDefineTypeVisitor();


        public override string DoAccept(TType type)
        {
            return type.Apply(CsDefineTypeName.Ins);
        }

        public override string Accept(TBean type)
        {
            return $"{type.Bean.NamespaceWithTopModule}.IReadOnly{type.Bean.Name}";
        }

        public override string Accept(TList type)
        {
            return $"System.Collections.Generic.IReadOnlyList<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TSet type)
        {
            return $"System.Collections.Generic.IReadOnlySet<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TMap type)
        {
            return $"System.Collections.Generic.IReadOnlyDictionary<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }
    }
}
