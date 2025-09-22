using Luban.Kotlin.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Kotlin.TemplateExtensions;

public class KotlinBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(KotlinBinDeserializeVisitor.Ins, bufName, fieldName);
    }
}