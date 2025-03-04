using Luban.Dart.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Dart.TemplateExtensions;

class DartJsonTemplateExtension : ScriptObject
{
    public static string DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $" if ({jsonVarName}.containsKey('{fieldName}')) {{ {type.Apply(JsonUnderlyingDeserializeVisitor.Ins,
                $"{jsonVarName}['{jsonFieldName}']"
                , fieldName, 0)}; }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return $"{type.Apply(JsonUnderlyingDeserializeVisitor.Ins,
                $"{jsonVarName}['{jsonFieldName}']",
                fieldName, 0)};";
        }

    }
}
