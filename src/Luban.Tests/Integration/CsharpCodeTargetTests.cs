// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class CsharpCodeTargetTests
{
    [Theory]
    [InlineData("cs-dotnet-json", "json")]
    [InlineData("cs-simple-json", "json")]
    [InlineData("cs-newtonsoft-json", "json")]
    [InlineData("cs-bin", "bin")]
    public void Core_CodeTarget_CompilesAndLoads(string codeTarget, string dataTarget)
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("core"),
            codeTargets: new[] { codeTarget },
            dataTargets: new[] { dataTarget },
            outputCodeDir: ws.CodeDir,
            outputDataDir: ws.DataDir);

        var tables = CsharpCompileHarness.LoadTables(ws.CodeDir, ws.DataDir, codeTarget);
        var tbItem = CsharpCompileHarness.GetProp<object>(tables, "TbItem");
        var item = CsharpCompileHarness.Invoke(tbItem, "Get", 1);
        Assert.NotNull(item);
        Assert.Equal("sword", CsharpCompileHarness.GetProp<string>(item, "Name"));

        var global = CsharpCompileHarness.GetProp<object>(tables, "TbGlobal");
        Assert.Equal(3, CsharpCompileHarness.GetProp<int>(global, "Version"));
        Assert.Equal("demo", CsharpCompileHarness.GetProp<string>(global, "Title"));
    }

    [Fact]
    public void Core_CsEditorJson_GeneratesFiles()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("core"),
            codeTargets: new[] { "cs-editor-json" },
            outputCodeDir: ws.CodeDir);
        Assert.True(Directory.GetFiles(ws.CodeDir, "*.cs", SearchOption.AllDirectories).Length > 0);
    }
}
