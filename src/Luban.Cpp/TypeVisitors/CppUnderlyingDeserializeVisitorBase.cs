using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public abstract class CppUnderlyingDeserializeVisitorBase : ITypeFuncVisitor<string, string, int, ITypeFuncVisitor<string>, string>
{
    public string Accept(TBool type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if (!{bufName}.readBool({fieldName})) return false;";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readByte({fieldName})) return false;";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readShort({fieldName})) return false;";
    }

    public string Accept(TInt type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readInt({fieldName})) return false;";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readLong({fieldName})) return false;";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readFloat({fieldName})) return false;";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readDouble({fieldName})) return false;";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"{{int __enum_temp__; if(!{bufName}.readInt(__enum_temp__)) return false; {fieldName} = {CppTemplateExtension.MakeTypeCppName(type.DefEnum)}(__enum_temp__); }}";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readString({fieldName})) return false;";
    }

    public string Accept(TBean type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{CppTemplateExtension.MakeTypeCppName(type.DefBean)}::deserialize{type.DefBean.Name}({bufName}, {fieldName})) return false;";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        return $"if(!{bufName}.readLong({fieldName})) return false;";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        var suffix = depth == 0 ? "" : $"_{depth}";
        return $"{{::luban::int32 n{suffix}; if(!{bufName}.readSize(n{suffix})) return false; n{suffix} = std::min(n{suffix}, ::luban::int32({bufName}.size())); {fieldName}.reserve(n{suffix});for(int i{suffix} = 0 ; i{suffix} < n{suffix} ; i{suffix}++) {{ {type.ElementType.Apply(typeVisitor)} _e{suffix}; {type.ElementType.Apply(this, bufName, $"_e{suffix}", depth + 1, typeVisitor)} {fieldName}.push_back(_e{suffix});}}}}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        var suffix = depth == 0 ? "" : $"_{depth}";
        return $"{{::luban::int32 n{suffix}; if(!{bufName}.readSize(n{suffix})) return false; n{suffix} = std::min(n{suffix}, ::luban::int32({bufName}.size())); {fieldName}.reserve(n{suffix});for(int i{suffix} = 0 ; i{suffix} < n{suffix} ; i{suffix}++) {{ {type.ElementType.Apply(typeVisitor)} _e{suffix}; {type.ElementType.Apply(this, bufName, $"_e{suffix}", depth + 1, typeVisitor)} {fieldName}.push_back(_e{suffix});}}}}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        var suffix = depth == 0 ? "" : $"_{depth}";
        return $"{{::luban::int32 n{suffix}; if(!{bufName}.readSize(n{suffix})) return false; n{suffix} = std::min(n{suffix}, ::luban::int32({bufName}.size())); {fieldName}.reserve(n{suffix} * 3 / 2);for(int i{suffix} = 0 ; i{suffix} < n{suffix} ; i{suffix}++) {{ {type.ElementType.Apply(typeVisitor)} _e{suffix}; {type.ElementType.Apply(this, bufName, $"_e{suffix}", depth + 1, typeVisitor)} {fieldName}.insert(_e{suffix});}}}}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth, ITypeFuncVisitor<string> typeVisitor)
    {
        var suffix = depth == 0 ? "" : $"_{depth}";
        return $"{{::luban::int32 n{suffix}; if(!{bufName}.readSize(n{suffix})) return false; n{suffix} = std::min(n{suffix}, (::luban::int32){bufName}.size()); {fieldName}.reserve(n{suffix} * 3 / 2);for(int i{suffix} = 0 ; i{suffix} < n{suffix} ; i{suffix}++) {{ {type.KeyType.Apply(typeVisitor)} _k{suffix}; {type.KeyType.Apply(this, bufName, $"_k{suffix}", depth + 1, typeVisitor)} {type.ValueType.Apply(typeVisitor)} _v{suffix}; {type.ValueType.Apply(this, bufName, $"_v{suffix}",depth + 1, typeVisitor)} {fieldName}[_k{suffix}] = _v{suffix};}}}}";
    }
}
