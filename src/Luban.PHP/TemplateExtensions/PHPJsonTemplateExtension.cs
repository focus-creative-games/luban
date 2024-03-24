using Luban.PHP.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.PHP.TemplateExtensions;

public class PHPJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        return type.Apply(JsonDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }
}
