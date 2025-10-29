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

using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Cpp.TypeVisitors;

public class CppUnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static CppUnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "::luban::byte";
    }

    public string Accept(TShort type)
    {
        return "::luban::int16";
    }

    public string Accept(TInt type)
    {
        return "::luban::int32";
    }

    public string Accept(TLong type)
    {
        return "::luban::int64";
    }

    public string Accept(TFloat type)
    {
        return "::luban::float32";
    }

    public string Accept(TDouble type)
    {
        return "::luban::float64";
    }

    public string Accept(TEnum type)
    {
        return CppTemplateExtension.MakeTypeCppName(type.DefEnum);
    }

    public string Accept(TString type)
    {
        return "::luban::String";
    }

    public virtual string Accept(TBean type)
    {
        return CppTemplateExtension.MakeTypeCppName(type.DefBean);
    }

    public string Accept(TDateTime type)
    {
        return "::luban::datetime";
    }

    public string Accept(TArray type)
    {
        return $"::luban::Array<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TList type)
    {
        return $"::luban::Vector<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"::luban::HashSet<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"::luban::HashMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }
}
