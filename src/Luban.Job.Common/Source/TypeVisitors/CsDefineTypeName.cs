using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsDefineTypeName : DecoratorFuncVisitor<string>
    {
        public static CsDefineTypeName Ins { get; } = new CsDefineTypeName();

        protected virtual ITypeFuncVisitor<string> UnderlyingVisitor => CsUnderingDefineTypeName.Ins;

        public override string DoAccept(TType type)
        {
            return type.IsNullable && !type.Apply(CsIsRawNullableTypeVisitor.Ins) ? (type.Apply(UnderlyingVisitor) + "?") : type.Apply(UnderlyingVisitor);
        }

        public override string Accept(TArray type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public override string Accept(TList type)
        {
            return $"{ConstStrings.CsList}<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TSet type)
        {
            return $"{ConstStrings.CsHashSet}<{type.ElementType.Apply(this)}>";
        }

        public override string Accept(TMap type)
        {
            return $"{ConstStrings.CsHashMap}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }
    }
}
