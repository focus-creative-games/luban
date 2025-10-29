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

using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;

namespace Luban.Defs;

public abstract class DefTypeBase
{
    public DefAssembly Assembly { get; set; }

    public string Name { get; set; }

    public string Namespace { get; set; }

    public string FullName => TypeUtil.MakeFullName(Namespace, Name);

    public string NamespaceWithTopModule => TypeUtil.MakeNamespace(GenerationContext.Current.TopModule, Namespace);

    public string FullNameWithTopModule => TypeUtil.MakeNamespace(GenerationContext.Current.TopModule, FullName);

    public List<string> Groups { get; set; }

    public string Comment { get; protected set; }

    public Dictionary<string, string> Tags { get; protected set; }

    public List<TypeMapper> TypeMappers { get; protected set; }

    public bool HasTag(string attrName)
    {
        return Tags != null && Tags.ContainsKey(attrName);
    }

    public string GetTag(string attrName)
    {
        return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
    }

    public virtual void PreCompile()
    {
        if (Groups != null && Groups.Count > 0)
        {
            LubanConfig c = GenerationContext.GlobalConf;
            if (Groups.Contains("*"))
            {
                Groups.Clear();
                Groups.AddRange(c.Groups.SelectMany(g => g.Names));
            }
            else
            {
                foreach (var g in Groups)
                {
                    if (c.Groups.All(gg => !gg.Names.Contains(g)))
                    {
                        throw new Exception($"type:{FullName} group:{g} not found");
                    }
                }
            }
        }
    }

    public abstract void Compile();

    public virtual void PostCompile() { }
}
