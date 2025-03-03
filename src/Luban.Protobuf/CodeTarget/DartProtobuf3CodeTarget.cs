using Luban.CodeTarget;
using Luban.Dart.TemplateExtensions;
using Luban.Protobuf.CodeTarget;
using Luban.Protobuf.TemplateExtensions;
using Scriban;

namespace Luban.Dart.CodeTarget;
[CodeTarget("dart-protobuf3")]
class DartProtobuf3CodeTarget : ProtobufSchemaTargetBase
{
    protected override string CommonTemplateSearchPath => "pb";
    protected override string TemplateDir => "dart";
    protected override string Syntax => "proto3";

    protected override string FileSuffixName => "dart";

    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        base.Handle(ctx, manifest);

        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.ExportTables, writer);
            return CreateOutputFile($"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", writer.ToResult(FileHeader));
        }));
        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }
    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new Protobuf3TemplateExtension());
        ctx.PushGlobal(new DartProtobuf3TemplateExtension());

    }
}
