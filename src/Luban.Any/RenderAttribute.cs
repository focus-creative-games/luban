namespace Luban.Any;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RenderAttribute : System.Attribute
{
    public string Name { get; }

    public RenderAttribute(string name)
    {
        Name = name;
    }
}