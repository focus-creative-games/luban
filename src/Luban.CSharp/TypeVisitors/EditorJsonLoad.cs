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

class EditorJsonLoad : ITypeFuncVisitor<string, string, int, string>
{
    public static EditorJsonLoad Ins { get; } = new();

    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"if(!{json}.IsBoolean) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        return $"if({json}.IsString) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})System.Enum.Parse(typeof({type.Apply(EditorDeclaringTypeNameVisitor.Ins)}), {json}); }} else if({json}.IsNumber) {{ {x} = ({type.Apply(EditorDeclaringTypeNameVisitor.Ins)})(int){json}; }} else {{ throw new SerializationException(); }}  ";
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        return $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        return $"if(!{json}.IsObject) {{ throw new SerializationException(); }}  {x} = {type.Apply(EditorUnderlyingTypeNameVisitor.Ins)}.LoadJson{type.DefBean.Name}({json});";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} int _n{depth} = {json}.Count; {x} = new {BinaryUnderlyingDeserializeVisitor.CreateNewArrayWithSize(type, $"_n{depth}")}; int _index{depth}=0; foreach(SimpleJSON.JSONNode __e{depth} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v{depth};  {type.ElementType.Apply(this, $"__e{depth}", $"__v{depth}", depth + 1)}  {x}[_index{depth}++] = __v{depth}; }}  ";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e{depth} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v{depth};  {type.ElementType.Apply(this, $"__e{depth}", $"__v{depth}", depth + 1)}  {x}.Add(__v{depth}); }}  ";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        return $"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e{depth} in {json}.Children) {{ {type.ElementType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v{depth};  {type.ElementType.Apply(this, $"__e{depth}", $"__v{depth}", depth + 1)}  {x}.Add(__v{depth}); }}  ";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        return @$"if(!{json}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.Apply(EditorDeclaringTypeNameVisitor.Ins)}(); foreach(JSONNode __e{depth} in {json}.Children) {{ {type.KeyType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __k{depth};  {type.KeyType.Apply(this, $"__e{depth}[0]", $"__k{depth}", depth + 1)} {type.ValueType.Apply(EditorDeclaringTypeNameVisitor.Ins)} __v{depth};  {type.ValueType.Apply(this, $"__e{depth}[1]", $"__v{depth}", depth + 1)}  {x}.Add(__k{depth}, __v{depth}); }}  ";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json};";
    }
}
