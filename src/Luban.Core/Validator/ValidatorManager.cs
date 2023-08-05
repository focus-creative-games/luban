using System.Reflection;

namespace Luban.Validator;

public class ValidatorManager
{
    public static ValidatorManager Ins { get; } = new ValidatorManager();
    
    private readonly Dictionary<string, Func<IDataValidator>> _dataValidators = new Dictionary<string, Func<IDataValidator>>();
    private readonly Dictionary<string, Func<ITableValidator>> _tableValidators = new Dictionary<string, Func<ITableValidator>>();

    public void Init()
    {
        
    }
    
    public IDataValidator CreateDataValidator(string name, string args)
    {
        if (_dataValidators.TryGetValue(name, out var f))
        {
            IDataValidator v = f();
            v.Args = args;
            return v;
        }
        else
        {
            throw new Exception($"not found data validator:{name}");
        }
    }
    
    public bool TryCreateDataValidator(string name, string args, out IDataValidator validator)
    {
        if (_dataValidators.TryGetValue(name, out var f))
        {
            IDataValidator v = f();
            v.Args = args;
            validator = v;
            return true;
        }
        else
        {
            validator = null;
            return false;
        }
    }
    
    public void RegisterDataValidator(string name, Func<IDataValidator> f)
    {
        if (_dataValidators.ContainsKey(name))
        {
            throw new Exception($"duplicate data validator:{name}");
        }
        _dataValidators.Add(name, f);
    }
    
    public ITableValidator CreateTableValidator(string name)
    {
        if (_tableValidators.TryGetValue(name, out var f))
        {
            return f();
        }
        else
        {
            throw new Exception($"not found table validator:{name}");
        }
    }
    
    public void RegisterTableValidator(string name, Func<ITableValidator> f)
    {
        if (_tableValidators.ContainsKey(name))
        {
            throw new Exception($"duplicate table validator:{name}");
        }
        _tableValidators.Add(name, f);
    }
    
    public void ScanRegisterValidators(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsAbstract || t.IsInterface)
            {
                continue;
            }

            if (t.GetCustomAttribute<ValidatorAttribute>() is { } attr)
            {
                if (attr.Type == ValidatorType.Data)
                {
                    RegisterDataValidator(attr.Name, () => (IDataValidator)Activator.CreateInstance(t));
                }
                else if (attr.Type == ValidatorType.Table)
                {
                    RegisterTableValidator(attr.Name, () => (ITableValidator)Activator.CreateInstance(t));
                }
                else
                {
                    throw new Exception($"unknown validator type:{attr.Name}");
                }
            }
        }
    }
}