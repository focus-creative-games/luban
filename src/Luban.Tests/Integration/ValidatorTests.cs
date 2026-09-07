// Copyright 2025 Code Philosophy

using Luban.Tests.Helpers;
using Xunit;

namespace Luban.Tests.Integration;

[Collection("LubanPipeline")]
public class ValidatorTests
{
    [Theory]
    [InlineData("validators/ref/pass")]
    [InlineData("validators/range/pass")]
    [InlineData("validators/set/pass")]
    [InlineData("validators/size/pass")]
    [InlineData("validators/index/pass")]
    [InlineData("validators/regex/pass")]
    [InlineData("validators/not_default/pass")]
    public void Validator_Pass(string fixture)
    {
        using var ws = new TempWorkspace();
        var result = LubanRunner.Run(
            TestPaths.FixtureConf(fixture),
            forceLoadTableDatas: true,
            outputDataDir: ws.DataDir);
        Assert.False(result.AnyValidatorFail, $"Expected pass for {fixture}");
    }

    [Theory]
    [InlineData("validators/ref/fail")]
    [InlineData("validators/range/fail")]
    [InlineData("validators/set/fail")]
    [InlineData("validators/size/fail")]
    [InlineData("validators/index/fail")]
    [InlineData("validators/regex/fail")]
    [InlineData("validators/not_default/fail")]
    public void Validator_Fail(string fixture)
    {
        using var ws = new TempWorkspace();
        var result = LubanRunner.Run(
            TestPaths.FixtureConf(fixture),
            forceLoadTableDatas: true,
            outputDataDir: ws.DataDir);
        Assert.True(result.AnyValidatorFail, $"Expected validator failure for {fixture}");
    }

    [Fact]
    public void PathValidator_Pass()
    {
        using var ws = new TempWorkspace();
        var assets = Path.Combine(TestPaths.Fixture("validators/path/pass"), "assets");
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("validators/path/pass"),
            forceLoadTableDatas: true,
            outputDataDir: ws.DataDir,
            extraOptions: new Dictionary<string, string>
            {
                ["pathValidator.rootDir"] = assets,
            });
        Assert.False(result.AnyValidatorFail);
    }

    [Fact]
    public void PathValidator_Fail()
    {
        using var ws = new TempWorkspace();
        var assets = Path.Combine(TestPaths.Fixture("validators/path/fail"), "assets");
        var result = LubanRunner.Run(
            TestPaths.FixtureConf("validators/path/fail"),
            forceLoadTableDatas: true,
            outputDataDir: ws.DataDir,
            extraOptions: new Dictionary<string, string>
            {
                ["pathValidator.rootDir"] = assets,
            });
        Assert.True(result.AnyValidatorFail);
    }
}
