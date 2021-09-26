using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CppRawUnderingDefineTypeName : ITypeFuncVisitor<string>
    {
        public static CppRawUnderingDefineTypeName Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "::bright::byte";
        }

        public string Accept(TShort type)
        {
            return "::bright::int16";
        }

        public string Accept(TFshort type)
        {
            return "::bright::int16";
        }

        public string Accept(TInt type)
        {
            return "::bright::int32";
        }

        public string Accept(TFint type)
        {
            return "::bright::int32";
        }

        public string Accept(TLong type)
        {
            return "::bright::int64";
        }

        public string Accept(TFlong type)
        {
            return "::bright::int64";
        }

        public string Accept(TFloat type)
        {
            return "::bright::float32";
        }

        public string Accept(TDouble type)
        {
            return "::bright::float64";
        }

        public string Accept(TEnum type)
        {
            return type.DefineEnum.CppFullName;
        }

        public string Accept(TString type)
        {
            return "::bright::String";
        }

        public string Accept(TBytes type)
        {
            return "::bright::Bytes";
        }

        public string Accept(TText type)
        {
            return "::bright::String";
        }

        public virtual string Accept(TBean type)
        {
            return $"{type.Bean.CppFullName}*";
        }

        public string Accept(TArray type)
        {
            return $"::bright::Vector<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TList type)
        {
            return $"::bright::Vector<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TSet type)
        {
            return $"::bright::HashSet<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TMap type)
        {
            return $"::bright::HashMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            return "::bright::Vector2";
        }

        public string Accept(TVector3 type)
        {
            return "::bright::Vector3";
        }

        public string Accept(TVector4 type)
        {
            return "::bright::Vector4";
        }

        public string Accept(TDateTime type)
        {
            return "::bright::datetime";
        }
    }
}
