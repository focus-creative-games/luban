using Luban.CustomBehaviour;

namespace Luban.DataTarget;

[AttributeUsage(AttributeTargets.Class)]
public class DataExporterAttribute : BehaviourBaseAttribute
{

    public DataExporterAttribute(string name) : base(name)
    {
    }
}
