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

using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.FlatBuffers.TemplateExtensions;
using Luban.FlatBuffers.TypeVisitors;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.FlatBuffers.CodeTarget;

[CodeTarget("flatbuffers")]
public class FlatBuffersSchemaTarget : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "fbs";

    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.NoneCodeStyle;

    protected override string DefaultOutputFileName => "schema.fbs";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // flatbuffers schema preserved key words
        "namespace", "enum", "struct", "union", "table", "root_table", "rpc_service", "file_extension", "attribute", "deprecated", "force_align", "include"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new FlatBuffersTemplateExtension());

        var maps = CollectKeyValueEntry(GenerationContext.Current.ExportBeans).KeyValueEntries.Values;
        ctx.PushGlobal(new ScriptObject()
        {
            {"__maps", maps},
        });
    }

    private MapKeyValueEntryCollection CollectKeyValueEntry(List<DefBean> beans)
    {
        var c = new MapKeyValueEntryCollection();

        foreach (DefBean bean in beans)
        {
            CollectMapKeyValueEntriesVisitor.Ins.Accept(bean, c);
        }

        return c;
    }
}
