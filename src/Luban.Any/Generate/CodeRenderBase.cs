namespace Luban.Any.Generate;

abstract class CodeRenderBase : ICfgCodeRender
{
    public abstract void Render(GenerationContext ctx);


    public abstract string Render(DefEnum c);
    public abstract string Render(DefBean b);
    public abstract string Render(DefTable c);
    public abstract string RenderService(string name, string module, List<DefTable> tables);

    public string RenderAny(DefTypeBase o)
    {
        switch (o)
        {
            case DefEnum e: return Render(e);
            case DefBean b: return Render(b);
            case DefTable r: return Render(r);
            default: throw new Exception($"unknown render type:'{o}'");
        }
    }

    protected virtual ELanguage GetLanguage(GenerationContext ctx)
    {
        return RenderFileUtil.GetLanguage(ctx.GenType);
    }

    protected void GenerateCodeScatter(GenerationContext ctx)
    {
        string genType = ctx.GenType;
        ctx.Render = this;
        ctx.Language = GetLanguage(ctx);
        DefAssembly.LocalAssebmly.CurrentLanguage = ctx.Language;
        foreach (var c in ctx.ExportTypes)
        {
            ctx.Tasks.Add(Task.Run(() =>
            {
                string body = ctx.Render.RenderAny(c);
                if (string.IsNullOrWhiteSpace(body))
                {
                    return;
                }
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(body, ctx.Language);
                var file = RenderFileUtil.GetDefTypePath(c.FullName, ctx.Language);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }

        ctx.Tasks.Add(Task.Run(() =>
        {
            var module = ctx.TopModule;
            var name = ctx.TargetRawTarget.Manager;
            var body = ctx.Render.RenderService(name, module, ctx.ExportTables);
            if (string.IsNullOrWhiteSpace(body))
            {
                return;
            }
            var content = FileHeaderUtil.ConcatAutoGenerationHeader(body, ctx.Language);
            var file = RenderFileUtil.GetDefTypePath(name, ctx.Language);
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content);
            ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }));
    }

    protected void GenerateCodeMonolithic(GenerationContext ctx, string outputFile, List<string> fileContent, Action<List<string>> preContent, Action<List<string>> postContent)
    {
        ctx.Tasks.Add(Task.Run(() =>
        {
            fileContent.Add(FileHeaderUtil.GetAutoGenerationHeader(ctx.Language));

            preContent?.Invoke(fileContent);

            foreach (var type in ctx.ExportTypes)
            {
                fileContent.Add(ctx.Render.RenderAny(type));
            }

            fileContent.Add(ctx.Render.RenderService(ctx.Assembly.TableManagerName, ctx.TopModule, ctx.ExportTables));
            postContent?.Invoke(fileContent);

            var file = outputFile;
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, string.Join("\n", fileContent));
            ctx.GenScatteredFiles.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }));
    }

}