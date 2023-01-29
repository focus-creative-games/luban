using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class TagNameVisitor : ITypeFuncVisitor<string>
    {
        public static TagNameVisitor Ins { get; } = new TagNameVisitor();

        public string Accept(TBool type)
        {
            return "BOOL";
        }

        public string Accept(TByte type)
        {
            return "BYTE";
        }

        public string Accept(TShort type)
        {
            return "SHORT";
        }

        public string Accept(TFshort type)
        {
            return "FSHORT";
        }

        public string Accept(TInt type)
        {
            return "INT";
        }

        public string Accept(TFint type)
        {
            return "FINT";
        }

        public string Accept(TLong type)
        {
            return "LONG";
        }

        public string Accept(TFlong type)
        {
            return "FLONG";
        }

        public string Accept(TFloat type)
        {
            return "FLOAT";
        }

        public string Accept(TDouble type)
        {
            return "DOUBLE";
        }

        public string Accept(TEnum type)
        {
            return "INT";
        }

        public string Accept(TString type)
        {
            return "STRING";
        }

        public string Accept(TBytes type)
        {
            return "BYTES";
        }

        public string Accept(TText type)
        {
            return "STRING";
        }

        public string Accept(TBean type)
        {
            return type.IsDynamic ? "DYNAMIC_BEAN" : "BEAN";
        }

        public string Accept(TArray type)
        {
            return "ARRAY";
        }

        public string Accept(TList type)
        {
            return "LIST";
        }

        public string Accept(TSet type)
        {
            return "SET";
        }

        public string Accept(TMap type)
        {
            return "MAP";
        }

        public string Accept(TVector2 type)
        {
            return "VECTOR2";
        }

        public string Accept(TVector3 type)
        {
            return "VECTOR3";
        }

        public string Accept(TVector4 type)
        {
            return "VECTOR4";
        }

        public string Accept(TDateTime type)
        {
            return "LONG";
        }
    }
}
