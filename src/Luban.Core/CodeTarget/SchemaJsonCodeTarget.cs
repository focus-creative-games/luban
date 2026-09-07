// Copyright 2025 Code Philosophy
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

using Luban.Defs;
using Luban.Schema;
using System.Collections.Generic;

namespace Luban.CodeTarget;

/// <summary>
/// Export compiled schema as schema.json. Does not require data loading.
/// Usage: -c schema-json -x outputCodeDir=... [-x schema-json.outputFile=schema.json]
/// </summary>
[CodeTarget("schema-json")]
public class SchemaJsonCodeTarget : CodeTargetBase
{
    private static readonly HashSet<string> s_emptyKeywords = new();

    public override string FileHeader => "";

    protected override string FileSuffixName => "json";

    protected override IReadOnlySet<string> PreservedKeyWords => s_emptyKeywords;

    public override void ValidateDefinition(GenerationContext ctx)
    {
    }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        string outputFile = EnvManager.Current.GetOptionOrDefault(Name, "outputFile", true, "schema.json");
        manifest.AddFile(CreateOutputFile(outputFile, SchemaJsonExporter.Export(ctx)));
    }

    public override void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer)
    {
    }

    public override void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer)
    {
    }

    public override void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
    }

    public override void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer)
    {
    }
}
