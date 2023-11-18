using System.Reflection;
using Luban.CustomBehaviour;
using Luban.Defs;
using Luban.Types;

namespace Luban.Validator;

public class ValidatorManager
{
    public static ValidatorManager Ins { get; } = new();

    public void Init()
    {

    }

    public void InitValidatorsRecursive(TType type)
    {
        foreach (var (tagName, tagValue) in type.Tags)
        {
            if (TryCreateDataValidator(tagName, tagValue, out var validator))
            {
                type.Validators.Add(validator);
            }
        }
        if (type is TArray array)
        {
            InitValidatorsRecursive(array.ElementType);
        }
        else if (type is TList list)
        {
            InitValidatorsRecursive(list.ElementType);
        }
        else if (type is TSet set)
        {
            InitValidatorsRecursive(set.ElementType);
        }
        else if (type is TMap map)
        {
            InitValidatorsRecursive(map.KeyType);
            InitValidatorsRecursive(map.ValueType);
        }
    }

    public void CompileValidatorsRecursive(TType type, DefField field)
    {
        foreach (var validator in type.Validators)
        {
            validator.Compile(field, type);
        }
        if (type is TArray array)
        {
            CompileValidatorsRecursive(array.ElementType, field);
        }
        else if (type is TList list)
        {
            CompileValidatorsRecursive(list.ElementType, field);
        }
        else if (type is TSet set)
        {
            CompileValidatorsRecursive(set.ElementType, field);
        }
        else if (type is TMap map)
        {
            CompileValidatorsRecursive(map.KeyType, field);
            CompileValidatorsRecursive(map.ValueType, field);
        }
    }

    public IDataValidator CreateDataValidator(string name, string args)
    {
        var validator = CustomBehaviourManager.Ins.CreateBehaviour<IDataValidator, ValidatorAttribute>(name);
        validator.Args = args;
        return validator;
    }

    public bool TryCreateDataValidator(string name, string args, out IDataValidator validator)
    {
        if (CustomBehaviourManager.Ins.TryCreateBehaviour<IDataValidator, ValidatorAttribute>(name, out validator))
        {
            validator.Args = args;
            return true;
        }
        return false;
    }
}
