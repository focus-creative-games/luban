using Luban.Cpp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Cpp.TemplateExtensions;

public class CppRawptrBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(CppRawptrDeserializeVisitor.Ins, bufName, fieldName, 0);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(CppRawptrDeclaringTypeNameVisitor.Ins);
    }
}
