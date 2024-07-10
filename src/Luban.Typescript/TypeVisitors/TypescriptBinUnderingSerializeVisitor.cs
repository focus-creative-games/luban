using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors
{
    public class TypescriptBinUnderingSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static TypescriptBinUnderingSerializeVisitor Ins { get; } = new();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteBool({fieldName})";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteByte({fieldName})";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteShort({fieldName})";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt({fieldName})";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{bufName}.{(type.IsBigInt ? "WriteLong" : "WriteNumberAsLong")}({fieldName})";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFloat({fieldName})";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteDouble({fieldName})";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt({fieldName})";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteString({fieldName})";
        }

        public string Accept(TDateTime type, string bufVarName, string fieldName)
        {
            return $"{bufVarName}.WriteLong({fieldName})";
        }

        public virtual string Accept(TBean type, string bufVarName, string fieldName)
        {
            if (type.DefBean.IsAbstractType)
            {
                return $"{type.DefBean.FullName}.serializeTo({bufVarName}, {fieldName})";
            }
            else
            {
                return $"{fieldName}.serialize({bufVarName})";
            }
        }

        public string Accept(TArray type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.length);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public virtual string Accept(TList type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.length);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public virtual string Accept(TSet type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.size);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public virtual string Accept(TMap type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.size);   for(let [_k, _v] of {fieldName}) {{ {type.KeyType.Apply(this, bufVarName, "_k")}; {type.ValueType.Apply(this, bufVarName, "_v")} }} }}";
        }
    }
}
