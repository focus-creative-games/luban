using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Common.TypeVisitors
{
    public class GoSerializeUnderingVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static GoSerializeUnderingVisitor Ins { get; } = new();

        public string Accept(TBool type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteBool({fieldName})";
        }

        public string Accept(TByte type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteByte({fieldName})";
        }

        public string Accept(TShort type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteShort({fieldName})";
        }

        public string Accept(TFshort type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteFshort({fieldName})";
        }

        public string Accept(TInt type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteInt({fieldName})";
        }

        public string Accept(TFint type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteFint({fieldName})";
        }

        public string Accept(TLong type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteLong({fieldName})";
        }

        public string Accept(TFlong type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteFlong({fieldName})";
        }

        public string Accept(TFloat type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteFloat({fieldName})";
        }

        public string Accept(TDouble type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteDouble({fieldName})";
        }

        public string Accept(TEnum type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteInt({fieldName})";
        }

        public string Accept(TString type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteString({fieldName})";
        }

        public string Accept(TText type, string fieldName, string bufName)
        {
            throw new System.NotSupportedException();
        }

        public string Accept(TBytes type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteBytes({fieldName})";
        }

        public string Accept(TVector2 type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteVector2({fieldName})";
        }

        public string Accept(TVector3 type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteVector3({fieldName})";
        }

        public string Accept(TVector4 type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteVector4({fieldName})";
        }

        public string Accept(TDateTime type, string fieldName, string bufName)
        {
            return $"{bufName}.WriteLong({fieldName})";
        }

        public string Accept(TBean type, string fieldName, string bufName)
        {
            return $"Serialize{type.Bean.GoFullName}({fieldName}, {bufName})";
        }

        private string GenList(TType elementType, string fieldName, string bufName)
        {
            return $@"{{ {bufName}.WriteSize(len({fieldName})); for _, _e_ := range({fieldName}) {{ {elementType.Apply(GoSerializeBinVisitor.Ins, "_e_", bufName)} }} }}";
        }

        public string Accept(TArray type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TList type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TSet type, string fieldName, string bufName)
        {
            return GenList(type.ElementType, fieldName, bufName);
        }

        public string Accept(TMap type, string fieldName, string bufName)
        {
            return $@"{{{bufName}.WriteSize(len({fieldName})); for _k_, _v_ := range({fieldName}) {{ {type.KeyType.Apply(GoSerializeBinVisitor.Ins, "_k_", bufName)}; {type.ValueType.Apply(GoSerializeBinVisitor.Ins, "_v_", bufName)} }} }}";
        }
    }
}
