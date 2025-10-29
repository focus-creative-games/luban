// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Luban;

public class EnvManager
{
    public static EnvManager Current { get; set; }

    private readonly Dictionary<string, string> _options;

    public EnvManager(Dictionary<string, string> options)
    {
        _options = options;
    }

    public bool HasOptionRaw(string optionName)
    {
        return _options.ContainsKey(optionName);
    }

    public string GetOptionRaw(string optionName)
    {
        return _options.TryGetValue(optionName, out var value) ? value : null;
    }

    public string GetOptionOrDefaultRaw(string optionName, string defaultValue)
    {
        return _options.TryGetValue(optionName, out var value) ? value : defaultValue;
    }


    public string GetOption(string namespaze, string name, bool useGlobalIfNotExits)
    {
        return TryGetOption(namespaze, name, useGlobalIfNotExits, out var value) ? value : throw new Exception($"option '{name}' not exists");
    }

    public bool TryGetOption(string namespaze, string name, bool useGlobalIfNotExits, out string value)
    {
        while (true)
        {
            string fullOptionName = string.IsNullOrEmpty(namespaze) ? name : namespaze + "." + name;
            if (_options.TryGetValue(fullOptionName, out value))
            {
                return true;
            }

            if (string.IsNullOrEmpty(namespaze) || !useGlobalIfNotExits)
            {
                return false;
            }

            int index = namespaze.LastIndexOf('.');
            if (index < 0)
            {
                namespaze = "";
            }
            else
            {
                namespaze = namespaze.Substring(0, index);
            }
        }
    }

    public string GetOptionOrDefault(string namespaze, string name, bool useGlobalIfNotExits, string defaultValue)
    {
        return TryGetOption(namespaze, name, useGlobalIfNotExits, out string value) ? value : defaultValue;
    }

    public bool GetBoolOptionOrDefault(string namespaze, string name, bool useGlobalIfNotExits, bool defaultValue)
    {
        if (TryGetOption(namespaze, name, useGlobalIfNotExits, out string value))
        {
            switch (value.ToLowerInvariant())
            {
                case "0":
                case "false":
                    return false;
                case "1":
                case "true":
                    return true;
                default:
                    throw new Exception($"invalid bool option value:{value}");
            }
        }
        return defaultValue;
    }

}
