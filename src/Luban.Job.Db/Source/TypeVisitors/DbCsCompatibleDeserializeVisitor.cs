using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Db.TypeVisitors
{
    class DbCsCompatibleDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static DbCsCompatibleDeserializeVisitor Ins { get; } = new DbCsCompatibleDeserializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadBool();";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadByte();";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadShort();";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFshort();";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadInt();";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFint();";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadLong();";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFlong();";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadFloat();";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadDouble();";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{fieldName} = ({type.DefineEnum.FullName}){bufName}.ReadInt();";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadString();";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadBytes();";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadString();";
        }

        public string Accept(TBean type, string bufName, string fieldName)
        {
            var bean = type.Bean;
            if (bean.IsNotAbstractType)
            {
                return $"{fieldName} = new {type.Apply(DbCsDefineTypeVisitor.Ins)}(); {fieldName}.Deserialize({bufName});";
            }
            else
            {
                return $"{fieldName} = {bean.FullName}.Deserialize{bean.Name}({bufName});";
            }
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            throw new System.NotSupportedException();
        }

        private string BeginSegment(TType type, string bufName)
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

        private string EndSegment(TType type, string bufName)
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
            return $"int _tagType = {bufName}.ReadInt(); System.Diagnostics.Debug.Assert(_tagType == FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); while({bufName}.Size > 0) {{ {BeginSegment(type.ElementType, bufName)} {type.ElementType.Apply(DbCsDefineTypeVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {EndSegment(type.ElementType, bufName)} {fieldName}.Add(_e);}}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"int _tagType = {bufName}.ReadInt(); System.Diagnostics.Debug.Assert(_tagType == FieldTag.{type.ElementType.Apply(TagNameVisitor.Ins)}); while({bufName}.Size > 0) {{ {BeginSegment(type.ElementType, bufName)} {type.ElementType.Apply(DbCsDefineTypeVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {EndSegment(type.ElementType, bufName)} {fieldName}.Add(_e);}}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"int _keyTagType = {bufName}.ReadInt(); System.Diagnostics.Debug.Assert(_keyTagType == FieldTag.{type.KeyType.Apply(TagNameVisitor.Ins)}); int _valueTagType = {bufName}.ReadInt(); System.Diagnostics.Debug.Assert(_valueTagType == FieldTag.{type.ValueType.Apply(TagNameVisitor.Ins)}); while({bufName}.Size > 0) {{ {type.KeyType.Apply(DbCsDefineTypeVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {BeginSegment(type.ValueType, bufName)} {type.ValueType.Apply(DbCsDefineTypeVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")} {EndSegment(type.ValueType, bufName)} {fieldName}.Add(_k, _v);}}";
        }

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadVector2();";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadVector3();";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadVector4();";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"{fieldName} = {bufName}.ReadInt();";
        }
    }
}
