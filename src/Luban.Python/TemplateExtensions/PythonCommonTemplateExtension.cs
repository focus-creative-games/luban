using Luban.Defs;
using Luban.Python.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Python.TemplateExtensions;

public class PythonCommonTemplateExtension : ScriptObject
{

    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakePyFullName(type.Namespace, type.Name);
    }

    public static string StrFullName(string fullName)
    {
        return fullName.Replace(".", "_");
    }

    public static string Deserialize(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName} != None: {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName, 0)}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName, 0);
        }
    }

    public static string DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName}.get('{jsonFieldName}') != None: {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName, 0)}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName, 0);
        }
    }
}
