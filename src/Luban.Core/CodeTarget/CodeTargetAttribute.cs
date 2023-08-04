namespace Luban.Core.CodeTarget;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class CodeTargetAttribute : Attribute
{
    public string Name { get; }
    
    public CodeTargetAttribute(string name)
    {
        Name = name;
    }
}