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

using Luban.Defs;
using Luban.Pipeline;
using Luban.RawDefs;

namespace Luban.Schema;

/// <summary>
/// Compile schema into a <see cref="GenerationContext"/> without running code/data targets.
/// Used by agent CLI modes: schema / list-tables / describe.
/// </summary>
public static class SchemaCompilation
{
    public static GenerationContext Compile(PipelineArguments args)
    {
        if (args == null)
        {
            throw new ArgumentNullException(nameof(args));
        }
        if (args.Config == null)
        {
            throw new ArgumentException("PipelineArguments.Config is required", nameof(args));
        }

        string collectorName = string.IsNullOrWhiteSpace(args.SchemaCollector) ? "default" : args.SchemaCollector;
        var schemaCollector = SchemaManager.Ins.CreateSchemaCollector(collectorName);
        schemaCollector.Load(args.Config);
        RawAssembly rawAssembly = schemaCollector.CreateRawAssembly();

        var defAssembly = new DefAssembly(
            rawAssembly,
            args.Target,
            args.OutputTables ?? new List<string>(),
            args.Config.Groups,
            args.Variants);

        var ctx = new GenerationContext();
        ctx.Init(new GenerationContextBuilder
        {
            Assembly = defAssembly,
            IncludeTags = args.IncludeTags,
            ExcludeTags = args.ExcludeTags,
            TimeZone = args.TimeZone,
        });
        return ctx;
    }
}
