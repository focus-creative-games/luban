namespace Luban.Any.Generate;

abstract class LuaCodeRenderBase : TemplateCodeRenderBase
{
    public override void Render(GenerationContext ctx)
    {
        DefAssembly.LocalAssebmly.CurrentLanguage = Common.ELanguage.LUA;
        var file = RenderFileUtil.GetFileOrDefault(ctx.GenArgs.OutputCodeMonolithicFile, "Types.lua");
        var content = this.RenderAll(ctx.ExportTypes);
        var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join('\n', content));
        ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
    }
}