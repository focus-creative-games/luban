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

public class RecursiveResolveVisitor : ITypeFuncVisitor<string, string, string>
{
    public static RecursiveResolveVisitor Ins { get; } = new();

    public string Accept(TBool type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type, string fieldName, string tablesName)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type, string fieldName, string tablesName)
    {
        return $"{fieldName}?.Resolve({tablesName});";
    }

    public string Accept(TArray type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TList type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TSet type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}) {{ _e?.Resolve({tablesName}); }}";
    }

    public string Accept(TMap type, string fieldName, string tablesName)
    {
        return $@"foreach(var _e in {fieldName}.Values) {{ _e?.Resolve({tablesName}); }}";
    }
}
