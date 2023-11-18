using Luban.Defs;
using Luban.Gdscript.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Gdscript.TemplateExtensions;

public class GdscriptJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName} != null: {type.Apply(UnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName)}";
        }
        else
        {
            return type.Apply(UnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName);
        }
    }

    public static string DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName}.get('{jsonFieldName}') != null: {type.Apply(UnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{jsonFieldName}\"]", fieldName)}";
        }
        else
        {
            return type.Apply(UnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{jsonFieldName}\"]", fieldName);
        }
    }
}
