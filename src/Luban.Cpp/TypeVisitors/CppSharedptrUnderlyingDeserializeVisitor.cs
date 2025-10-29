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

using Luban.Cpp.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Cpp.TypeVisitors;

public class CppSharedptrUnderlyingDeserializeVisitor : CppUnderlyingDeserializeVisitorBase
{
    public static CppSharedptrUnderlyingDeserializeVisitor Ins { get; } = new CppSharedptrUnderlyingDeserializeVisitor();

    //public string Accept(TArray type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size()));{fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;{type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    //}

    //public string Accept(TList type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.push_back(_e);}}}}";
    //}

    //public string Accept(TSet type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, ::luban::int32({bufName}.size())); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.ElementType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _e;  {type.ElementType.Apply(this, bufName, "_e")} {fieldName}.insert(_e);}}}}";
    //}

    //public string Accept(TMap type, string bufName, string fieldName)
    //{
    //    return $"{{::luban::int32 n; if(!{bufName}.readSize(n)) return false; n = std::min(n, (::luban::int32){bufName}.size()); {fieldName}.reserve(n * 3 / 2);for(int i = 0 ; i < n ; i++) {{ {type.KeyType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _k;  {type.KeyType.Apply(this, bufName, "_k")} {type.ValueType.Apply(CppSharedptrDeclaringTypeNameVisitor.Ins)} _v;  {type.ValueType.Apply(this, bufName, "_v")}     {fieldName}[_k] = _v;}}}}";

    //}
}
