// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class DataTargetGoldenTests
{
    public static IEnumerable<object[]> TextTargets()
    {
        // Prefer text formats for readable goldens; binary formats use hash compare.
        yield return new object[] { "json" };
        yield return new object[] { "json2" };
        yield return new object[] { "xml" };
        yield return new object[] { "yaml" };
        yield return new object[] { "lua" };
        yield return new object[] { "bin" };
        yield return new object[] { "bin-offset" };
        yield return new object[] { "bson" };
        yield return new object[] { "msgpack" };
        yield return new object[] { "protobuf2-json" };
        yield return new object[] { "protobuf3-json" };
        yield return new object[] { "flatbuffers-json" };
    }

    [Theory]
    [MemberData(nameof(TextTargets))]
    public void Core_DataTarget_MatchesGolden(string dataTarget)
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("core"),
            dataTargets: new[] { dataTarget },
            outputDataDir: ws.DataDir);

        Assert.True(Directory.GetFiles(ws.DataDir, "*", SearchOption.AllDirectories).Length > 0,
            $"No output files for data target {dataTarget}");
        GoldenAssert.AssertDirectoryMatches(ws.DataDir, $"core/{dataTarget}");
    }

    [Fact]
    public void Core_JsonConvert_ProducesOutput()
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("core"),
            dataTargets: new[] { "json-convert" },
            outputDataDir: ws.DataDir);
        Assert.True(Directory.GetFiles(ws.DataDir, "*", SearchOption.AllDirectories).Length > 0);
        GoldenAssert.AssertDirectoryMatches(ws.DataDir, "core/json-convert");
    }

    [Theory]
    [InlineData("protobuf2-bin")]
    [InlineData("protobuf3-bin")]
    public void Core_ProtobufBin_ProducesOutput(string dataTarget)
    {
        using var ws = new TempWorkspace();
        LubanRunner.Run(
            TestPaths.FixtureConf("core"),
            dataTargets: new[] { dataTarget },
            outputDataDir: ws.DataDir);
        Assert.True(Directory.GetFiles(ws.DataDir, "*", SearchOption.AllDirectories).Length > 0);
        GoldenAssert.AssertDirectoryMatches(ws.DataDir, $"core/{dataTarget}");
    }
}
