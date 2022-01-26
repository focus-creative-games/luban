using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CsCtorValueVisitor : DecoratorFuncVisitor<string>
    {
        public static CsCtorValueVisitor Ins { get; } = new CsCtorValueVisitor();

        public override string DoAccept(TType type)
        {
            return "default";
        }

        public override string Accept(TString type)
        {
            return "\"\"";
        }

        public override string Accept(TBytes type)
        {
            return "System.Array.Empty<byte>()";
        }

        public override string Accept(TText type)
        {
            return "\"\"";
        }

        public override string Accept(TBean type)
        {
            return type.Bean.IsAbstractType ? "default" : $"new {type.Apply(CsDefineTypeName.Ins)}()";
        }

        public override string Accept(TArray type)
        {
            return $"System.Array.Empty<{type.ElementType.Apply(CsDefineTypeName.Ins)}>()";
        }

        public override string Accept(TList type)
        {
            return $"new {ConstStrings.CsList}<{type.ElementType.Apply(CsDefineTypeName.Ins)}>()";
        }

        public override string Accept(TSet type)
        {
            return $"new {ConstStrings.CsHashSet}<{type.ElementType.Apply(CsDefineTypeName.Ins)}>()";
        }

        public override string Accept(TMap type)
        {
            return $"new {ConstStrings.CsHashMap}<{type.KeyType.Apply(CsDefineTypeName.Ins)},{type.ValueType.Apply(CsDefineTypeName.Ins)}>()";
        }
    }
}
