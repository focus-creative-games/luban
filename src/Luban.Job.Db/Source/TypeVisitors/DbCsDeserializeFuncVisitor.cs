using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsDeserializeFuncVisitor : ITypeFuncVisitor<string>
    {
        public static DbCsDeserializeFuncVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "Bright.Common.SerializationUtil.DeserializeBool";
        }

        public string Accept(TByte type)
        {
            return "Bright.Common.SerializationUtil.DeserializeByte";
        }

        public string Accept(TShort type)
        {
            return "Bright.Common.SerializationUtil.DeserializeShort";
        }

        public string Accept(TFshort type)
        {
            return "Bright.Common.SerializationUtil.DeserializeFshort";
        }

        public string Accept(TInt type)
        {
            return "Bright.Common.SerializationUtil.DeserializeInt";
        }

        public string Accept(TFint type)
        {
            return "Bright.Common.SerializationUtil.DeserializeFint";
        }

        public string Accept(TLong type)
        {
            return "Bright.Common.SerializationUtil.DeserializeLong";
        }

        public string Accept(TFlong type)
        {
            return "Bright.Common.SerializationUtil.DeserializeFlong";
        }

        public string Accept(TFloat type)
        {
            return "Bright.Common.SerializationUtil.DeserializeFloat";
        }

        public string Accept(TDouble type)
        {
            return "Bright.Common.SerializationUtil.DeserializeDouble";
        }

        public string Accept(TEnum type)
        {
            return "Bright.Common.SerializationUtil.DeserializeInt";
        }

        public string Accept(TString type)
        {
            return "Bright.Common.SerializationUtil.DeserializeString";
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
                return $"{typeName}.Deserialize{type.Bean.Name}";
            }
            else
            {
                return $"Bright.Common.SerializationUtil.DeserializeBean<{typeName}>";
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
            return "Bright.Common.SerializationUtil.DeserializeVector2";
        }

        public string Accept(TVector3 type)
        {
            return "Bright.Common.SerializationUtil.DeserializeVector3";
        }

        public string Accept(TVector4 type)
        {
            return "Bright.Common.SerializationUtil.DeserializeVector4";
        }

        public string Accept(TDateTime type)
        {
            return "Bright.Common.SerializationUtil.DeserializeLong";
        }
    }
}
