namespace Luban.Any.Validators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class ValidatorAttribute : Attribute
{
    public string Name { get; }

    public ValidatorAttribute(string name)
    {
        Name = name;
    }
}