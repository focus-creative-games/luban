using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptSerializeFuncVisitor : ITypeFuncVisitor<string>
    {
        public static DbTypescriptSerializeFuncVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "SerializeFactory.serializeBool";
        }

        public string Accept(TByte type)
        {
            return "SerializeFactory.serializeByte";
        }

        public string Accept(TShort type)
        {
            return "SerializeFactory.serializeShort";
        }

        public string Accept(TFshort type)
        {
            return "SerializeFactory.serializeFshort";
        }

        public string Accept(TInt type)
        {
            return "SerializeFactory.serializeInt";
        }

        public string Accept(TFint type)
        {
            return "SerializeFactory.serializeFint";
        }

        public string Accept(TLong type)
        {
            return "SerializeFactory.serializeLong";
        }

        public string Accept(TFlong type)
        {
            return "SerializeFactory.serializeFlong";
        }

        public string Accept(TFloat type)
        {
            return "SerializeFactory.serializeFloat";
        }

        public string Accept(TDouble type)
        {
            return "SerializeFactory.serializeDouble";
        }

        public string Accept(TEnum type)
        {
            return "SerializeFactory.serializeInt";
        }

        public string Accept(TString type)
        {
            return "SerializeFactory.serializeString";
        }

        public string Accept(TBytes type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TText type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TBean type)
        {
            var typeName = type.Apply(DbTypescriptDefineTypeNameVisitor.Ins);
            if (type.IsDynamic)
            {
                return $"{typeName}.serialize{type.Bean.Name}Any";
            }
            else
            {
                return "SerializeFactory.serializeBean";
            }
        }

        public string Accept(TArray type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TList type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TSet type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TMap type)
        {
            throw new NotImplementedException();
        }

        public string Accept(TVector2 type)
        {
            return "SerializeFactory.serializeVector2";
        }

        public string Accept(TVector3 type)
        {
            return "SerializeFactory.serializeVector3";
        }

        public string Accept(TVector4 type)
        {
            return "SerializeFactory.serializeVector4";
        }

        public string Accept(TDateTime type)
        {
            return "SerializeFactory.serializeLong";
        }
    }
}
