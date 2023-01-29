using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CppUnderingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
    {
        public static CppUnderingDeserializeVisitor Ins { get; } = new CppUnderingDeserializeVisitor();

        public string Accept(TBool type, string bufName, string fieldName)
        {
            return $"if (!{bufName}.readBool({fieldName})) return false;";
        }

        public string Accept(TByte type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readByte({fieldName})) return false;";
        }

        public string Accept(TShort type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readShort({fieldName})) return false;";
        }

        public string Accept(TFshort type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readFshort({fieldName})) return false;";
        }

        public string Accept(TInt type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readInt({fieldName})) return false;";
        }

        public string Accept(TFint type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readFint({fieldName})) return false;";
        }

        public string Accept(TLong type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readLong({fieldName})) return false;";
        }

        public string Accept(TFlong type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readFlong({fieldName})) return false;";
        }

        public string Accept(TFloat type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readFloat({fieldName})) return false;";
        }

        public string Accept(TDouble type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readDouble({fieldName})) return false;";
        }

        public string Accept(TEnum type, string bufName, string fieldName)
        {
            return $"{{int __enum_temp__; if(!{bufName}.readInt(__enum_temp__)) return false; {fieldName} = {type.DefineEnum.CppFullName}(__enum_temp__); }}";
        }

        public string Accept(TString type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readString({fieldName})) return false;";
        }

        public string Accept(TBytes type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readBytes({fieldName})) return false;";
        }

        public string Accept(TText type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readString({fieldName})) return false; /* key */ if(!{bufName}.readString({fieldName})) return false; /* text */";
        }

        public string Accept(TBean type, string bufName, string fieldName)
        {
            return $"if(!{type.Bean.CppFullName}::deserialize{type.Bean.Name}({bufName}, {fieldName})) return false;";
        }

        public string Accept(TArray type, string bufName, string fieldName)
        {
            return $"{{::bright::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::bright::int32({bufName}.size()));{fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppDefineTypeName.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
        }

        public string Accept(TList type, string bufName, string fieldName)
        {
            return $"{{::bright::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::bright::int32({bufName}.size())); {fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
        }

        public string Accept(TSet type, string bufName, string fieldName)
        {
            return $"{{::bright::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::bright::int32({bufName}.size())); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppDefineTypeName.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.insert(_e);}}}}";
        }

        public string Accept(TMap type, string bufName, string fieldName)
        {
            return $"{{::bright::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, (::bright::int32){bufName}.size()); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(CppDefineTypeName.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(CppDefineTypeName.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}[_k] = _v;}}}}";

        }

        public string Accept(TVector2 type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readVector2({fieldName})) return false;";
        }

        public string Accept(TVector3 type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readVector3({fieldName})) return false;";
        }

        public string Accept(TVector4 type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readVector4({fieldName})) return false;";
        }

        public string Accept(TDateTime type, string bufName, string fieldName)
        {
            return $"if(!{bufName}.readLong({fieldName})) return false;";
        }
    }
}
