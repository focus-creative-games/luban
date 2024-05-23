using Luban.CodeTarget;
using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class RustBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string bufName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = if {bufName}.read_bool() {{ Some({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)}) }} else {{ None }};";
        }
        else
        {
            return $"let {fieldName} = {type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)};";
        }
    }

    public static string DeserializeRow(string fieldName, string bufName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = if {bufName}.read_bool() {{ Some({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)}) }} else {{ None }};";
        }
        else
        {
            if (type is TBean {IsDynamic: true})
            {
                return $"let {fieldName} = {type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)};";
            }

            return $"let {fieldName} = std::sync::Arc::new({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)});";
        }
    }
}