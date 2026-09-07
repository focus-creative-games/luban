// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class CrossCuttingTests
{
    [Fact]
    public void L10n_TextList_And_Validate()
    {
        using var ws = new TempWorkspace();
        var texts = Path.Combine(TestPaths.Fixture("l10n"), "Data", "texts.json");
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("l10n"),
            dataTargets: new[] { "json", "text-list" },
            outputDataDir: ws.DataDir,
            forceLoadTableDatas: true,
            extraOptions: new Dictionary<string, string>
            {
                ["l10n.provider"] = "default",
                ["l10n.textFile.path"] = $"*@{texts}",
                ["l10n.textFile.keyFieldName"] = "key",
                ["l10n.textListFile"] = "texts.txt",
                // Concurrent data targets share one output dir; disable cleanup so they don't delete each other.
                ["outputSaver.json.cleanUpOutputDir"] = "false",
                ["outputSaver.text-list.cleanUpOutputDir"] = "false",
            });
        Assert.False(result.AnyValidatorFail);
        Assert.True(Directory.GetFiles(ws.DataDir, "*", SearchOption.AllDirectories).Length > 0);
        GoldenAssert.AssertDirectoryMatches(ws.DataDir, "l10n/default");
    }

    [Fact]
    public void Tags_ExcludeDev_DropsTaggedRecord()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("tags_groups"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            excludeTags: new[] { "dev" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        Assert.Null(CsharpCompileHarness.Invoke(tbItem, "GetOrDefault", 1));
        Assert.NotNull(CsharpCompileHarness.Invoke(tbItem, "GetOrDefault", 2));
    }

    [Fact]
    public void Groups_ServerTarget_ExportsServerTables()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("tags_groups"),
            target: "server",
            dataTargets: new[] { "json" },
            outputDataDir: ws.DataDir);

        var files = Directory.GetFiles(ws.DataDir, "*.json").Select(Path.GetFileNameWithoutExtension).ToHashSet(StringComparer.OrdinalIgnoreCase);
        Assert.Contains("tbitem", files);
        Assert.Contains("tbserveritem", files);
        Assert.DoesNotContain("tbclientitem", files);
    }

    [Fact]
    public void Variant_Zh_SelectsLocalizedName()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("variants"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir,
            variants: new Dictionary<string, string> { ["Item.name"] = "zh" });

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("中文", CsharpCompileHarness.GetProp<string>(item, "Name"));
    }
}
