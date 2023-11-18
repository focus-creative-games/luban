using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class ContextTemplateExtension : ScriptObject
{


    public static bool HasTag(dynamic obj, string attrName)
    {
        return obj.HasTag(attrName);
    }

    public static string GetTag(dynamic obj, string attrName)
    {
        return obj.GetTag(attrName);
    }

    public static bool HasOption(string name)
    {
        return EnvManager.Current.HasOptionRaw(name);
    }

    public static string GetOption(string name)
    {
        return EnvManager.Current.GetOptionRaw(name);
    }

    public static string GetOptionOrDefault(string name, string defaultValue)
    {
        return EnvManager.Current.GetOptionOrDefaultRaw(name, defaultValue);
    }
}
