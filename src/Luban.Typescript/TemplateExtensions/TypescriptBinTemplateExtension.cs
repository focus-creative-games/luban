using Luban.Types;
using Scriban.Runtime;

namespace Luban.Typescript.TemplateExtensions;

public class TypescriptBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string bufName, TType type)
    {
        return type.Apply(BinDeserializeVisitor.Ins, bufName, fieldName);
    }
}
