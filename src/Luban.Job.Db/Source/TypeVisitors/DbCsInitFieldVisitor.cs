using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsInitFieldVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static DbCsInitFieldVisitor Ins { get; } = new DbCsInitFieldVisitor();

        public string Accept(TBool type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TByte type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TShort type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFshort type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TInt type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFint type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TLong type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFlong type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TFloat type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TDouble type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TEnum type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TString type, string fieldName, string logType)
        {
            return $"{fieldName} = \"\";";
        }

        public string Accept(TBytes type, string fieldName, string logType)
        {
            return $"{fieldName} = System.Array.Empty<byte>();";
        }

        public string Accept(TText type, string fieldName, string logType)
        {
            throw new NotSupportedException();
        }

        public string Accept(TBean type, string fieldName, string logType)
        {
            if (type.Bean.IsAbstractType)
            {
                return $"{fieldName} = default;";
            }
            else
            {
                return $"{fieldName} = new {type.DbCsDefineType()}();";
            }
        }

        public string Accept(TArray type, string fieldName, string logType)
        {
            throw new NotSupportedException();
        }

        public string Accept(TList type, string fieldName, string logType)
        {
            return $"{fieldName} = new {type.DbCsDefineType()}(_v => new {logType}(this, _v));";
        }

        public string Accept(TSet type, string fieldName, string logType)
        {
            return $"{fieldName} = new {type.DbCsDefineType()}(_v => new {logType}(this, _v));";
        }

        public string Accept(TMap type, string fieldName, string logType)
        {
            return $"{fieldName} = new {type.DbCsDefineType()}(_v => new {logType}(this, _v));";
        }

        public string Accept(TVector2 type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TVector3 type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TVector4 type, string fieldName, string logType)
        {
            return $"{fieldName} = default;";
        }

        public string Accept(TDateTime type, string x, string y)
        {
            throw new NotSupportedException();
        }
    }
}
