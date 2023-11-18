using Luban.Java.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.Java.TemplateExtensions;

public class JavaBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(JavaBinDeserializeVisitor.Ins, bufName, fieldName);
    }
}
