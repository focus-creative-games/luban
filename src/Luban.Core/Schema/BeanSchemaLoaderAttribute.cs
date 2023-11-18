using Luban.CustomBehaviour;

namespace Luban.Schema;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BeanSchemaLoaderAttribute : BehaviourBaseAttribute
{
    public BeanSchemaLoaderAttribute(string name) : base(name)
    {
    }
}
