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
using Luban.Golang.TemplateExtensions;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.Golang.CodeTarget;

public abstract class GoCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    protected override string FileSuffixName => "go";

    protected override ICodeStyle DefaultCodeStyle => CodeFormatManager.Ins.GoDefaultCodeStyle;

    private static readonly HashSet<string> s_preservedKeyWords = new()
    {
        // go preserved key words 
        //"break", "default", "func", "interface", "select", "case", "defer", "go", "map", "struct", "chan", "else", "goto", "package", "switch", "const", "fallthrough", "if", "range", "continue", "for", "import", "return", "var"
    };

    protected override IReadOnlySet<string> PreservedKeyWords => s_preservedKeyWords;
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new GoCommonTemplateExtension());
        string lubanModuleName = EnvManager.Current.GetOption(Name, "lubanGoModule", true);
        ctx.PushGlobal(new ScriptObject()
        {
            {"__luban_module_name", lubanModuleName},
        });
    }
}
