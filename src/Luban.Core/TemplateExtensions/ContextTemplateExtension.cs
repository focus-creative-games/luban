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

using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class ContextTemplateExtension : ScriptObject
{


    public static bool HasTag(dynamic obj, string attrName)
    {
        return obj.HasTag(attrName);
    }

    public static string GetTag(dynamic obj, string attrName)
    {
        return obj.GetTag(attrName);
    }

    public static bool HasOption(string name)
    {
        return EnvManager.Current.HasOptionRaw(name);
    }

    public static string GetOption(string name)
    {
        return EnvManager.Current.GetOptionRaw(name);
    }

    public static string GetOptionOrDefault(string name, string defaultValue)
    {
        return EnvManager.Current.GetOptionOrDefaultRaw(name, defaultValue);
    }
}
