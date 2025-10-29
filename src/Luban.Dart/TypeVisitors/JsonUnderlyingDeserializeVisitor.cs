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

﻿using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Dart.TypeVisitors;

class JsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string x, string y, int z)
    {
        return $"{y} = {x} as bool";
    }

    public string Accept(TByte type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TShort type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TInt type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TLong type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TFloat type, string x, string y, int z)
    {
        return $"{y} = {x}.toDouble()";
    }

    public string Accept(TDouble type, string x, string y, int z)
    {
        return $"{y} = {x}.toDouble()";
    }

    public string Accept(TEnum type, string x, string y, int z)
    {
        var name = type.DefEnum.Name;
        return $"{y} = {name}.fromValue({x})";
    }

    public string Accept(TString type, string x, string y, int z)
    {
        return $"{y} = {x} as String";
    }

    public string Accept(TDateTime type, string x, string y, int z)
    {
        return $"{y} = {x} as int";
    }

    public string Accept(TBean type, string x, string y, int z)
    {
        return $"{y} = {type.DefBean.Name}.deserialize({x})";
    }

    public string Accept(TArray type, string x, string y, int depth)
    {
        string __j = $"_json{depth}";
        string __v = $"_v{depth}";
        string __e = $"_e{depth}";
        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}.empty(growable: true); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {y}.add({__v}); }} }}";
    }

    public string Accept(TList type, string x, string y, int depth)
    {
        string __j = $"_json{depth}";
        string __v = $"_v{depth}";
        string __e = $"_e{depth}";
        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}.empty(growable: true); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {y}.add({__v}); }} }}";
    }

    public string Accept(TSet type, string x, string y, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        string __j = $"_json{depth}";

        return $"{{var {__j} = {x} as List<dynamic>; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}(); for(var {__e} in {__j}) {{{type.ElementType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v}; {type.ElementType.Apply(this, __e, __v, depth + 1)};  {y}.add({__v}); }} }}";
    }

    public string Accept(TMap type, string x, string y, int depth)
    {
        string __e = $"__e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        string __json = $"__json{depth}";

        return @$"{{ var {__json} = {x}; {y} = {type.Apply(DartDeclaringTypeNameVisitor.Ins)}(); for(var {__e} in {__json}) {{ var {type.KeyType.Apply(this, $"{__e}[0]", __k, depth + 1)}; {type.ValueType.Apply(DartDeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)};  {y}[{__k}]= {__v}; }}   }}";
    }
}
