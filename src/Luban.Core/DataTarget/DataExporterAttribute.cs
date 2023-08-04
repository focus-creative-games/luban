namespace Luban.DataTarget;

[AttributeUsage(AttributeTargets.Class)]
public class DataExporterAttribute : System.Attribute
{
    public string Name { get; }
    
    
    public DataExporterAttribute(string name)
    {
        Name = name;
    }
}