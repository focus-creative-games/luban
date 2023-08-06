namespace Luban;

public class EnvManager
{
    public static EnvManager Current { get; set; }
    
    private readonly Dictionary<string, string> _options;

    public EnvManager(Dictionary<string, string> options)
    {
        _options = options;
    }

    public bool HasOption(string optionName)
    {
        return _options.ContainsKey(optionName);
    }

    public string GetOption(string optionName)
    {
        return _options.TryGetValue(optionName, out var value) ? value : null;
    }

    public string GetOptionOrDefault(string optionName, string defaultValue)
    {
        return _options.TryGetValue(optionName, out var value) ? value : defaultValue;
    }
    

    public string GetOption(string family, string name, bool useGlobalIfNotExits)
    {
        string nameWithFamily = family + "." + name;
        if (_options.TryGetValue(nameWithFamily, out var value))
        {
            return value;
        }
        if (useGlobalIfNotExits && _options.TryGetValue(name, out value))
        {
            return value;
        }
        throw new Exception($"option '{nameWithFamily}' not exists");
    }
    
    public bool TryGetOption(string family, string name, bool useGlobalIfNotExits, out string value)
    {
        string nameWithFamily = family + "." + name;
        if (_options.TryGetValue(nameWithFamily, out value))
        {
            return true;
        }
        if (useGlobalIfNotExits && _options.TryGetValue("global." + name, out value))
        {
            return true;
        }

        return false;
    }
    
    public string GetOptionOrDefault(string family, string name, bool useGlobalIfNotExits, string defaultValue)
    {
        return TryGetOption(family, name, useGlobalIfNotExits, out string value) ? value : defaultValue;
    }
    
    public bool GetBoolOptionOrDefault(string family, string name, bool useGlobalIfNotExits, bool defaultValue)
    {
        if (TryGetOption(family, name, useGlobalIfNotExits, out string value))
        {
            switch (value.ToLowerInvariant())
            {
                case "0":
                case "false": return false;
                case "1":
                case "true": return true;
                default: throw new Exception($"invalid bool option value:{value}");
            }   
        }
        return defaultValue;
    }

}