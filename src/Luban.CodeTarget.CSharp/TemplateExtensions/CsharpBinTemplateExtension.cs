using Luban.CodeTarget.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CodeTarget.CSharp.TemplateExtensions;

public class CsharpBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string bufName, string fieldName, TType type)
    {
        return type.Apply(BinaryDeserializeVisitor.Ins, bufName, fieldName);
    }
}