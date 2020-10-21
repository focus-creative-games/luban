using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoTypeNameVisitor : ITypeFuncVisitor<string>
    {
        public static GoTypeNameVisitor Ins { get; } = new GoTypeNameVisitor();

        public string Accept(TBool type)
        {
            return "bool";
        }

        public string Accept(TByte type)
        {
            return "byte";
        }

        public string Accept(TShort type)
        {
            return "int16";
        }

        public string Accept(TFshort type)
        {
            return "int16";
        }

        public string Accept(TInt type)
        {
            return "int32";
        }

        public string Accept(TFint type)
        {
            return "int32";
        }

        public string Accept(TLong type)
        {
            return "int64";
        }

        public string Accept(TFlong type)
        {
            return "int64";
        }

        public string Accept(TFloat type)
        {
            return "float32";
        }

        public string Accept(TDouble type)
        {
            return "float64";
        }

        public string Accept(TEnum type)
        {
            return "int32";
        }

        public string Accept(TString type)
        {
            return "string";
        }

        public string Accept(TBytes type)
        {
            return "[]byte";
        }

        public string Accept(TText type)
        {
            return "string";
        }

        public string Accept(TBean type)
        {
            return type.Bean.IsAbstractType ? $"interface{{}}" : $"*{type.Bean.GoFullName}";
        }

        public string Accept(TArray type)
        {
            return $"[]{type.ElementType.Apply(this)}";
        }

        public string Accept(TList type)
        {
            return $"[]{type.ElementType.Apply(this)}";
        }

        public string Accept(TSet type)
        {
            return $"[]{type.ElementType.Apply(this)}";
        }

        public string Accept(TMap type)
        {
            return $"map[{type.KeyType.Apply(this)}]{type.ValueType.Apply(this)}";
        }

        public string Accept(TVector2 type)
        {
            return $"math.Vector2";
        }

        public string Accept(TVector3 type)
        {
            return $"math.Vector3";
        }

        public string Accept(TVector4 type)
        {
            return $"math.Vector4";
        }

        public string Accept(TDateTime type)
        {
            return "int32";
        }
    }
}
