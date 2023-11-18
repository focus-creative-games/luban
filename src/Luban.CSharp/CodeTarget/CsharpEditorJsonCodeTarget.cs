using Luban.CodeTarget;
using Luban.CSharp.TemplateExtensions;
using Luban.Defs;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("cs-editor-json")]
public class CsharpEditorJsonCodeTarget : CsharpCodeTargetBase
{
    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();

        foreach (var bean in ctx.Assembly.TypeList.OfType<DefBean>())
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(bean.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var @enum in ctx.Assembly.TypeList.OfType<DefEnum>())
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(@enum.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    private string TopModule => EnvManager.Current.GetOptionOrDefault("editor", "topModule", true, "editor.cfg");

    private string MakeNameWithTopModule(string name)
    {
        return TypeUtil.MakeFullName(TopModule, name);
    }

    public override void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
        var template = GetTemplate("bean");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", TopModule },
            { "__name", bean.Name },
            { "__namespace", bean.Namespace },
            { "__namespace_with_top_module", MakeNameWithTopModule(bean.Namespace) },
            { "__full_name_with_top_module", MakeNameWithTopModule(bean.FullName) },
            { "__bean", bean },
            { "__this", bean },
            {"__fields", bean.Fields},
            {"__hierarchy_fields", bean.HierarchyFields},
            {"__parent_def_type", bean.ParentDefType},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    public override void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer)
    {
        var template = GetTemplate("enum");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", @enum.Name },
            { "__namespace", @enum.Namespace },
            { "__top_module", TopModule },
            { "__namespace_with_top_module", MakeNameWithTopModule(@enum.Namespace) },
            { "__full_name_with_top_module", MakeNameWithTopModule(@enum.FullName) },
            { "__enum", @enum },
            { "__this", @enum },
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpEditorTemplateExtension());
    }
}
