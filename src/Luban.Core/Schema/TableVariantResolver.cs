// Copyright 2026 Code Philosophy
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
using Luban.Utils;

namespace Luban.Schema;

/// <summary>
/// Selects one RawTable per FullName according to --variant options.
/// Must run before readSchemaFromFile.
/// </summary>
public static class TableVariantResolver
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static List<RawTable> Resolve(IReadOnlyList<RawTable> tables, Dictionary<string, string> variants)
    {
        variants ??= new Dictionary<string, string>();

        var groups = new Dictionary<string, List<RawTable>>();
        var order = new List<string>();
        foreach (var table in tables)
        {
            string fullName = TypeUtil.MakeFullName(table.Namespace, table.Name);
            if (!groups.TryGetValue(fullName, out var list))
            {
                list = new List<RawTable>();
                groups.Add(fullName, list);
                order.Add(fullName);
            }
            list.Add(table);
        }

        var result = new List<RawTable>(order.Count);
        foreach (string fullName in order)
        {
            result.Add(ResolveGroup(fullName, groups[fullName], variants));
        }
        return result;
    }

    private static RawTable ResolveGroup(string fullName, List<RawTable> group, Dictionary<string, string> variants)
    {
        RawTable fallback = null;
        var byVariant = new Dictionary<string, RawTable>();
        string tableName = group[0].Name;

        foreach (var table in group)
        {
            var declared = table.Variants;
            if (declared == null || declared.Count == 0)
            {
                if (fallback != null)
                {
                    throw new LubanException("error.def.table.variant_fallback_duplicate", fullName);
                }
                fallback = table;
                continue;
            }

            foreach (string variantName in declared)
            {
                if (!byVariant.TryAdd(variantName, table))
                {
                    throw new LubanException("error.def.table.variant_duplicate", fullName, variantName);
                }
            }
        }

        if (byVariant.Count == 0)
        {
            fallback.CurrentVariant = "";
            return fallback;
        }

        string selected = null;
        if (variants.TryGetValue(fullName, out var byFullName))
        {
            selected = byFullName;
        }
        else if (variants.TryGetValue(tableName, out var byName))
        {
            selected = byName;
        }
        else if (variants.TryGetValue("default", out var byDefault))
        {
            selected = byDefault;
        }

        if (!string.IsNullOrEmpty(selected) && byVariant.TryGetValue(selected, out var chosen))
        {
            chosen.CurrentVariant = selected;
            return chosen;
        }

        if (fallback != null)
        {
            if (string.IsNullOrEmpty(selected))
            {
                s_logger.Warn(MessageCatalog.Format("warn.def.table.variant_not_set", fullName));
            }
            else
            {
                s_logger.Warn(MessageCatalog.Format("warn.def.table.variant_fallback", fullName, selected));
            }
            fallback.CurrentVariant = "";
            return fallback;
        }

        if (string.IsNullOrEmpty(selected))
        {
            throw new LubanException("error.def.table.variant_not_set", fullName);
        }

        throw new LubanException("error.def.table.variant_not_in_list", fullName, selected, string.Join(",", byVariant.Keys));
    }
}
