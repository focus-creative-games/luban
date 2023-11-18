namespace Luban.CustomBehaviour;

public abstract class BehaviourBaseAttribute : Attribute, ICustomBehaviour
{
    public string Name { get; }

    public int Priority { get; set; }

    protected BehaviourBaseAttribute(string name)
    {
        Name = name;
        Priority = 0;
    }
}
