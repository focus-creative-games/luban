using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsCompatibleSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static DbCsCompatibleSerializeVisitor Ins { get; } = new DbCsCompatibleSerializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteBool({fieldName});";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteByte({fieldName});";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteShort({fieldName});";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFshort({fieldName});";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt({fieldName});";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFint({fieldName});";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteLong({fieldName});";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFlong({fieldName});";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteFloat({fieldName});";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteDouble({fieldName});";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteInt((int){fieldName});";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteString({fieldName});";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteBytes({fieldName});";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteString({fieldName});";
        }

        public string Accept(TBean type, string bufName, string fieldName)
        {
            var bean = type.Bean;
            if (bean.IsNotAbstractType)
            {
                return $"{fieldName}.Serialize({bufName});";
            }
            else
            {
                return $"{bean.FullName}.Serialize{bean.Name}({bufName}, {fieldName});";
            }
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            throw new NotSupportedException();
        }

        private string EnterSegment(TType type, string bufName)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $"{bufName}.EnterSegment(out var _state2_);";
            }
            else
            {
                return "";
            }
        }

        private string LeaveSegment(TType type, string bufName)
        {
            if (type.Apply(CompatibleSerializeNeedEmbedVisitor.Ins))
            {
                return $"{bufName}.LeaveSegment(_state2_);";
            }
            else
            {
                return "";
            }
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{fieldName}.Serialize({bufName});";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{fieldName}.Serialize({bufName});";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{fieldName}.Serialize({bufName});";
        }

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteVector2({fieldName});";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteVector3({fieldName});";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteVector4({fieldName});";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteLong({fieldName});";
        }
    }
}
