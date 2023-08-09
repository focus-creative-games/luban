using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class CsharpDotNetJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{ if ({bufName}.TryGetProperty(\"{jsonFieldName}\", out var _j) && _j.ValueKind != JsonValueKind.Null) {{ {type.Apply(DotNetJsonDeserializeVisitor.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(DotNetJsonDeserializeVisitor.Ins, $"{bufName}.GetProperty(\"{jsonFieldName}\")", fieldName, 0);
        }
    }
}