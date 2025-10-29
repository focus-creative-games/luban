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

using Luban.CodeTarget;
using Luban.CSharp.TypeVisitors;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class RustBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string bufName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = if {bufName}.read_bool() {{ Some({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)}) }} else {{ None }};";
        }
        else
        {
            return $"let {fieldName} = {type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)};";
        }
    }

    public static string DeserializeRow(string fieldName, string bufName, TType type)
    {
        if (type.IsNullable)
        {
            return $"let mut {fieldName} = if {bufName}.read_bool() {{ Some({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)}) }} else {{ None }};";
        }
        else
        {
            if (type is TBean { IsDynamic: true })
            {
                return $"let {fieldName} = {type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)};";
            }

            return $"let {fieldName} = std::sync::Arc::new({type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, bufName, fieldName, 0)});";
        }
    }
}
