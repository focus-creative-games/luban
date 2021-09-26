using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class TypescriptDefineTypeNameUnderlyingVisitor : ITypeFuncVisitor<string>
    {
        public static TypescriptDefineTypeNameUnderlyingVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "boolean";
        }

        public string Accept(TByte type)
        {
            return "number";
        }

        public string Accept(TShort type)
        {
            return "number";
        }

        public string Accept(TFshort type)
        {
            return "number";
        }

        public string Accept(TInt type)
        {
            return "number";
        }

        public string Accept(TFint type)
        {
            return "number";
        }

        public string Accept(TLong type)
        {
            return type.IsBigInt ? "bigint" : "number";
        }

        public string Accept(TFlong type)
        {
            return "bigint";
        }

        public string Accept(TFloat type)
        {
            return "number";
        }

        public string Accept(TDouble type)
        {
            return "number";
        }

        public string Accept(TEnum type)
        {
            return type.DefineEnum.FullName;
        }

        public string Accept(TString type)
        {
            return "string";
        }

        public string Accept(TBytes type)
        {
            return "Uint8Array";
        }

        public string Accept(TText type)
        {
            return "string";
        }

        public string Accept(TBean type)
        {
            return type.Bean.FullName;
        }


        private string GetArrayType(TType elementType)
        {
            switch (elementType)
            {
                case TByte _: return "Uint8Array";
                case TShort _:
                case TFshort _: return "Int16Array";
                case TInt _:
                case TFint _: return "Int32Array";
                case TLong _:
                case TFlong _: return "Int64Array";
                case TFloat _: return "Float32Array";
                case TDouble _: return "Float64Array";
                default: return $"{elementType.Apply(this)}[]";
            }
        }

        public virtual string Accept(TArray type)
        {
            return GetArrayType(type.ElementType);
        }

        public virtual string Accept(TList type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public virtual string Accept(TSet type)
        {
            return $"Set<{type.ElementType.Apply(this)}>";
        }

        public virtual string Accept(TMap type)
        {
            return $"Map<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            return "Vector2";
        }

        public string Accept(TVector3 type)
        {
            return "Vector3";
        }

        public string Accept(TVector4 type)
        {
            return "Vector4";
        }

        public string Accept(TDateTime type)
        {
            return "number";
        }
    }
}
