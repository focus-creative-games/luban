// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class DataLoaderTests
{
    [Theory]
    [InlineData("loaders/csv")]
    [InlineData("loaders/tsv")]
    [InlineData("loaders/json")]
    [InlineData("loaders/xml")]
    [InlineData("loaders/yml")]
    [InlineData("loaders/lua")]
    [InlineData("loaders/lit")]
    public void Loader_ExportsEquivalentJson(string fixture)
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf(fixture),
            dataTargets: new[] { "json" },
            outputDataDir: ws.DataDir);

        Assert.True(File.Exists(Path.Combine(ws.DataDir, "tbitem.json"))
                    || Directory.GetFiles(ws.DataDir, "*.json").Length > 0);

        // Normalize by loading through csharp
        LubanRunner.Run(
            TestPaths.FixtureConf(fixture),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir);

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.Equal("sword", CsharpCompileHarness.GetProp<string>(item, "Name"));
        Assert.Equal(100, CsharpCompileHarness.GetProp<int>(item, "Price"));
    }
}
