using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsInitFieldVisitor : ITypeFuncVisitor<string, string>
    {
        public static DbCsInitFieldVisitor Ins { get; } = new();

        public string Accept(TBool type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TByte type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TShort type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFshort type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TInt type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFint type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TLong type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFlong type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFloat type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TDouble type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TEnum type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TString type, string fieldName)
        {
            return $"{fieldName} = \"\";";
        }

        public string Accept(TBytes type, string fieldName)
        {
            return $"{fieldName} = System.Array.Empty<byte>();";
        }

        public string Accept(TText type, string fieldName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TBean type, string fieldName)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = default;";
            }
            else
            {
                return $"{fieldName} = new {type.Apply(DbCsDefineTypeVisitor.Ins)}();";
            }
        }

        public string Accept(TArray type, string fieldName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TList type, string fieldName)
        {
            var elementType = type.ElementType;
            return $"{fieldName} = new {type.Apply(DbCsDefineTypeVisitor.Ins)}(FieldTag.{elementType.Apply(TagNameVisitor.Ins)}, {elementType.Apply(DbCsSerializeFuncVisitor.Ins)}, {elementType.Apply(DbCsDeserializeFuncVisitor.Ins)});";
        }

        public string Accept(TSet type, string fieldName)
        {
            var elementType = type.ElementType;
            return $"{fieldName} = new {type.Apply(DbCsDefineTypeVisitor.Ins)}(FieldTag.{elementType.Apply(TagNameVisitor.Ins)}, {elementType.Apply(DbCsSerializeFuncVisitor.Ins)}, {elementType.Apply(DbCsDeserializeFuncVisitor.Ins)});";
        }

        public string Accept(TMap type, string fieldName)
        {
            var keyType = type.KeyType;
            var valueType = type.ValueType;
            return $"{fieldName} = new {type.Apply(DbCsDefineTypeVisitor.Ins)}(FieldTag.{keyType.Apply(TagNameVisitor.Ins)}, FieldTag.{valueType.Apply(TagNameVisitor.Ins)}, {keyType.Apply(DbCsSerializeFuncVisitor.Ins)}, {keyType.Apply(DbCsDeserializeFuncVisitor.Ins)}, {valueType.Apply(DbCsSerializeFuncVisitor.Ins)}, {valueType.Apply(DbCsDeserializeFuncVisitor.Ins)});";
        }

        public string Accept(TVector2 type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TVector3 type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TVector4 type, string fieldName)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TDateTime type, string fieldName)
        {
            return $"{fieldName} = default;";
        }
    }
}
