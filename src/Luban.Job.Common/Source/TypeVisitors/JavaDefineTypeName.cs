using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    public class JavaDefineTypeName : ITypeFuncVisitor<string>
    {
        public static JavaDefineTypeName Ins { get; } = new JavaDefineTypeName();

        public virtual string Accept(TBool type)
        {
            return type.IsNullable ? "Boolean" : "boolean";
        }

        public virtual string Accept(TByte type)
        {
            return type.IsNullable ? "Byte" : "byte";
        }

        public virtual string Accept(TShort type)
        {
            return type.IsNullable ? "Short" : "short";
        }

        public virtual string Accept(TFshort type)
        {
            return type.IsNullable ? "Short" : "short";
        }

        public virtual string Accept(TInt type)
        {
            return type.IsNullable ? "Integer" : "int";
        }

        public virtual string Accept(TFint type)
        {
            return type.IsNullable ? "Integer" : "int";
        }

        public virtual string Accept(TLong type)
        {
            return type.IsNullable ? "Long" : "long";
        }

        public virtual string Accept(TFlong type)
        {
            return type.IsNullable ? "Long" : "long";
        }

        public virtual string Accept(TFloat type)
        {
            return type.IsNullable ? "Float" : "float";
        }

        public virtual string Accept(TDouble type)
        {
            return type.IsNullable ? "Double" : "double";
        }

        public virtual string Accept(TEnum type)
        {
            //return type.DefineEnum.FullNameWithTopModule;
            return type.IsNullable ? "Integer" : "int";
        }

        public string Accept(TString type)
        {
            return "String";
        }

        public string Accept(TBytes type)
        {
            return "byte[]";
        }

        public string Accept(TText type)
        {
            return "String";
        }

        public string Accept(TBean type)
        {
            return type.Bean.FullNameWithTopModule;
        }

        public string Accept(TArray type)
        {
            return $"{type.ElementType.Apply(this)}[]";
        }

        public string Accept(TList type)
        {
            return $"java.util.List<{type.ElementType.Apply(JavaBoxDefineTypeName.Ins)}>";
        }

        public string Accept(TSet type)
        {
            return $"java.util.Set<{type.ElementType.Apply(JavaBoxDefineTypeName.Ins)}>";
        }

        public string Accept(TMap type)
        {
            return $"java.util.Map<{type.KeyType.Apply(JavaBoxDefineTypeName.Ins)}, {type.ValueType.Apply(JavaBoxDefineTypeName.Ins)}>";
        }

        public string Accept(TVector2 type)
        {
            return "bright.math.Vector2";
        }

        public string Accept(TVector3 type)
        {
            return "bright.math.Vector3";
        }

        public string Accept(TVector4 type)
        {
            return "bright.math.Vector4";
        }

        public virtual string Accept(TDateTime type)
        {
            return type.IsNullable ? "Long" : "long";
        }
    }
}
