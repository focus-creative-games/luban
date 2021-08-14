using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    abstract class PythonCodeRenderBase : CodeRenderBase
    {
        [ThreadStatic]
        private static Template t_tsConstRender;
        public override string Render(DefConst c)
        {
            var ctx = new TemplateContext();
            var env = new TTypeTemplateCommonExtends
            {
                ["x"] = c
            };
            ctx.PushGlobal(env);


            var template = t_tsConstRender ??= Template.Parse(@"
{{~if x.comment != '' ~}}
'''
{{x.comment}}
'''
{{~end~}}
class {{x.py_full_name}}:
    {{~ for item in x.items ~}}
{{~if item.comment != '' ~}}
    '''
    {{item.comment}}
    '''
{{~end~}}
    {{item.name}} = {{py_const_value item.ctype item.value}}
    {{~end~}}
    {{~if (x.items == empty)~}}
    pass
    {{~end~}}

");
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_tsEnumRender;
        public override string Render(DefEnum e)
        {
            var template = t_tsEnumRender ??= Template.Parse(@"
{{~if comment != '' ~}}
'''
{{comment}}
'''
{{~end~}}
class {{py_full_name}}(Enum):
    {{~ for item in items ~}}
{{~if item.comment != '' ~}}
    '''
    {{item.comment}}
    '''
{{~end~}}
    {{item.name}} = {{item.value}}
    {{~end~}}
    {{~if (items == empty)~}}
    pass
    {{~end~}}
");
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(@"
{{
    name = x.name
    namespace = x.namespace
    tables = x.tables
}}

class {{name}}:
    {{~ for table in tables ~}}
    #def {{table.name}} : return self._{{table.name}}
    {{~end~}}

    def __init__(self, loader):
        {{~for table in tables ~}}
        self.{{table.name}} = {{table.py_full_name}}(loader('{{table.output_data_file}}')); 
        {{~end~}}

");
            var result = template.RenderCode(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
