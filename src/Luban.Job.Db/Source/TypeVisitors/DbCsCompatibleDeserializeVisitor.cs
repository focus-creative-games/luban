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
            return $"{fieldName}.Deserialize({bufName});";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{fieldName}.Deserialize({bufName});";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{fieldName}.Deserialize({bufName});";
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
            return $"{fieldName} = {bufName}.ReadLong();";
        }
    }
}
