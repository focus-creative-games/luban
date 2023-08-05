namespace Luban.Validator;

[AttributeUsage(AttributeTargets.Class)]
public class ValidatorAttribute : Attribute
{
    public string Name { get; }
    
    public ValidatorType Type { get; }
    
    public ValidatorAttribute(string name, ValidatorType type = ValidatorType.Data)
    {
        this.Name = name;
        this.Type = type;
    }
}