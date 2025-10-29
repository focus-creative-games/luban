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

using System.Xml.Linq;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

public static class XmlSchemaUtil
{
    public static void ValidAttrKeys(string defineFile, XElement e, List<string> optionKeys, List<string> requireKeys)
    {
        foreach (var k in e.Attributes())
        {
            var name = k.Name.LocalName;
            if (!requireKeys.Contains(name) && optionKeys != null && !optionKeys.Contains(name))
            {
                throw new LoadDefException($"定义文件:{defineFile} 定义:{e} 包含未知属性 attr:{name}");
            }
        }
        foreach (var k in requireKeys)
        {
            if (e.Attribute(k) == null)
            {
                throw new LoadDefException($"定义文件:{defineFile} 定义:{e} 缺失属性 attr:{k}");
            }
        }
    }

    private static readonly List<string> _refGroupRequireAttrs = new() { "name", "ref" };

    public static RawRefGroup CreateRefGroup(string fileName, XElement e)
    {
        ValidAttrKeys(fileName, e, null, _refGroupRequireAttrs);

        return new RawRefGroup()
        {
            Name = XmlUtil.GetRequiredAttribute(e, "name"),
            Refs = XmlUtil.GetRequiredAttribute(e, "ref").Split(',').Select(s => s.Trim()).ToList(),
        };
    }
}
