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

namespace Luban.CSharp.TypeVisitors;

class EditorJsonSaveUnderlying2 : ITypeFuncVisitor<string, string, int, string>
{
    public static EditorJsonSaveUnderlying2 Ins { get; } = new();

    public string Accept(TBool type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONBool({value});";
    }

    public string Accept(TByte type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TShort type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TInt type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TLong type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TFloat type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TDouble type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber({value});";
    }

    public string Accept(TEnum type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONNumber((int){value});";
    }

    public string Accept(TString type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONString({value});";
    }

    public string Accept(TDateTime type, string varName, string value, int depth)
    {
        return $"{varName} = new JSONString({value});";
    }

    public string Accept(TBean type, string varName, string value, int depth)
    {
        return $"{{ var __bjson{depth} = new JSONObject();  {varName} = __bjson{depth}; {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.SaveJson{type.DefBean.Name}({value}, __bjson{depth}); }}";
    }

    private string AcceptArrayLike(TType elementType, string varName, string value, int depth)
    {
        return $"{{ var __cjson{depth} = new JSONArray(); {varName} = __cjson{depth}; foreach(var _e{depth} in {value}) {{ JSONNode __v{depth}; {elementType.Apply(this, $"__v{depth}", $"_e{depth}", depth + 1)} __cjson{depth}.Add(__v{depth}); }} }}";
    }

    public string Accept(TArray type, string varName, string value, int depth)
    {
        return AcceptArrayLike(type.ElementType, varName, value, depth);
    }

    public string Accept(TList type, string varName, string value, int depth)
    {
        return AcceptArrayLike(type.ElementType, varName, value, depth);
    }

    public string Accept(TSet type, string varName, string value, int depth)
    {
        return AcceptArrayLike(type.ElementType, varName, value, depth);
    }

    public string Accept(TMap type, string varName, string value, int depth)
    {
        return $"{{ var __cjson{depth} = new JSONArray(); {varName} = __cjson{depth}; foreach(var _e{depth} in {value}) {{ var __entry{depth} = new JSONArray(); __cjson{depth}.Add(__entry{depth}); JSONNode __k{depth}; {type.KeyType.Apply(this, $"__k{depth}", $"_e{depth}.Key", depth + 1)} __entry{depth}.Add(__k{depth}); JSONNode __v{depth}; {type.ValueType.Apply(this, $"__v{depth}", $"_e{depth}.Value", depth + 1)} __entry{depth}.Add(__v{depth}); }} }}";
    }
}
