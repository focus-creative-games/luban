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
using Luban.Pipeline;
using Luban.Schema;
using Xunit;

namespace Luban.Tests.Unit;

public class SchemaSourceTests
{
    public SchemaSourceTests()
    {
        MessageCatalog.Init("en");
    }

    [Fact]
    public void FromPath_ParsesSheetAndKeepsRelativeFile()
    {
        var source = SchemaSource.FromPath("Sheet1@Defines/__tables__.xlsx");
        Assert.NotNull(source);
        Assert.Equal("Defines/__tables__.xlsx", source.File);
        Assert.Equal("Sheet1", source.Sheet);
        Assert.Equal("Sheet1@Defines/__tables__.xlsx", source.Display);
    }

    [Fact]
    public void Create_RelativizesAgainstConfigDir()
    {
        string configDir = Path.Combine(Path.GetTempPath(), "luban-schema-source-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(configDir);
        try
        {
            string abs = Path.Combine(configDir, "Defines", "bean.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(abs)!);
            File.WriteAllText(abs, "<module/>");

            var config = new LubanConfig { ConfigDir = configDir };
            using var scope = PipelineScope.Create(new Dictionary<string, string>(), scanPluginAssemblies: false);
            using (scope.Enter())
            {
                scope.Config = config;
                var source = SchemaSource.Create(abs);
                Assert.Equal("Defines/bean.xml", source.File);
                Assert.Null(source.Sheet);
            }
        }
        finally
        {
            Directory.Delete(configDir, true);
        }
    }

    [Fact]
    public void DiagnosticReport_MapsSchemaOriginToFileAndLocation()
    {
        var origin = SchemaSource.Create("Defines/__tables__.xlsx", "Sheet1");
        var report = DiagnosticReport.FromException(
            new LubanException(origin, "error.def.table.value_type_not_exist", "TbItem", "Item"));

        Assert.Single(report.Errors);
        var item = report.Errors[0];
        Assert.Equal("schema", item.Category);
        Assert.Equal("Defines/__tables__.xlsx", item.File);
        Assert.Equal("Sheet1", item.Location);
    }
}
