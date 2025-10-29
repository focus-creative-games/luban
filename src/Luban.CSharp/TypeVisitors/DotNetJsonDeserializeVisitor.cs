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

using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class DotNetJsonDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static DotNetJsonDeserializeVisitor Ins { get; } = new();
    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetBoolean();";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetByte();";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetInt16();";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetInt32();";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetInt64();";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetSingle();";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetDouble();";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        return $"{x} = ({type.Apply(DeclaringTypeNameVisitor.Ins)}){json}.GetInt32();";
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetString();";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"{x} = {json}.GetInt64();";
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        string src = $"{CSharpUtil.GetFullNameWithGlobalQualifier(type.DefBean)}.Deserialize{type.DefBean.Name}({json})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{x} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")};";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        string _n = $"_n{depth}";
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        string __index = $"__index{depth}";
        string typeStr = $"{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}[{_n}]";
        if (type.Dimension > 1)
        {
            typeStr = $"{type.FinalElementType.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}[{_n}]";
            for (int i = 0; i < type.Dimension - 1; i++)
            {
                typeStr += "[]";
            }
        }
        return $"{{ var {__json} = {json}; int {_n} = {__json}.GetArrayLength(); {x} = new {typeStr}; int {__index}=0; foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        return $"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __json = $"__json{depth}";
        return $"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        string __e = $"__e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        string __json = $"__json{depth}";
        return @$"{{ var {__json} = {json}; {x} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)} {__k};  {type.KeyType.Apply(this, $"{__e}[0]", __k, depth + 1)} {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)}  {x}.Add({__k}, {__v}); }}   }}";
    }
}
