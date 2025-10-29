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

using System.Text;
using Luban.Cpp.TypeVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Cpp.TemplateExtensions;

public class CppTemplateExtension : ScriptObject
{
    public static string MakeTypeCppName(DefTypeBase type)
    {
        return TypeUtil.MakeCppFullName(type.Namespace, type.Name);
    }

    public static string MakeCppName(string typeName)
    {
        return TypeUtil.MakeCppFullName("", typeName);
    }

    public static string GetterName(string originName)
    {
        var words = originName.Split('_').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        var s = new StringBuilder("get");
        foreach (var word in words)
        {
            s.Append(TypeUtil.UpperCaseFirstChar(word));
        }
        return s.ToString();
    }

    public static string NamespaceWithGraceBegin(string ns)
    {
        return TypeUtil.MakeCppNamespaceBegin(ns);
    }

    public static string NamespaceWithGraceEnd(string ns)
    {
        return TypeUtil.MakeCppNamespaceEnd(ns);
    }

    public static string GetValueOfNullableType(TType type, string varName)
    {
        return $"(*({varName}))";
    }

}
