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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Luban.Schema;

/// <summary>
/// Export compiled schema as machine-readable JSON for AI / tooling.
/// </summary>
public static class SchemaJsonExporter
{
    public const int FormatVersion = 1;

    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public static string Export(GenerationContext ctx)
    {
        var doc = Build(ctx);
        return JsonSerializer.Serialize(doc, s_options);
    }

    public static SchemaJsonDocument Build(GenerationContext ctx)
    {
        return new SchemaJsonDocument
        {
            Version = FormatVersion,
            Target = ctx.Target.Name,
            Manager = ctx.Target.Manager,
            TopModule = ctx.Target.TopModule,
            Groups = ctx.Target.Groups?.ToList() ?? new List<string>(),
            Tables = ctx.ExportTables.Select(ToTable).ToList(),
            Beans = ctx.ExportBeans.Select(ToBean).ToList(),
            Enums = ctx.ExportEnums.Select(ToEnum).ToList(),
        };
    }

    private static SchemaJsonTable ToTable(DefTable t)
    {
        return new SchemaJsonTable
        {
            FullName = t.FullName,
            Name = t.Name,
            Namespace = t.Namespace,
            ValueType = t.ValueType,
            Mode = t.Mode.ToString().ToLowerInvariant(),
            Index = t.Index,
            InputFiles = t.InputFiles?.ToList() ?? new List<string>(),
            Groups = t.Groups?.ToList() ?? new List<string>(),
            Comment = EmptyToNull(t.Comment),
            ReadSchemaFromFile = t.ReadSchemaFromFile,
            OutputDataFile = t.OutputDataFile,
            Tags = TagsOrNull(t.Tags),
            Variant = EmptyToNull(t.CurrentVariant),
        };
    }

    private static SchemaJsonBean ToBean(DefBean b)
    {
        return new SchemaJsonBean
        {
            FullName = b.FullName,
            Name = b.Name,
            Namespace = b.Namespace,
            Parent = EmptyToNull(b.Parent),
            IsAbstract = b.IsAbstractType,
            Alias = EmptyToNull(b.Alias),
            Comment = EmptyToNull(b.Comment),
            Groups = b.Groups?.ToList() ?? new List<string>(),
            Tags = TagsOrNull(b.Tags),
            Fields = b.HierarchyFields.Select(ToField).ToList(),
            Children = b.Children?.Select(c => c.FullName).ToList(),
        };
    }

    private static SchemaJsonField ToField(DefField f)
    {
        return new SchemaJsonField
        {
            Name = f.Name,
            Type = f.Type,
            Alias = EmptyToNull(f.Alias),
            Comment = EmptyToNull(f.Comment),
            Groups = f.Groups?.ToList() ?? new List<string>(),
            Tags = TagsOrNull(f.Tags),
            HostType = f.HostType?.FullName,
            Variants = f.Variants is { Count: > 0 } ? f.Variants.ToList() : null,
        };
    }

    private static SchemaJsonEnum ToEnum(DefEnum e)
    {
        return new SchemaJsonEnum
        {
            FullName = e.FullName,
            Name = e.Name,
            Namespace = e.Namespace,
            IsFlags = e.IsFlags,
            Comment = EmptyToNull(e.Comment),
            Groups = e.Groups?.ToList() ?? new List<string>(),
            Tags = TagsOrNull(e.Tags),
            Items = e.Items.Select(i => new SchemaJsonEnumItem
            {
                Name = i.Name,
                Value = i.IntValue,
                Alias = EmptyToNull(i.Alias),
                Comment = EmptyToNull(i.Comment),
                Tags = TagsOrNull(i.Tags),
            }).ToList(),
        };
    }

    private static string EmptyToNull(string s) => string.IsNullOrWhiteSpace(s) ? null : s;

    private static Dictionary<string, string> TagsOrNull(Dictionary<string, string> tags)
        => tags == null || tags.Count == 0 ? null : new Dictionary<string, string>(tags);
}

public sealed class SchemaJsonDocument
{
    public int Version { get; set; }
    public string Target { get; set; }
    public string Manager { get; set; }
    public string TopModule { get; set; }
    public List<string> Groups { get; set; }
    public List<SchemaJsonTable> Tables { get; set; }
    public List<SchemaJsonBean> Beans { get; set; }
    public List<SchemaJsonEnum> Enums { get; set; }
}

public sealed class SchemaJsonTable
{
    public string FullName { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string ValueType { get; set; }
    public string Mode { get; set; }
    public string Index { get; set; }
    public List<string> InputFiles { get; set; }
    public List<string> Groups { get; set; }
    public string Comment { get; set; }
    public bool ReadSchemaFromFile { get; set; }
    public string OutputDataFile { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public string Variant { get; set; }
}

public sealed class SchemaJsonBean
{
    public string FullName { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string Parent { get; set; }
    public bool IsAbstract { get; set; }
    public string Alias { get; set; }
    public string Comment { get; set; }
    public List<string> Groups { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public List<SchemaJsonField> Fields { get; set; }
    public List<string> Children { get; set; }
}

public sealed class SchemaJsonField
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Alias { get; set; }
    public string Comment { get; set; }
    public List<string> Groups { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public string HostType { get; set; }
    public List<string> Variants { get; set; }
}

public sealed class SchemaJsonEnum
{
    public string FullName { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public bool IsFlags { get; set; }
    public string Comment { get; set; }
    public List<string> Groups { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public List<SchemaJsonEnumItem> Items { get; set; }
}

public sealed class SchemaJsonEnumItem
{
    public string Name { get; set; }
    public int Value { get; set; }
    public string Alias { get; set; }
    public string Comment { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}
