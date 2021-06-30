using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.TypeVisitors
{
    class TypescriptBinUnderingSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static TypescriptBinUnderingSerializeVisitor Ins { get; } = new TypescriptBinUnderingSerializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteBool({fieldName});";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteByte({fieldName});";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteShort({fieldName});";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFshort({fieldName});";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt({fieldName});";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFint({fieldName});";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{bufName}.{(type.IsBigInt ? "WriteLong" : "WriteNumberAsLong")}({fieldName});";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFlong({fieldName});";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFloat({fieldName});";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteDouble({fieldName});";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt({fieldName});";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteString({fieldName});";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteArrayBuffer({fieldName}.buffer);";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteString({fieldName});";
        }

        public string Accept(TVector2 type, string bufVarName, string fieldName)
        {
            return $"{fieldName}.to({bufVarName})";
        }

        public string Accept(TVector3 type, string bufVarName, string fieldName)
        {
            return $"{fieldName}.to({bufVarName})";
        }

        public string Accept(TVector4 type, string bufVarName, string fieldName)
        {
            return $"{fieldName}.to({bufVarName})";
        }

        public string Accept(TDateTime type, string bufVarName, string fieldName)
        {
            return $"{bufVarName}.WriteInt({fieldName});";
        }

        public string Accept(TBean type, string bufVarName, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{type.Bean.FullName}.serializeTo({bufVarName}, {fieldName});";
            }
            else
            {
                return $"{fieldName}.serialize({bufVarName});";
            }
        }

        private static string GetNewArray(TArray arrayType, string size)
        {
            switch (arrayType.ElementType)
            {
                case TByte _: return $"new Uint8Array({size})";
                case TShort _:
                case TFshort _: return $"new Int16Array({size})";
                case TInt _:
                case TFint _: return $"new Int32Array({size})";
                case TLong _:
                case TFlong _: return $"new Int64Array({size})";
                case TFloat _: return $"new Float32Array({size})";
                case TDouble _: return $"new Float64Array({size})";
                default: return "[]";
            }
        }

        private static bool IsRawArrayElementType(TType elementType)
        {
            switch (elementType)
            {
                case TByte _:
                case TShort _:
                case TFshort _:
                case TInt _:
                case TFint _:
                case TLong _:
                case TFlong _:
                case TFloat _:
                case TDouble _: return true;
                default: return false;
            }
        }

        public string Accept(TArray type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.length);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public string Accept(TList type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.length);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public string Accept(TSet type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.size);   for(let _e of {fieldName}) {{ {type.ElementType.Apply(this, bufVarName, "_e")} }} }}";
        }

        public string Accept(TMap type, string bufVarName, string fieldName)
        {
            return $"{{ {bufVarName}.WriteSize({fieldName}.size);   for(let [_k, _v] of {fieldName}) {{ {type.KeyType.Apply(this, bufVarName, "_k")} {type.ValueType.Apply(this, bufVarName, "_v")} }} }}";
        }
    }
}
