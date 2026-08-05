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

using Luban.Gdscript.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using NLog;

namespace Luban.Gdscript.TypeVisitors;

public class BinaryUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string, string, string>
{
    public static BinaryUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TByte type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TShort type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }
    public string Accept(TInt type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TLong type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TFloat type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TDouble type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TEnum type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TString type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TBean type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName});";
    }

    public string Accept(TArray type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName}, {eleTypeVarName});";
    }

    public string Accept(TList type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName}, {eleTypeVarName});";
    }

    public string Accept(TSet type, string bufName, string fieldName, string eleTypeVarName = "", string eleTypeVarName2 = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName}, {eleTypeVarName});";
    }

    public string Accept(TMap type, string bufName, string fieldName, string keyTypeVarName = "", string valTypeVarName = "")
    {
        var funcName = type.Apply(BinaryUnderlyingDeserializeFuncNameVisitor.Ins);
        return $"{fieldName} = {funcName}({bufName}, {keyTypeVarName}, {valTypeVarName});";
    }
}
