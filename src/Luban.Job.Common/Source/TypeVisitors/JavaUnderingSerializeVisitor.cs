using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    class JavaUnderingSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static JavaUnderingSerializeVisitor Ins { get; } = new JavaUnderingSerializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{bufName}.writeBool({fieldName});";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{bufName}.writeByte({fieldName});";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{bufName}.writeShort({fieldName});";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{bufName}.writeFshort({fieldName});";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{bufName}.writeInt({fieldName});";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{bufName}.writeFint({fieldName});";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{bufName}.writeLong({fieldName});";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{bufName}.writeFlong({fieldName});";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{bufName}.writeFloat({fieldName});";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{bufName}.writeDouble({fieldName});";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{bufName}.writeInt((int){fieldName});";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{bufName}.writeString({fieldName});";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{bufName}.writeBytes({fieldName});";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"{bufName}.writeString({fieldName});";
        }

        public string Accept(TBean type, string bufName, string fieldName)
        {
            return $"{type.Bean.FullNameWithTopModule}.serialize{type.Bean.Name}({bufName}, {fieldName});";
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.writeSize({fieldName}.length); for({type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e : {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.writeSize({fieldName}.size()); for({type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e : {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.writeSize({fieldName}.size()); for({type.ElementType.Apply(JavaBoxDefineTypeName.Ins)} _e : {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.writeSize({fieldName}.size()); for(java.util.Map.Entry<{type.KeyType.Apply(JavaBoxDefineTypeName.Ins)},{type.ValueType.Apply(JavaBoxDefineTypeName.Ins)}> _e : {fieldName}.entrySet()) {{ {type.KeyType.Apply(this, bufName, "_e.getKey()")} {type.ValueType.Apply(this, bufName, "_e.getValue()")} }} }}";
        }


        public static string VectorName => "Vector";

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"{bufName}.write{VectorName}2({fieldName});";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"{bufName}.write{VectorName}3({fieldName});";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"{bufName}.write{VectorName}4({fieldName});";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"{bufName}.writeLong({fieldName});";
        }
    }
}
