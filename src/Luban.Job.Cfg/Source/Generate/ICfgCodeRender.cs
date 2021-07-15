using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    interface ICfgCodeRender : ICodeRender<DefTable>
    {
        string Render(DefBean b);

        string Render(DefTable c);
    }
}
