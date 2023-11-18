namespace Luban.CustomBehaviour;

public interface ICustomBehaviour
{
    public string Name { get; }

    int Priority { get; }
}
