using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.FlatBuffers.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.FlatBuffers.CodeTarget;

[CodeTarget("flatbuffers")]
public class FlatBuffersSchemaTarget : TemplateCodeTargetBase
{
    public override string FileHeader => "";

    protected override string FileSuffixName => "fbs";
    
    protected override ICodeStyle CodeStyle => CodeFormatManager.Ins.NoneCodeStyle;
    
    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        string outputSchemaFileName = EnvManager.Current.GetOptionOrDefault(Name, "outputFile", true, "schema.fbs");
        manifest.AddFile(new OutputFile()
        {
            File = $"{outputSchemaFileName}",
            Content = GenerateSchema(ctx),
        });
    }
    
    protected override Template GetTemplate(string name)
    {
        if (TemplateManager.Ins.TryGetTemplate($"fbs/{name}", out var template))
        {
            return template;
        }

        if (!string.IsNullOrWhiteSpace(CommonTemplateSearchPath) && TemplateManager.Ins.TryGetTemplate($"{CommonTemplateSearchPath}/{name}", out template))
        {
            return template;
        }
        throw new Exception($"template:{name} not found");
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        
    }

    protected virtual string GenerateSchema(GenerationContext ctx)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"schema");
        var tplCtx = CreateTemplateContext(template);
        tplCtx.PushGlobal(new FlatBuffersTemplateExtension());
        OnCreateTemplateContext(tplCtx);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager},
            { "__namespace", ctx.Target.TopModule},
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