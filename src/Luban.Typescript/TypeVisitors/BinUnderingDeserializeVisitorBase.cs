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

using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public abstract class BinUnderingDeserializeVisitorBase : ITypeFuncVisitor<string, string, int, string>
{
    public string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readBool()";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readByte()";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readShort()";
    }

    public string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readInt()";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.{(type.IsBigInt ? "readLong" : "readLongAsNumber")}()";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readFloat()";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readDouble()";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readInt()";
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readString()";
    }

    public abstract string Accept(TBean type, string bufVarName, string fieldName, int depth);

    public string Accept(TArray type, string bufVarName, string fieldName, int depth)
    {
        return $"{{ let n = Math.min({bufVarName}.readSize(), {bufVarName}.size); {fieldName} = []; for(let i = 0 ; i < n ; i++) {{ let _e{depth} ;{type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {fieldName}.push(_e{depth});}}}}";
    }

    public virtual string Accept(TList type, string bufVarName, string fieldName, int depth)
    {
        return $"{{ let n = Math.min({bufVarName}.readSize(), {bufVarName}.size); {fieldName} = []; for(let i = 0 ; i < n ; i++) {{ let _e{depth}; {type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {fieldName}.push(_e{depth});}}}}";
    }

    public virtual string Accept(TSet type, string bufVarName, string fieldName, int depth)
    {
        return $"{{ let n = Math.min({bufVarName}.readSize(), {bufVarName}.size); {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(let i = 0 ; i < n ; i++) {{ let _e{depth}; {type.ElementType.Apply(this, bufVarName, $"_e{depth}", depth + 1)}; {fieldName}.add(_e{depth});}}}}";
    }

    public virtual string Accept(TMap type, string bufVarName, string fieldName, int depth)
    {
        return $"{{ let n = Math.min({bufVarName}.readSize(), {bufVarName}.size); {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(let i = 0 ; i < n ; i++) {{ let _k{depth}; {type.KeyType.Apply(this, bufVarName, $"_k{depth}", depth + 1)};  let _v{depth};  {type.ValueType.Apply(this, bufVarName, $"_v{depth}", depth + 1)}; {fieldName}.set(_k{depth}, _v{depth});  }} }}";
    }

    public string Accept(TDateTime type, string bufVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufVarName}.readLongAsNumber()";
    }
}
