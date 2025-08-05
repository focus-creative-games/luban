using Luban.Types;
using Luban.Javascript.TypeVisitors;
using Scriban.Runtime;

namespace Luban.Javascript.TemplateExtensions;

public class JavascriptJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        return type.Apply(JsonDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }
}
