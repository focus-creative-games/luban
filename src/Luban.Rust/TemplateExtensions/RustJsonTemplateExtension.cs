using Luban.Rust.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Rust.TemplateExtensions;

public class RustJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = serde_json::from_value({jsonVarName}.clone())";
        }
        else
        {
            return $"let {fieldName} = {type.Apply(RustJsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{fieldName}\"]", fieldName, 0)};";
        }
    }

    public static string DeserializeRow(string fieldName, string jsonVarName, TType type)
    {
        if (type is TBean {DefBean: {IsAbstractType: true}})
        {
            return $"let {fieldName} = {type.Apply(RustJsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}", fieldName, 0)};";
        }

        return $"let {fieldName} = std::sync::Arc::new({type.Apply(RustJsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}", fieldName, 0)});";
    }

    public static string DeserializeField(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = None; if let Some(value) = {jsonVarName}.get(\"{fieldName}\") {{ {fieldName} = Some({type.Apply(RustJsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{fieldName}\"]", fieldName, 0)}); }}";
        }
        else
        {
            return $"let {fieldName} = {type.Apply(RustJsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{fieldName}\"]", fieldName, 0)};";
        }
    }
}