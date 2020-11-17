using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsCompatibleSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static DbCsCompatibleSerializeVisitor Ins { get; } = new DbCsCompatibleSerializeVisitor();

        public string Accept(TBool type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteBool({fieldName});";
        }

        public string Accept(TByte type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteByte({fieldName});";
        }

        public string Accept(TShort type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteShort({fieldName});";
        }

        public string Accept(TFshort type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteFshort({fieldName});";
        }

        public string Accept(TInt type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteInt({fieldName});";
        }

        public string Accept(TFint type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteFint({fieldName});";
        }

        public string Accept(TLong type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteLong({fieldName});";
        }

        public string Accept(TFlong type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteFlong({fieldName});";
        }

        public string Accept(TFloat type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteFloat({fieldName});";
        }

        public string Accept(TDouble type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteDouble({fieldName});";
        }

        public string Accept(TEnum type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteInt((int){fieldName});";
        }

        public string Accept(TString type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteString({fieldName});";
        }

        public string Accept(TBytes type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteBytes({fieldName});";
        }

        public string Accept(TText type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteString({fieldName});";
        }

        public string Accept(TBean type, string byteBufName, string fieldName)
        {
            var bean = type.Bean;
            if (bean.IsNotAbstractType)
            {
                return $"{byteBufName}.BeginWriteSegment(out var _state2_); {fieldName}.Serialize({byteBufName}); {byteBufName}.EndWriteSegment(_state2_);";
            }
            else
            {
                return $"{byteBufName}.BeginWriteSegment(out var _state2_); {bean.FullName}.Serialize{bean.Name}({byteBufName}, {fieldName});{byteBufName}.EndWriteSegment(_state2_);";
            }
        }

        public string Accept(TArray type, string byteBufName, string fieldName)
        {
            throw new NotSupportedException();
        }

        public string Accept(TList type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteInt(FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); foreach(var _e in {fieldName}) {{ {type.ElementType.Apply(this, byteBufName, "_e")} }}";
        }

        public string Accept(TSet type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteInt(FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); foreach(var _e in {fieldName}) {{ {type.ElementType.Apply(this, byteBufName, "_e")} }}";
        }

        public string Accept(TMap type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteInt(FieldTag.{type.KeyType.Apply(TagNameVisitor.Ins)}); {byteBufName}.WriteInt(FieldTag.{type.ValueType.Apply(TagNameVisitor.Ins)}); foreach((var _k, var _v) in {fieldName}) {{ {type.KeyType.Apply(this, byteBufName, "_k")} {type.ValueType.Apply(this, byteBufName, "_v")}}}";

        }

        public string Accept(TVector2 type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteVector2({fieldName});";
        }

        public string Accept(TVector3 type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteVector3({fieldName});";
        }

        public string Accept(TVector4 type, string byteBufName, string fieldName)
        {
            return $"{byteBufName}.WriteVector4({fieldName});";
        }

        public string Accept(TDateTime type, string x, string y)
        {
            throw new NotImplementedException();
        }
    }
}
