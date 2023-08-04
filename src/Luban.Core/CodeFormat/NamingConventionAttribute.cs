namespace Luban.CodeFormat;

[AttributeUsage(AttributeTargets.Class)]
public class NamingConventionAttribute : Attribute
{
    public string Name { get; }
    
    public NamingConventionAttribute(string name)
    {
        Name = name;
    }
}