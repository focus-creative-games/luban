using Luban.Job.Common.Defs;
using Luban.Job.Common.Types;

namespace Luban.Job.Common.TypeVisitors
{
    class CsUnderingSerializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static CsUnderingSerializeVisitor Ins { get; } = new CsUnderingSerializeVisitor();

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
            return $"{type.Bean.FullName}.Serialize{type.Bean.Name}({bufName}, {fieldName});";
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteSize({fieldName}.Length); foreach(var _e in {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteSize({fieldName}.Count); foreach(var _e in {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteSize({fieldName}.Count); foreach(var _e in {fieldName}) {{ {type.ElementType.Apply(this, bufName, "_e")} }} }}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{ {bufName}.WriteSize({fieldName}.Count); foreach(var _e in {fieldName}) {{ {type.KeyType.Apply(this, bufName, "_e.Key")} {type.ValueType.Apply(this, bufName, "_e.Value")} }} }}";
        }


        public static string VectorName => (DefAssemblyBase.IsUseUnityVectors ? "UnityVector" : "Vector");

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"{bufName}.Write{VectorName}2({fieldName});";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"{bufName}.Write{VectorName}3({fieldName});";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"{bufName}.Write{VectorName}4({fieldName});";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"{bufName}.WriteLong({fieldName});";
        }
    }
}
