using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsSerializeFuncVisitor : ITypeFuncVisitor<string>
    {
        public static DbCsSerializeFuncVisitor Ins { get; } = new();

        public string Accept(TBool type)
        {
            return "Bright.Common.SerializationUtil.SerializeBool";
        }

        public string Accept(TByte type)
        {
            return "Bright.Common.SerializationUtil.SerializeByte";
        }

        public string Accept(TShort type)
        {
            return "Bright.Common.SerializationUtil.SerializeShort";
        }

        public string Accept(TFshort type)
        {
            return "Bright.Common.SerializationUtil.SerializeFshort";
        }

        public string Accept(TInt type)
        {
            return "Bright.Common.SerializationUtil.SerializeInt";
        }

        public string Accept(TFint type)
        {
            return "Bright.Common.SerializationUtil.SerializeFint";
        }

        public string Accept(TLong type)
        {
            return "Bright.Common.SerializationUtil.SerializeLong";
        }

        public string Accept(TFlong type)
        {
            return "Bright.Common.SerializationUtil.SerializeFlong";
        }

        public string Accept(TFloat type)
        {
            return "Bright.Common.SerializationUtil.SerializeFloat";
        }

        public string Accept(TDouble type)
        {
            return "Bright.Common.SerializationUtil.SerializeDouble";
        }

        public string Accept(TEnum type)
        {
            return "Bright.Common.SerializationUtil.SerializeInt";
        }

        public string Accept(TString type)
        {
            return "Bright.Common.SerializationUtil.SerializeString";
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
                return $"{typeName}.Serialize{type.Bean.Name}";
            }
            else
            {
                return $"Bright.Common.SerializationUtil.SerializeBean<{typeName}>";
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
            return "Bright.Common.SerializationUtil.SerializeVector2";
        }

        public string Accept(TVector3 type)
        {
            return "Bright.Common.SerializationUtil.SerializeVector3";
        }

        public string Accept(TVector4 type)
        {
            return "Bright.Common.SerializationUtil.SerializeVector4";
        }

        public string Accept(TDateTime type)
        {
            return "Bright.Common.SerializationUtil.SerializeLong";
        }
    }
}
