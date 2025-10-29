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
using Luban.Gdscript.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Gdscript.TemplateExtensions;

public class GdscriptJsonTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string jsonVarName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName} != null: {type.Apply(UnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName)}";
        }
        else
        {
            return type.Apply(UnderlyingDeserializeVisitor.Ins, jsonVarName, fieldName);
        }
    }

    public static string DeserializeField(string fieldName, string jsonVarName, string jsonFieldName, TType type)
    {
        if (type.IsNullable)
        {
            return $"if {jsonVarName}.get('{jsonFieldName}') != null: {type.Apply(UnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{jsonFieldName}\"]", fieldName)}";
        }
        else
        {
            return type.Apply(UnderlyingDeserializeVisitor.Ins, $"{jsonVarName}[\"{jsonFieldName}\"]", fieldName);
        }
    }
}
