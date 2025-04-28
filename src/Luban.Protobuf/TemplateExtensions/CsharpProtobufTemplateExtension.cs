using Scriban.Runtime;
namespace Luban.Protobuf.TemplateExtensions;

public class CsharpProtobufTemplateExtension : ScriptObject
{

    public static string NamespaceWithGraceBegin(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return string.Empty;
        }
        ns = ns.Substring(0, 1).ToUpper() + ns.Substring(1);

        return $"namespace {ns}\n{{";
    }

    public static string NamespaceWithGraceEnd(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            return string.Empty;
        }
        return "}";
    }

    public static string ProtoFullName(string typeName)
    {
        var name = string.Join("", typeName.Split('.'));

        return name.Substring(0, 1).ToUpper() + name.Substring(1);
    }
}
