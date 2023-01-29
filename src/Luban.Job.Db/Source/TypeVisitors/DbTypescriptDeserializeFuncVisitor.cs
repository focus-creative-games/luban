using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbTypescriptDeserializeFuncVisitor : ITypeFuncVisitor<string>
    {
        public static DbTypescriptDeserializeFuncVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "SerializeFactory.deserializeBool";
        }

        public string Accept(TByte type)
        {
            return "SerializeFactory.deserializeByte";
        }

        public string Accept(TShort type)
        {
            return "SerializeFactory.deserializeShort";
        }

        public string Accept(TFshort type)
        {
            return "SerializeFactory.deserializeFshort";
        }

        public string Accept(TInt type)
        {
            return "SerializeFactory.deserializeInt";
        }

        public string Accept(TFint type)
        {
            return "SerializeFactory.deserializeFint";
        }

        public string Accept(TLong type)
        {
            return "SerializeFactory.deserializeLong";
        }

        public string Accept(TFlong type)
        {
            return "SerializeFactory.deserializeFlong";
        }

        public string Accept(TFloat type)
        {
            return "SerializeFactory.deserializeFloat";
        }

        public string Accept(TDouble type)
        {
            return "SerializeFactory.deserializeDouble";
        }

        public string Accept(TEnum type)
        {
            return "SerializeFactory.deserializeInt";
        }

        public string Accept(TString type)
        {
            return "SerializeFactory.deserializeString";
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
                return $"{type.Apply(DbTypescriptDefineTypeNameVisitor.Ins)}.deserialize{type.Bean.Name}Any";
            }
            else
            {
                return $"(_buf: ByteBuf):{typeName} => {{ var x = new {typeName}();  x.deserialize(_buf); return x }}";
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
            return "SerializeFactory.deserializeVector2";
        }

        public string Accept(TVector3 type)
        {
            return "SerializeFactory.deserializeVector3";
        }

        public string Accept(TVector4 type)
        {
            return "SerializeFactory.deserializeVector4";
        }

        public string Accept(TDateTime type)
        {
            return "SerializeFactory.deserializeLong";
        }
    }
}
