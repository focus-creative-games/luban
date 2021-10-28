using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class ErlangDefineTypeNameVisitor : ITypeFuncVisitor<string>
    {
        public static ErlangDefineTypeNameVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "boolean()";
        }

        public string Accept(TByte type)
        {
            return "binary()";
        }

        public string Accept(TShort type)
        {
            return "integer()";
        }

        public string Accept(TFshort type)
        {
            return "integer()";
        }

        public string Accept(TInt type)
        {
            return "integer()";
        }

        public string Accept(TFint type)
        {
            return "integer()";
        }

        public string Accept(TLong type)
        {
            return "integer()";
        }

        public string Accept(TFlong type)
        {
            return "integer()";
        }

        public string Accept(TFloat type)
        {
            return "float()";
        }

        public string Accept(TDouble type)
        {
            return "float()";
        }

        public string Accept(TEnum type)
        {
            return "string()";
        }

        public string Accept(TString type)
        {
            return "string()";
        }

        public string Accept(TBytes type)
        {
            return "[binary()]";
        }

        public string Accept(TText type)
        {
            return "string()";
        }

        public string Accept(TBean type)
        {
            return $"'{type.Bean.FullName}'()" ;
        }

        public virtual string Accept(TArray type)
        {
            return $"[{type.ElementType.Apply(this)}]";
        }

        public virtual string Accept(TList type)
        {
            return $"[{type.ElementType.Apply(this)}]";
        }

        public virtual string Accept(TSet type)
        { 
            return $"[{type.ElementType.Apply(this)}]";
        }

        public virtual string Accept(TMap type)
        {
            return string.Format("#{{0} => {1}}",type.KeyType.Apply(this),type.ValueType.Apply(this));
        }

        public string Accept(TVector2 type)
        {
            return "[float()]";
        }

        public string Accept(TVector3 type)
        {
            return "[float()]";
        }

        public string Accept(TVector4 type)
        {
            return "[float()]";
        }

        public string Accept(TDateTime type)
        {
            return "integer()";
        }
    }
}
