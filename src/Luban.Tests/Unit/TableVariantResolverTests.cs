// Copyright 2026 Code Philosophy

using Luban.Diagnostics;
using Luban.RawDefs;
using Luban.Schema;
using Xunit;

namespace Luban.Tests.Unit;

public class TableVariantResolverTests
{
    public TableVariantResolverTests()
    {
        MessageCatalog.Init("en");
    }

    private static RawTable T(string name, string variant = null, string input = "a.json", string valueType = "Item")
    {
        return new RawTable
        {
            Name = name,
            Namespace = "",
            ValueType = valueType,
            InputFiles = new List<string> { input },
            Variants = string.IsNullOrWhiteSpace(variant)
                ? new List<string>()
                : variant.Split(',', ';').Select(x => x.Trim()).Where(x => x.Length > 0).ToList(),
        };
    }

    [Fact]
    public void OrdinaryTable_IgnoresDefaultVariant()
    {
        var tables = new List<RawTable> { T("TbAlone") };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "zh" });
        Assert.Single(resolved);
        Assert.Equal("TbAlone", resolved[0].Name);
        Assert.Equal("", resolved[0].CurrentVariant);
    }

    [Fact]
    public void SelectByTableName()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", null, "item.json"),
            T("TbItem", "zh", "item_zh.json"),
            T("TbItem", "en", "item_en.json"),
        };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["TbItem"] = "zh" });
        Assert.Single(resolved);
        Assert.Equal("item_zh.json", resolved[0].InputFiles[0]);
        Assert.Equal("zh", resolved[0].CurrentVariant);
    }

    [Fact]
    public void SelectByDefault_AndOverride()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", null, "item.json"),
            T("TbItem", "zh", "item_zh.json"),
            T("TbItem", "en", "item_en.json"),
            T("TbGlobal", null, "g.json"),
            T("TbGlobal", "zh,en", "g_locale.json"),
        };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string>
        {
            ["default"] = "en",
            ["TbItem"] = "zh",
        });
        Assert.Equal(2, resolved.Count);
        var item = resolved.First(t => t.Name == "TbItem");
        var global = resolved.First(t => t.Name == "TbGlobal");
        Assert.Equal("item_zh.json", item.InputFiles[0]);
        Assert.Equal("zh", item.CurrentVariant);
        Assert.Equal("g_locale.json", global.InputFiles[0]);
        Assert.Equal("en", global.CurrentVariant);
    }

    [Fact]
    public void Fallback_WhenNotSet()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", null, "item.json"),
            T("TbItem", "zh", "item_zh.json"),
        };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string>());
        Assert.Single(resolved);
        Assert.Equal("item.json", resolved[0].InputFiles[0]);
        Assert.Equal("", resolved[0].CurrentVariant);
    }

    [Fact]
    public void Fallback_WhenSelectedMisses()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", null, "item.json"),
            T("TbItem", "zh", "item_zh.json"),
        };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "fr" });
        Assert.Single(resolved);
        Assert.Equal("item.json", resolved[0].InputFiles[0]);
    }

    [Fact]
    public void MultiVariant_SharedTable()
    {
        var tables = new List<RawTable>
        {
            T("TbGlobal", "zh,en", "g.json"),
        };
        var zh = TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "zh" });
        Assert.Equal("zh", zh[0].CurrentVariant);
        var en = TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "en" });
        Assert.Equal("en", en[0].CurrentVariant);
    }

    [Fact]
    public void TaggedOnly_WithoutVariant_Throws()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", "zh"),
            T("TbItem", "en"),
        };
        var ex = Assert.Throws<LubanException>(() =>
            TableVariantResolver.Resolve(tables, new Dictionary<string, string>()));
        Assert.Equal("error.def.table.variant_not_set", ex.MessageKey);
    }

    [Fact]
    public void OrphanTagged_Mismatch_Throws()
    {
        var tables = new List<RawTable> { T("TbItem", "zh") };
        var ex = Assert.Throws<LubanException>(() =>
            TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "en" }));
        Assert.Equal("error.def.table.variant_not_in_list", ex.MessageKey);
    }

    [Fact]
    public void DuplicateVariant_Throws()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", "zh", "a.json"),
            T("TbItem", "zh", "b.json"),
        };
        var ex = Assert.Throws<LubanException>(() =>
            TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "zh" }));
        Assert.Equal("error.def.table.variant_duplicate", ex.MessageKey);
    }

    [Fact]
    public void DuplicateFallback_Throws()
    {
        var tables = new List<RawTable>
        {
            T("TbItem", null, "a.json"),
            T("TbItem", null, "b.json"),
            T("TbItem", "zh", "c.json"),
        };
        var ex = Assert.Throws<LubanException>(() =>
            TableVariantResolver.Resolve(tables, new Dictionary<string, string> { ["default"] = "zh" }));
        Assert.Equal("error.def.table.variant_fallback_duplicate", ex.MessageKey);
    }

    [Fact]
    public void FullName_PreferredOverShortName()
    {
        var tables = new List<RawTable>
        {
            new RawTable
            {
                Namespace = "cfg",
                Name = "TbItem",
                ValueType = "Item",
                InputFiles = new List<string> { "fallback.json" },
                Variants = new List<string>(),
            },
            new RawTable
            {
                Namespace = "cfg",
                Name = "TbItem",
                ValueType = "Item",
                InputFiles = new List<string> { "zh.json" },
                Variants = new List<string> { "zh" },
            },
            new RawTable
            {
                Namespace = "cfg",
                Name = "TbItem",
                ValueType = "Item",
                InputFiles = new List<string> { "en.json" },
                Variants = new List<string> { "en" },
            },
        };
        var resolved = TableVariantResolver.Resolve(tables, new Dictionary<string, string>
        {
            ["TbItem"] = "en",
            ["cfg.TbItem"] = "zh",
        });
        Assert.Equal("zh.json", resolved[0].InputFiles[0]);
    }
}
