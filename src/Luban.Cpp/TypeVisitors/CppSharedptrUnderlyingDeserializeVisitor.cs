using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppSharedptrUnderlyingDeserializeVisitor : CppUnderlyingDeserializeVisitorBase
{
    public static CppSharedptrUnderlyingDeserializeVisitor Ins { get; } = new CppSharedptrUnderlyingDeserializeVisitor();

    //public string Accept(TArray type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size()));{fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    //}

    //public string Accept(TList type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    //}

    //public string Accept(TSet type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.insert(_e);}}}}";
    //}

    //public string Accept(TMap type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, (::luban::int32){bufName}.size()); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}[_k] = _v;}}}}";

    //}
}
