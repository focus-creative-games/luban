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

public class BinaryUnderlyingDeserializeFuncNameVisitor : ITypeFuncVisitor<string>
{
    public static BinaryUnderlyingDeserializeFuncNameVisitor Ins { get; } = new();

    public string Accept(TBool type) => "LubanUtil.read_bool";
    public string Accept(TByte type) => "LubanUtil.read_byte";
    public string Accept(TShort type) => "LubanUtil.read_short";
    public string Accept(TInt type) => "LubanUtil.read_int";
    public string Accept(TLong type) => "LubanUtil.read_long";
    public string Accept(TFloat type) => "LubanUtil.read_float";
    public string Accept(TDouble type) => "LubanUtil.read_double"; 
    public string Accept(TEnum type) => "LubanUtil.read_int";
    public string Accept(TString type) => "LubanUtil.read_string";
    public string Accept(TDateTime type) => "LubanUtil.read_long";
    public string Accept(TBean type) => $"{type.Apply(DeclaringTypeNameVisitor.Ins)}.from_bytes";
    public string Accept(TArray type) => "LubanUtil.read_array";
    public string Accept(TList type) => "LubanUtil.read_array";
    public string Accept(TSet type) => "LubanUtil.read_array";
    public string Accept(TMap type) => "LubanUtil.read_map";
}
