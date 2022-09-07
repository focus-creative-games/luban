using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class LuaCommentTypeVisitor : ITypeFuncVisitor<string>
    {
        public static LuaCommentTypeVisitor Ins { get; } = new LuaCommentTypeVisitor();

        public string Accept(TBool type)
        {
            return "boolean";
        }

        public string Accept(TByte type)
        {
            return "integer";
        }

        public string Accept(TShort type)
        {
            return "integer";
        }

        public string Accept(TFshort type)
        {
            return "integer";
        }

        public string Accept(TInt type)
        {
            return "integer";
        }

        public string Accept(TFint type)
        {
            return "integer";
        }

        public string Accept(TLong type)
        {
            return "integer";
        }

        public string Accept(TFlong type)
        {
            return "integer";
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
            return "integer";
        }

        public string Accept(TString type)
        {
            return "string";
        }

        public string Accept(TBytes type)
        {
            return $"string";
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
            return "{x:number,y:number}";
        }

        public string Accept(TVector3 type)
        {
            return "{x:number,y:number,z:number}";
        }

        public string Accept(TVector4 type)
        {
            return "{x:number,y:number,z:number,w:number}";
        }

        public string Accept(TDateTime type)
        {
            return "integer";
        }
    }
}
