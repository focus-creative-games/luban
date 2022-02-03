using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class EmmyLuaCommentTypeVisitor : ITypeFuncVisitor<string>
    {
        public static EmmyLuaCommentTypeVisitor Ins { get; } = new EmmyLuaCommentTypeVisitor();

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
            return "number";
        }

        public string Accept(TFlong type)
        {
            return "number";
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
            return "string";
        }

        public string Accept(TText type)
        {
            return "string";
        }

        public string Accept(TBean type)
        {
            return type.Bean.FullName;
        }

        public string Accept(TArray type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public string Accept(TList type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public string Accept(TSet type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public string Accept(TMap type)
        {
            return $"table<{type.KeyType.Apply(this)},{type.ValueType.Apply(this)}>";
        }

        public string Accept(TVector2 type)
        {
            return "vector2";
        }

        public string Accept(TVector3 type)
        {
            return "vector3";
        }

        public string Accept(TVector4 type)
        {
            return "vector4";
        }

        public string Accept(TDateTime type)
        {
            return "number";
        }
    }
}
