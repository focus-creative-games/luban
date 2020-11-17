using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Proto.TypeVisitors
{
    class CsUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static CsUnderingDeserializeVisitor Ins { get; } = new CsUnderingDeserializeVisitor();

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
            return $"{fieldName} = {type.Bean.FullName}.Deserialize{type.Bean.Name}({bufName});";
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            return $"{{int n = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.ElementType.Apply(CsDefineTypeName.Ins)}[n];for(var i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}[i] = _e;}}}}";
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{int n = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}(n);for(var i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.Add(_e);}}}}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{int n = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}(/*n * 3 / 2*/);for(var i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CsDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.Add(_e);}}}}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{int n = System.Math.Min({bufName}.ReadSize(), {bufName}.Size);{fieldName} = new {type.Apply(CsDefineTypeName.Ins)}(n * 3 / 2);for(var i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(CsDefineTypeName.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(CsDefineTypeName.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}.Add(_k, _v);}}}}";

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
