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
using Luban.Javascript.TemplateExtensions;
using Scriban;

namespace Luban.Javascript.CodeTarget;

public abstract class JavascriptCodeTargetBase : AllInOneTemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "js";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.TypescriptDefaultCodeStyle;

    protected override string DefaultOutputFileName => "schema.js";

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // typescript preserved key words
        // remove `type` because it's used frequently
       "class", "function", "var", "let", "const", "if", "else", "switch", "case", "default",
        "for", "while", "do", "break", "continue", "return", "try", "catch", "finally", "throw",
        "new", "delete", "typeof", "instanceof", "void", "this", "super", "extends", "import",
        "export", "from", "as", "in", "of", "await", "async", "yield", "with", "debugger",
        "static", "get", "set",

        "enum", "implements", "interface", "package", "private", "protected", "public"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new JavascriptCommonTemplateExtension());
    }
}
