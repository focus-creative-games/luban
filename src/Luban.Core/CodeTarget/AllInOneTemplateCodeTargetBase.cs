using Luban.CodeFormat;
using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Tmpl;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.CodeTarget;

public abstract class AllInOneTemplateCodeTargetBase : TemplateCodeTargetBase
{
    protected abstract string DefaultOutputFileName { get; }

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        string outputSchemaFileName = EnvManager.Current.GetOptionOrDefault(Name, $"outputFile", true, DefaultOutputFileName);
        manifest.AddFile(CreateOutputFile($"{outputSchemaFileName}", GenerateSchema(ctx)));
    }

    protected virtual string GenerateSchema(GenerationContext ctx)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"schema");
        var tplCtx = CreateTemplateContext(template);
        OnCreateTemplateContext(tplCtx);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager},
            { "__namespace", ctx.Target.TopModule},
            { "__full_name", TypeUtil.MakeFullName(ctx.Target.TopModule, ctx.Target.Manager)},
            { "__tables", ctx.ExportTables},
            { "__beans", ctx.ExportBeans},
            { "__enums", ctx.ExportEnums},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
        return writer.ToResult(FileHeader);
    }
}
