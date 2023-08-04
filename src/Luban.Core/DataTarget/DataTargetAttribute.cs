namespace Luban.DataTarget;

[AttributeUsage(AttributeTargets.Class)]
public class DataTargetAttribute : System.Attribute
{
    public string Name { get; }
    
    public DataTargetAttribute(string name)
    {
        Name = name;
    }
}