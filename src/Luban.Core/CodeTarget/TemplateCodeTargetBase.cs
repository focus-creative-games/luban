using Luban.CodeFormat;
using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Tmpl;
using Luban.Utils;
using Scriban;
using Scriban.Runtime;

namespace Luban.CodeTarget;

public abstract class TemplateCodeTargetBase : CodeTargetBase
{
    protected virtual string CommonTemplateSearchPath => $"common/{FileSuffixName}";

    protected virtual string TemplateDir => Name;

    protected TemplateContext CreateTemplateContext(Template template)
    {
        var ctx = new TemplateContext()
        {
            LoopLimit = 0,
            NewLine = "\n",
        };
        ctx.PushGlobal(new ContextTemplateExtension());
        ctx.PushGlobal(new TypeTemplateExtension());
        OnCreateTemplateContext(ctx);
        return ctx;
    }

    protected abstract void OnCreateTemplateContext(TemplateContext ctx);

    protected virtual Scriban.Template GetTemplate(string name)
    {
        if (TemplateManager.Ins.TryGetTemplate($"{TemplateDir}/{name}", out var template))
        {
            return template;
        }

        if (!string.IsNullOrWhiteSpace(CommonTemplateSearchPath) && TemplateManager.Ins.TryGetTemplate($"{CommonTemplateSearchPath}/{name}", out template))
        {
            return template;
        }
        throw new Exception($"template:{name} not found");
    }

    public override void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer)
    {
        var template = GetTemplate("tables");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", ctx.Target.Manager },
            { "__namespace", ctx.Target.TopModule },
            { "__tables", tables },
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    public override void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer)
    {
        var template = GetTemplate("table");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__manager_name", ctx.Target.Manager },
            { "__manager_name_with_top_module", TypeUtil.MakeFullName(ctx.TopModule, ctx.Target.Manager) },
            { "__name", table.Name },
            { "__namespace", table.Namespace },
            { "__namespace_with_top_module", table.NamespaceWithTopModule },
            { "__full_name_with_top_module", table.FullNameWithTopModule },
            { "__table", table },
            { "__this", table },
            { "__key_type", table.KeyTType},
            { "__value_type", table.ValueTType},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    public override void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
        var template = GetTemplate("bean");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__manager_name", ctx.Target.Manager },
            { "__manager_name_with_top_module", TypeUtil.MakeFullName(ctx.TopModule, ctx.Target.Manager) },
            { "__name", bean.Name },
            { "__namespace", bean.Namespace },
            { "__namespace_with_top_module", bean.NamespaceWithTopModule },
            { "__full_name_with_top_module", bean.FullNameWithTopModule },
            { "__bean", bean },
            { "__this", bean },
            {"__export_fields", bean.ExportFields},
            {"__hierarchy_export_fields", bean.HierarchyExportFields},
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
            { "__top_module", ctx.Target.TopModule },
            { "__namespace_with_top_module", @enum.NamespaceWithTopModule },
            { "__full_name_with_top_module", @enum.FullNameWithTopModule },
            { "__enum", @enum },
            { "__this", @enum },
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }
}
