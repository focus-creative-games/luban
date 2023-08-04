using Luban.CodeTarget.CSharp.TypeVisitors;
using Luban.Core.CodeFormat;
using Luban.Core.Defs;
using Luban.Core.Types;
using Luban.Core.Utils;
using Scriban.Runtime;

namespace Luban.CodeTarget.CSharp.TemplateExtensions;

public class CsharpSimpleJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"{{ var _j = {bufName}[\"{jsonFieldName}\"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) {{ {type.Apply(SimpleJsonDeserializeVisitor.Ins, "_j", fieldName, 0)} }} else {{ {fieldName} = null; }} }}";
        }
        else
        {
            return type.Apply(SimpleJsonDeserializeVisitor.Ins, $"{bufName}[\"{jsonFieldName}\"]", fieldName, 0);
        }
    }
}