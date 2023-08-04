namespace Luban.Any.Validators;

public interface IValidator : IProcessor
{
    void Validate(ValidatorContext ctx, TType type, DType data);
}