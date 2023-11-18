using Luban.Java.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Java.TemplateExtensions;

public class JavaJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        return type.Apply(JavaJsonUnderlyingDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }

    public static string DeserializeField(string fieldName, string jsonName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{ if ({jsonName}.has(\"{jsonFieldName}\") && !{jsonName}.get(\"{jsonFieldName}\").isJsonNull()) {{ {type.Apply(JavaJsonUnderlyingDeserializeVisitor.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(TypeVisitors.JavaJsonUnderlyingDeserializeVisitor.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0);
        }
    }
}
