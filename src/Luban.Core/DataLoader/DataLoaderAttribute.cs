namespace Luban.DataLoader;

[AttributeUsage(AttributeTargets.Class)]
public class DataLoaderAttribute : Attribute
{
    public string[] LoaderNames { get; }

    public DataLoaderAttribute(params string[] loaderNames)
    {
        LoaderNames = loaderNames;
    }
}