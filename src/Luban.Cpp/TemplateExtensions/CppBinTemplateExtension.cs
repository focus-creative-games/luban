using Luban.Cpp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Cpp.TemplateExtensions;

public class CppBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(CppDeserializeVisitor.Ins, bufName, fieldName);
    }
}