using Luban.CodeTarget;
using Luban.Lua.TemplateExtensions;
using Luban.Tmpl;
using Scriban;
using Scriban.Runtime;

namespace Luban.Lua.CodeTarget;

abstract class LuaCodeTargetBase : TemplateCodeTargetBase
{
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_LUA;
    protected override string FileSuffixName => "lua";
        
    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        string outputSchemaFileName = EnvManager.Current.GetOptionOrDefault(Name, "outputFile", true, "schema.lua");
        manifest.AddFile(new OutputFile()
        {
            File = $"{outputSchemaFileName}",
            Content = GenerateSchema(ctx),
        });
    }
        
    protected virtual string GenerateSchema(GenerationContext ctx)
    {
        var writer = new CodeWriter();
        var template = GetTemplate($"schema");
        var tplCtx = CreateTemplateContext(template);
        tplCtx.PushGlobal(new LuaCommonTemplateExtension());
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