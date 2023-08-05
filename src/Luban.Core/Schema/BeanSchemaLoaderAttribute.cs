namespace Luban.Schema;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BeanSchemaLoaderAttribute : Attribute
{
    public string Name { get; }
    
    public BeanSchemaLoaderAttribute(string name)
    {
        Name = name;
    }
}