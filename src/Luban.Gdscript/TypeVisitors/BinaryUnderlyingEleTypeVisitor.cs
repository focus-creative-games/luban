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

namespace Luban.Gdscript.TypeVisitors;

public class BinaryUnderlyingEleTypeVisitor : ITypeFuncVisitor<string>
{
    public static BinaryUnderlyingEleTypeVisitor Ins { get; } = new();

    public string Accept(TBool type) => "LubanEleType.create(LubanEleType.BOOL)";
    public string Accept(TByte type) => "LubanEleType.create(LubanEleType.BYTE)";
    public string Accept(TShort type) => "LubanEleType.create(LubanEleType.SHORT)";
    public string Accept(TInt type) => "LubanEleType.create(LubanEleType.INT)";
    public string Accept(TLong type) => "LubanEleType.create(LubanEleType.LONG)";
    public string Accept(TFloat type) => "LubanEleType.create(LubanEleType.FLOAT)";
    public string Accept(TDouble type) => "LubanEleType.create(LubanEleType.DOUBLE)";
    public string Accept(TEnum type) => "LubanEleType.create(LubanEleType.INT)";
    public string Accept(TString type) => "LubanEleType.create(LubanEleType.STRING)";
    public string Accept(TDateTime type) => "LubanEleType.create(LubanEleType.LONG)";
    public string Accept(TBean type) => $"LubanEleType.create_bean({type.Apply(DeclaringTypeNameVisitor.Ins)}.from_bytes)";
    public string Accept(TArray type) => $"LubanEleType.create_arr({type.ElementType.Apply(BinaryUnderlyingEleTypeVisitor.Ins)})";
    public string Accept(TList type) => $"LubanEleType.create_arr({type.ElementType.Apply(BinaryUnderlyingEleTypeVisitor.Ins)})";
    public string Accept(TSet type) => $"LubanEleType.create_arr({type.ElementType.Apply(BinaryUnderlyingEleTypeVisitor.Ins)})";
    public string Accept(TMap type) => $"LubanEleType.create_map({type.KeyType.Apply(BinaryUnderlyingEleTypeVisitor.Ins)}, {type.ValueType.Apply(BinaryUnderlyingEleTypeVisitor.Ins)})";
}
