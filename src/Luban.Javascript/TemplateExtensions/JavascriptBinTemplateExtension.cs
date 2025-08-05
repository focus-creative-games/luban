using Luban.Types;
using Scriban.Runtime;

namespace Luban.Javascript.TemplateExtensions;

public class JavascriptBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string bufName, TType type)
    {
        return type.Apply(BinDeserializeVisitor.Ins, bufName, fieldName);
    }
}
