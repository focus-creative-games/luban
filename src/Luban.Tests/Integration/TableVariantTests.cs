// Copyright 2026 Code Philosophy

using Luban.Diagnostics;
using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class TableVariantTests
{
    [Fact]
    public void SelectZh_LoadsZhData()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["TbItem"] = "zh", ["default"] = "zh" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("中文物品", CsharpCompileHarness.GetProp<string>(item, "Name"));
    }

    [Fact]
    public void SelectEn_UsesDifferentValueType()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "en" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("EnglishItem", CsharpCompileHarness.GetProp<string>(item, "Name"));
        Assert.Equal("en-only-field", CsharpCompileHarness.GetProp<string>(item, "Desc"));
    }

    [Fact]
    public void DefaultEn_OverrideItemZh()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "en", ["TbItem"] = "zh" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("中文物品", CsharpCompileHarness.GetProp<string>(item, "Name"));

        var tbGlobal = CsharpCompileHarness.GetProp<object>(tables, "TbGlobal");
        var global = CsharpCompileHarness.Invoke(tbGlobal, "Get", 1);
        Assert.Equal(200, CsharpCompileHarness.GetProp<int>(global, "Value"));
    }

    [Fact]
    public void NoVariant_UsesFallback()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir);

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("default-item", CsharpCompileHarness.GetProp<string>(item, "Name"));

        var tbGlobal = CsharpCompileHarness.GetProp<object>(tables, "TbGlobal");
        var global = CsharpCompileHarness.Invoke(tbGlobal, "Get", 1);
        Assert.Equal(100, CsharpCompileHarness.GetProp<int>(global, "Value"));
    }

    [Fact]
    public void MissVariant_UsesFallback()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "fr" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("default-item", CsharpCompileHarness.GetProp<string>(item, "Name"));
    }

    [Fact]
    public void SharedMultiVariant_WorksForZhAndEn()
    {
        foreach (var locale in new[] { "zh", "en" })
        {
            using var ws = new TempWorkspace();
            LubanRunner.Run(
                TestPaths.FixtureConf("table-variants"),
                codeTargets: new[] { "cs-dotnet-json" },
                dataTargets: new[] { "json" },
                outputCodeDir: ws.CodeDir,
                outputDataDir: ws.DataDir,
                variants: new Dictionary<string, string> { ["default"] = locale });

            var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
            var tbGlobal = CsharpCompileHarness.GetProp<object>(tables, "TbGlobal");
            var global = CsharpCompileHarness.Invoke(tbGlobal, "Get", 1);
            Assert.Equal(200, CsharpCompileHarness.GetProp<int>(global, "Value"));
        }
    }

    [Fact]
    public void Ref_ResolvesSelectedTable()
    {
        using var ws = new TempWorkspace();
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("table-variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "zh" },
            forceLoadTableDatas: true);
        Assert.False(result.AnyValidatorFail);
    }

    [Fact]
    public void TaggedOnly_WithoutVariant_Fails()
    {
        using var ws = new TempWorkspace();
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("table-variants-tagged-only"),
            dataTargets: new[] { "json" },
            outputDataDir: ws.DataDir,
            throwOnError: false);
        Assert.False(result.Succeeded);
        Assert.IsType<LubanException>(result.Exception);
        Assert.Equal("error.def.table.variant_not_set", ((LubanException)result.Exception).MessageKey);
    }

    [Fact]
    public void TaggedOnly_Mismatch_Fails()
    {
        using var ws = new TempWorkspace();
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("table-variants-tagged-only"),
            dataTargets: new[] { "json" },
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "fr" },
            throwOnError: false);
        Assert.False(result.Succeeded);
        Assert.IsType<LubanException>(result.Exception);
        Assert.Equal("error.def.table.variant_not_in_list", ((LubanException)result.Exception).MessageKey);
    }

    [Fact]
    public void FieldAndTable_ShareDefault()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["default"] = "zh" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("中文", CsharpCompileHarness.GetProp<string>(item, "Name"));
    }
}
