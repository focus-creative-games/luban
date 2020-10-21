using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class CppUnderingDefineTypeName : ITypeFuncVisitor<string>
    {
        public static CppUnderingDefineTypeName Ins { get; } = new CppUnderingDefineTypeName();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "uint8_t";
        }

        public string Accept(TShort type)
        {
            return "int16_t";
        }

        public string Accept(TFshort type)
        {
            return "int16_t";
        }

        public string Accept(TInt type)
        {
            return "int32_t";
        }

        public string Accept(TFint type)
        {
            return "int32_t";
        }

        public string Accept(TLong type)
        {
            return "int64_t";
        }

        public string Accept(TFlong type)
        {
            return "int64_t";
        }

        public string Accept(TFloat type)
        {
            return "float";
        }

        public string Accept(TDouble type)
        {
            return "double";
        }

        public string Accept(TEnum type)
        {
            return type.DefineEnum.CppFullName;
        }

        public string Accept(TString type)
        {
            return "bright::String";
        }

        public string Accept(TBytes type)
        {
            return "bright::Bytes";
        }

        public string Accept(TText type)
        {
            return "bright::String";
        }

        public string Accept(TBean type)
        {
            return type.Bean.CppFullName + "*";
        }

        public string Accept(TArray type)
        {
            return $"std::vector<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TList type)
        {
            return $"std::vector<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TSet type)
        {
            return $"std::unordered_set<{type.ElementType.Apply(this)}>";
        }

        public string Accept(TMap type)
        {
            return $"std::unordered_map<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            return "bright::math::Vector2";
        }

        public string Accept(TVector3 type)
        {
            return "bright::math::Vector3";
        }

        public string Accept(TVector4 type)
        {
            return "bright::math::Vector4";
        }

        public string Accept(TDateTime type)
        {
            return "int32_t";
        }
    }
}
