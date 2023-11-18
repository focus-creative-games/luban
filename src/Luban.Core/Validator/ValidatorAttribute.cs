using Luban.CustomBehaviour;

namespace Luban.Validator;

[AttributeUsage(AttributeTargets.Class)]
public class ValidatorAttribute : BehaviourBaseAttribute
{
    public ValidatorType Type { get; }

    public ValidatorAttribute(string name, ValidatorType type = ValidatorType.Data) : base(name)
    {
        this.Type = type;
    }
}
