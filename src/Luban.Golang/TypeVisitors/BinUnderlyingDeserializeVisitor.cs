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

using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

public class BinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string, int, string>
{
    public static BinUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadBool(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TByte type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadByte(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TShort type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadShort(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TInt type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TLong type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TFloat type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadFloat(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TDouble type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadDouble(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TEnum type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadInt(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TString type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadString(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TDateTime type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {bufName}.ReadLong(); {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    public string Accept(TBean type, string fieldName, string bufName, string err, int depth)
    {
        return $"{{ if {fieldName}, {err} = {($"New{GoCommonTemplateExtension.FullName(type.DefBean)}({bufName})")}; {err} != nil {{ {err} = errors.New(\"error\"); return }} }}";
    }

    private string GenList(TType elementType, string fieldName, string bufName, string err, int depth)
    {
        return $@"{{{fieldName} = make([]{elementType.Apply(DeclaringTypeNameVisitor.Ins)}, 0); var _n{depth}_ int; if _n{depth}_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""error""); return}}; for i{depth} := 0 ; i{depth} < _n{depth}_ ; i{depth}++ {{ var _e{depth}_ {elementType.Apply(DeclaringTypeNameVisitor.Ins)}; {elementType.Apply(DeserializeBinVisitor.Ins, $@"_e{depth}_", bufName, err, depth + 1)}; {fieldName} = append({fieldName}, _e{depth}_) }} }}";
    }

    public string Accept(TArray type, string fieldName, string bufName, string err, int depth)
    {
        return GenList(type.ElementType, fieldName, bufName, err, depth);
    }

    public string Accept(TList type, string fieldName, string bufName, string err, int depth)
    {
        return GenList(type.ElementType, fieldName, bufName, err, depth);
    }

    public string Accept(TSet type, string fieldName, string bufName, string err, int depth)
    {
        return GenList(type.ElementType, fieldName, bufName, err, depth);
    }

    public string Accept(TMap type, string fieldName, string bufName, string err, int depth)
    {
        return $@"{{ {fieldName} = make({type.Apply(DeclaringTypeNameVisitor.Ins)}); var _n{depth}_ int; if _n{depth}_, {err} = {bufName}.ReadSize(); {err} != nil {{ {err} = errors.New(""error""); return}}; for i{depth} := 0 ; i{depth} < _n{depth}_ ; i{depth}++ {{ var _key{depth}_ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(DeserializeBinVisitor.Ins, $@"_key{depth}_", bufName, err, depth + 1)}; var _value{depth}_ {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(DeserializeBinVisitor.Ins, $@"_value{depth}_", bufName, err, depth + 1)}; {fieldName}[_key{depth}_] = _value{depth}_}} }}";
    }
}
