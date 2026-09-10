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

using Luban.Diagnostics;
using Luban.RawDefs;

namespace Luban.Schema;

public abstract class SchemaCollectorBase : ISchemaCollector
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly List<RawEnum> _enums = new();
    private readonly List<RawBean> _beans = new();

    private readonly List<RawTable> _tables = new();

    private readonly List<RawRefGroup> _refGroups = new();

    private readonly Dictionary<string, string> _constAliases = new();

    private Dictionary<string, string> _variants = new();

    protected List<RawTable> Tables => _tables;

    protected IReadOnlyDictionary<string, string> Variants => _variants;

    public abstract void Load(LubanConfig config);

    public abstract RawAssembly CreateRawAssembly();

    public virtual void SetVariants(Dictionary<string, string> variants)
    {
        _variants = variants ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// Resolve table variants in-place. Must be called after all tables are collected
    /// and before readSchemaFromFile.
    /// </summary>
    protected void ResolveTableVariants()
    {
        var resolved = TableVariantResolver.Resolve(_tables, _variants);
        _tables.Clear();
        _tables.AddRange(resolved);
    }

    protected RawAssembly CreateRawAssembly(LubanConfig config)
    {
        return new RawAssembly()
        {
            Tables = _tables,
            Targets = config.Targets.ToList(),
            Groups = config.Groups.ToList(),
            RefGroups = _refGroups,
            Enums = _enums,
            Beans = _beans,
            ConstAliases = _constAliases,
        };
    }

    public void Add(RawTable table)
    {
        lock (this)
        {
            _tables.Add(table);
        }
    }

    public void Add(RawBean bean)
    {
        lock (this)
        {
            _beans.Add(bean);
        }
    }

    public void Add(RawEnum @enum)
    {
        lock (this)
        {
            _enums.Add(@enum);
        }
    }

    public void Add(RawRefGroup refGroup)
    {
        lock (this)
        {
            _refGroups.Add(refGroup);
        }
    }

    public void AddConstAlias(string name, string alias)
    {
        lock (this)
        {
            if (_constAliases.ContainsKey(name))
            {
                s_logger.Warn(MessageCatalog.Format("warn.schema.duplicate_const_alias", name, _constAliases[name], alias));
            }
            _constAliases[name] = alias;
        }
    }
}
