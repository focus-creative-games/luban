using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class CsharpSimpleJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVar, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{var _j = {jsonVar}; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) {{ {type.Apply(SimpleJsonDeserializeVisitor.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(SimpleJsonDeserializeVisitor.Ins, jsonVar, fieldName, 0);
        }
    }

    public static string DeserializeField(string fieldName, string jsonVar, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{ var _j = {jsonVar}[\"{jsonFieldName}\"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) {{ {type.Apply(SimpleJsonDeserializeVisitor.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(SimpleJsonDeserializeVisitor.Ins, $"{jsonVar}[\"{jsonFieldName}\"]", fieldName, 0);
        }
    }
}
