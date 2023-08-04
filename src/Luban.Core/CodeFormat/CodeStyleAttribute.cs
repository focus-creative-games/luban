namespace Luban.CodeFormat;

[AttributeUsage(AttributeTargets.Class)]
public class CodeStyleAttribute : Attribute
{
    public string Name { get; }
    
    public CodeStyleAttribute(string name)
    {
        Name = name;
    }
}