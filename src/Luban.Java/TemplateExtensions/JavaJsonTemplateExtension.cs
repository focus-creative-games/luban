using Luban.Java.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Java.TemplateExtensions;

public class JavaJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{var _j = {jsonVar}; if (_j.ValueKind != JsonValueKind.Null) {{ {type.Apply(JavaJsonDeserialize.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(JavaJsonDeserialize.Ins, jsonVar, fieldName, 0);
        }
    }
    
    public static string DeserializeField(string fieldName, string jsonVar, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{if ({jsonVar}.TryGetProperty(\"{jsonFieldName}\", out var _j) && _j.ValueKind != JsonValueKind.Null) {{ {type.Apply(JavaJsonDeserialize.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(JavaJsonDeserialize.Ins, $"{jsonVar}.GetProperty(\"{jsonFieldName}\")", fieldName, 0);
        }
    }
}