namespace Luban.Schema;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SchemaLoaderAttribute : System.Attribute
{
    public string Type { get; }

    public string[] ExtNames { get; }

    public int Priority { get; set; }

    public SchemaLoaderAttribute(string type, params string[] extNames)
    {
        Type = type;
        ExtNames = extNames;
    }
}
