using Luban.Types;
using Luban.Typescript.TypeVisitors;
using Scriban.Runtime;

namespace Luban.Typescript.TemplateExtensions;

public class TypescriptJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        return type.Apply(JsonDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }
}
