using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptInitFieldVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static DbTypescriptInitFieldVisitor Ins { get; } = new();

        public string Accept(TBool type, string fieldName, string logType)
        {
            return $"{fieldName} = false";
        }

        public string Accept(TByte type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TShort type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TFshort type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TInt type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TFint type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TLong type, string fieldName, string logType)
        {
            return $"{fieldName} = {(type.IsBigInt ? "BigInt(0)" : "0")}";
        }

        public string Accept(TFlong type, string fieldName, string logType)
        {
            return $"{fieldName} = BigInt(0)";
        }

        public string Accept(TFloat type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TDouble type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TEnum type, string fieldName, string logType)
        {
            return $"{fieldName} = 0";
        }

        public string Accept(TString type, string fieldName, string logType)
        {
            return $"{fieldName} = \"\"";
        }

        public string Accept(TBytes type, string fieldName, string logType)
        {
            return $"{fieldName} = new Uint8Array()";
        }

        public string Accept(TText type, string fieldName, string logType)
        {
            throw new NotSupportedException();
        }

        public string Accept(TBean type, string fieldName, string logType)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = null";
            }
            else
            {
                return $"{fieldName} = new {type.Apply(DbTypescriptDefineTypeNameVisitor.Ins)}()";
            }
        }

        public string Accept(TArray type, string fieldName, string logType)
        {
            throw new NotSupportedException();
        }

        public string Accept(TList type, string fieldName, string logType)
        {
            var elementType = type.ElementType;
            return $"{fieldName} = new {type.Apply(DbTypescriptDefineTypeNameVisitor.Ins)}(FieldTag.{elementType.Apply(TagNameVisitor.Ins)}, {elementType.Apply(DbTypescriptSerializeFuncVisitor.Ins)}, {elementType.Apply(DbTypescriptDeserializeFuncVisitor.Ins)})";
        }

        public string Accept(TSet type, string fieldName, string logType)
        {
            var elementType = type.ElementType;
            return $"{fieldName} = new {type.Apply(DbTypescriptDefineTypeNameVisitor.Ins)}(FieldTag.{elementType.Apply(TagNameVisitor.Ins)}, {elementType.Apply(DbTypescriptSerializeFuncVisitor.Ins)}, {elementType.Apply(DbTypescriptDeserializeFuncVisitor.Ins)})";
        }

        public string Accept(TMap type, string fieldName, string logType)
        {
            var keyType = type.KeyType;
            var valueType = type.ValueType;
            return $"{fieldName} = new {type.Apply(DbTypescriptDefineTypeNameVisitor.Ins)}(FieldTag.{keyType.Apply(TagNameVisitor.Ins)}, FieldTag.{valueType.Apply(TagNameVisitor.Ins)}, {keyType.Apply(DbTypescriptSerializeFuncVisitor.Ins)}, {keyType.Apply(DbTypescriptDeserializeFuncVisitor.Ins)}, {valueType.Apply(DbTypescriptSerializeFuncVisitor.Ins)}, {valueType.Apply(DbTypescriptDeserializeFuncVisitor.Ins)})";

        }

        public string Accept(TVector2 type, string fieldName, string logType)
        {
            return $"{fieldName} = new Vector2()";
        }

        public string Accept(TVector3 type, string fieldName, string logType)
        {
            return $"{fieldName} = new Vector3()";
        }

        public string Accept(TVector4 type, string fieldName, string logType)
        {
            return $"{fieldName} = new Vector4()";
        }

        public string Accept(TDateTime type, string fieldName, string logType
            )
        {
            return $"{fieldName} = 0";
        }
    }
}
