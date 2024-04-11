using Luban.Cpp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Cpp.TemplateExtensions;

public class CppSharedptrBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(CppSharedptrDeserializeVisitor.Ins, bufName, fieldName,0);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins);
    }
}
