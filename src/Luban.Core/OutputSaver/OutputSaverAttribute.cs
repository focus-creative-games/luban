namespace Luban.OutputSaver;

[AttributeUsage(AttributeTargets.Class)]
public class OutputSaverAttribute : Attribute
{
    public string Name { get; }
    
    public OutputSaverAttribute(string name)
    {
        Name = name;
    }
}