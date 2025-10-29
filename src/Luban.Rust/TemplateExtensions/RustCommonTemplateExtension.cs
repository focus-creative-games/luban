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

﻿using Luban.Defs;
using Luban.Rust.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Rust.TemplateExtensions;

public class RustCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type?.Apply(RustDeclaringBoxTypeNameVisitor.Ins) ?? string.Empty;
    }

    public static string GetterName(string name)
    {
        return "get_" + name;
    }

    public static string FullName(DefTypeBase type)
    {
        return $"crate::{type.FullName.Replace(".", "::")}";
    }

    public static string BaseTraitName(DefBean bean)
    {
        if (!bean.IsAbstractType)
        {
            return string.Empty;
        }

        var name = $"crate::{bean.FullName.Replace(".", "::")}";
        return name.Insert(name.Length - bean.Name.Length, "T");

    }
}
