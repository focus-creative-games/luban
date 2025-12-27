using Luban.Kotlin.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Kotlin.TemplateExtensions;

public class KotlinJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        return type.Apply(KotlinJsonUnderlyingDeserializeVisitor.Ins, jsonVar, fieldName, 0);
    }

    public static string DeserializeField(string fieldName, string jsonName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"run {{ if ({jsonName}.has(\"{jsonFieldName}\") && !{jsonName}.get(\"{jsonFieldName}\").isJsonNull) {{ {type.Apply(KotlinJsonUnderlyingDeserializeVisitor.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0)} }} else {{ {fieldName} = null }} }}";
        }
        else
        {
            // 对于非空字段，如果字段缺失或为null，应该抛出异常
            return $"run {{ if ({jsonName}.has(\"{jsonFieldName}\") && !{jsonName}.get(\"{jsonFieldName}\").isJsonNull) {{ {type.Apply(KotlinJsonUnderlyingDeserializeVisitor.Ins, $"{jsonName}.get(\"{jsonFieldName}\")", fieldName, 0)} }} else {{ throw IllegalArgumentException(\"Required field '{jsonFieldName}' is missing or null\") }} }}";
        }
    }

    public static string JsonFieldName(string originName)
    {
        return $"\"{originName}\"";
    }
}