namespace Luban.Pipeline;

[AttributeUsage(AttributeTargets.Class)]
public class PipelineAttribute : Attribute
{
    public string Name { get; }
    
    public PipelineAttribute(string name)
    {
        Name = name;
    }
}