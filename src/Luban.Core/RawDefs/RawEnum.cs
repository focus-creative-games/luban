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

namespace Luban.RawDefs;

public class EnumItem
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public string Value { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }
}

public class RawEnum
{
    public string Namespace { get; set; }

    public string Name { get; set; }

    public string FullName => Namespace.Length > 0 ? Namespace + "." + Name : Name;

    public bool IsFlags { get; set; }

    public bool IsUniqueItemId { get; set; }

    public string Comment { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public List<EnumItem> Items { get; set; }

    public List<string> Groups { get; set; }

    public List<TypeMapper> TypeMappers { get; set; }
}
