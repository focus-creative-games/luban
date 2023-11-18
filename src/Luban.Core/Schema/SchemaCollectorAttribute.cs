using Luban.CustomBehaviour;

namespace Luban.Schema;

[AttributeUsage(AttributeTargets.Class)]
public class SchemaCollectorAttribute : BehaviourBaseAttribute
{
    public SchemaCollectorAttribute(string name) : base(name)
    {
    }
}
