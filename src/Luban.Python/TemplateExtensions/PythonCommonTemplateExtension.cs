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

using Luban.Defs;
using Luban.Python.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Python.TemplateExtensions;

public class PythonCommonTemplateExtension : ScriptObject
{

    public static string FullName(DefTypeBase type)
    {
        return TypeUtil.MakePyFullName(type.Namespace, type.Name);
    }

    public static string StrFullName(string fullName)
    {
        return fullName.Replace(".", "_");
    }

    public static string Deserialize(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName} != None: {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName, 0)}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName, 0);
        }
    }

    public static string DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName}.get('{jsonFieldName}') != None: {type.Apply(JsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName, 0)}";
        }
        else
        {
            return type.Apply(JsonUnderlyingDeserializeVisitor.Ins, $"{jsonVarName}['{jsonFieldName}']", fieldName, 0);
        }
    }
}
