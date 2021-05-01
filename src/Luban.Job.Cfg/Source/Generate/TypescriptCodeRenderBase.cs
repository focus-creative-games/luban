using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class TypescriptCodeRenderBase : CodeRenderBase
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
namespace {{x.namespace}} {
export class {{x.name}} {
    {{~ for item in x.items ~}}
    static {{item.name}} = {{ts_const_value item.ctype item.value}};
    {{~end~}}
}
}

");
            var result = template.Render(ctx);

            return result;
        }

        [ThreadStatic]
        private static Template t_tsEnumRender;
        public override string Render(DefEnum e)
        {
            var template = t_tsEnumRender ??= Template.Parse(@"
namespace {{namespace}} {
export enum {{name}} {
    {{- for item in items }}
    {{item.name}} = {{item.value}},
    {{-end}}
}
}

");
            var result = template.Render(e);

            return result;
        }
    }
}
