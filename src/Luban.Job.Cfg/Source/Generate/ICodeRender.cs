using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    interface ICodeRender
    {
        string RenderAny(object o);

        string Render(DefConst c);

        string Render(DefEnum c);

        string Render(DefBean b);

        string Render(DefTable c);

        string RenderService(string name, string module, List<DefTable> tables);
    }
}
