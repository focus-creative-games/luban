using Scriban.Runtime;

namespace Luban.Core.TemplateExtensions;

public class ContextTemplateExtends : ScriptObject
{
    

    public static bool HasTag(dynamic obj, string attrName)
    {
        return obj.HasTag(attrName);
    }

    public static string GetTag(dynamic obj, string attrName)
    {
        return obj.GetTag(attrName);
    }

    public static bool HasEnv(string name)
    {
        return GenerationContext.Current.HasEnv(name);
    }

    public static string GetEnv(string name)
    {
        return GenerationContext.Current.GetEnv(name);
    }

    public static string GetEnvOr(string name, string defaultValue)
    {
        return GenerationContext.Current.GetEnvOrDefault(name, defaultValue);
    }
}