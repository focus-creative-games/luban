using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class RustTypeUnderlyingNameVisitor : ITypeFuncVisitor<string>
    {
        public static RustTypeUnderlyingNameVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "u8";
        }

        public string Accept(TShort type)
        {
            return "i16";
        }

        public string Accept(TFshort type)
        {
            return "i16";
        }

        public string Accept(TInt type)
        {
            return "i32";
        }

        public string Accept(TFint type)
        {
            return "i32";
        }

        public string Accept(TLong type)
        {
            return "i64";
        }

        public string Accept(TFlong type)
        {
            return "i64";
        }

        public string Accept(TFloat type)
        {
            return "f32";
        }

        public string Accept(TDouble type)
        {
            return "f64";
        }

        public string Accept(TEnum type)
        {
            return "i32";
        }

        public string Accept(TString type)
        {
            return "String";
        }

        public string Accept(TBytes type)
        {
            throw new System.NotSupportedException();
        }

        public string Accept(TText type)
        {
            return "String";
        }

        public string Accept(TBean type)
        {
            return type.Bean.RustFullName;
        }

        public string Accept(TArray type)
        {
            return $"Vec<{type.ElementType.Apply(RustTypeNameVisitor.Ins)}>";
        }

        public string Accept(TList type)
        {
            return $"Vec<{type.ElementType.Apply(RustTypeNameVisitor.Ins)}>";
        }

        public string Accept(TSet type)
        {
            return $"std::collections::HashSet<{type.ElementType.Apply(RustTypeNameVisitor.Ins)}>";
        }

        public string Accept(TMap type)
        {
            return $"std::collections::HashMap<{type.KeyType.Apply(RustTypeNameVisitor.Ins)}, {type.ValueType.Apply(RustTypeNameVisitor.Ins)}>";
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
            return "i64";
        }
    }
}
