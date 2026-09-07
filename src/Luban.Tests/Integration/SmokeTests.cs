// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class SmokeTests
{
    [Fact]
    public void Smoke_Json_DataTarget_MatchesGolden()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("smoke"),
            dataTargets: new[] { "json" },
            outputDataDir: ws.DataDir);

        GoldenAssert.AssertDirectoryMatches(ws.DataDir, "smoke/json");
    }

    [Fact]
    public void Smoke_CsDotnetJson_LoadsAndReadsFields()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("smoke"),
            codeTargets: new[] { "cs-dotnet-json" },
            dataTargets: new[] { "json" },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir);

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, "cs-dotnet-json");
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.NotNull(item);
        Assert.Equal("sword", CsharpCompileHarness.GetProp<string>(item, "Name"));
        Assert.Equal(100, CsharpCompileHarness.GetProp<int>(item, "Price"));
    }
}
