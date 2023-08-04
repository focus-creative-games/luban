namespace Luban.Any.Generate;

[Render("code_cpp_bin")]
class CppCodeBinRender : TemplateCodeRenderBase
{
    protected override string RenderTemplateDir => "cpp_bin";

    public override void Render(GenerationContext ctx)
    {
        // ������ ͷ�ļ����� ���ɵ�һ���ļ�
        // ���� const,enum,bean,table, service ��˳������
        DefAssembly.LocalAssebmly.CurrentLanguage = ELanguage.CPP;
        ctx.Tasks.Add(Task.Run(() =>
        {
            var enums = new List<DefEnum>();
            var enumCodes = new List<string>();

            var beans = new List<DefBean>();
            var beanCodes = new List<string>();

            var tables = ctx.ExportTables;
            var tableCodes = new List<string>();

            foreach (var type in ctx.ExportTypes)
            {
                switch(type)
                {
                    case DefEnum e:
                    {
                        enums.Add(e);
                        enumCodes.Add(Render(e));
                        break;
                    }
                    case DefBean b:
                    {
                        beans.Add(b);
                        beanCodes.Add(Render(b));
                        break;
                    }
                }
            }

            foreach (var type in ctx.ExportTables)
            {
                tableCodes.Add(Render(type));
            }

            string tablesCode = RenderService(ctx.Assembly.TableManagerName, ctx.TopModule, ctx.ExportTables);

            var rawContent = GetConfigTemplate("all_types").RenderCode(new 
            {
                Enums = enums,
                Beans = beans,
                EnumCodes = enumCodes,
                BeanCodes = beanCodes,
                TableCodes = tableCodes,
                TablesCode = tablesCode,
            });
            var content = FileHeaderUtil.ConcatAutoGenerationHeader(rawContent, ELanguage.CPP);
            var file = ctx.Assembly.GetOptionOr($"{RenderTemplateDir}.output_all_types_file", "gen_types.h");
            var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
            ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
        }));

        var beanTypes = ctx.ExportTypes.Where(c => c is DefBean).ToList();

        int TYPE_PER_STUB_FILE = int.Parse(ctx.Assembly.GetOptionOr($"{RenderTemplateDir}.type_per_stub_file", "100"));

        string stubFileFormat = ctx.Assembly.GetOptionOr($"{RenderTemplateDir}.stub_file_name_format", "gen_stub_{0}.cpp");
        var template = GetConfigTemplate("stub");
        for (int i = 0, n = (beanTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
        {
            int index = i;
            ctx.Tasks.Add(Task.Run(() =>
            {
                int startIndex = index * TYPE_PER_STUB_FILE;
                var rawContent = template.RenderCode(new { Types = beanTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, beanTypes.Count - startIndex)), });
                var content = FileHeaderUtil.ConcatAutoGenerationHeader(rawContent, ELanguage.CPP);
                var file = string.Format(stubFileFormat, index);
                var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
            }));
        }
    }

}