using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static CppUnderlyingDeserializeVisitor Ins { get; } = new CppUnderlyingDeserializeVisitor();

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

    public string Accept(TInt type, string bufName, string fieldName)
    {
        return $"if(!{bufName}.readInt({fieldName})) return false;";
    }

    public string Accept(TLong type, string bufName, string fieldName)
    {
        return $"if(!{bufName}.readLong({fieldName})) return false;";
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
        return $"{{int __enum_temp__; if(!{bufName}.readInt(__enum_temp__)) return false; {fieldName} = {CppTemplateExtension.MakeTypeCppName(type.DefEnum)}(__enum_temp__); }}";
    }

    public string Accept(TString type, string bufName, string fieldName)
    {
        return $"if(!{bufName}.readString({fieldName})) return false;";
    }

    public string Accept(TBean type, string bufName, string fieldName)
    {
        return $"if(!{CppTemplateExtension.MakeTypeCppName(type.DefBean)}::deserialize{type.DefBean.Name}({bufName}, {fieldName})) return false;";
    }

    public string Accept(TDateTime type, string bufName, string fieldName)
    {
        return $"if(!{bufName}.readLong({fieldName})) return false;";
    }

    public string Accept(TArray type, string bufName, string fieldName)
    {
        return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size()));{fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName)
    {
        return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName)
    {
        return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.insert(_e);}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName)
    {
        return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, (::luban::int32){bufName}.size()); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}[_k] = _v;}}}}";

    }
}