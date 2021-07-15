using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class GoCodeRenderBase : CodeRenderBase
    {
        [ThreadStatic]
        private static Template t_constRender;

        public override string Render(DefConst c)
        {
            string package = "cfg";

            var template = t_constRender ??= Template.Parse(@"

package {{package}}

const (
    {{~for item in x.items ~}}
    {{x.go_full_name}}_{{item.name}} = {{go_const_value item.ctype item.value}}
    {{~end~}}
)
");
            var result = template.RenderCode(c, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_enumRender;

        public override string Render(DefEnum e)
        {
            string package = "cfg";

            var template = t_enumRender ??= Template.Parse(@"

package {{package}}

const (
    {{~for item in x.items ~}}
    {{x.go_full_name}}_{{item.name}} = {{item.value}}
    {{~end~}}
)

");
            var result = template.RenderCode(e, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

    }
}
