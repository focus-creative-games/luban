using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Generate;

namespace Luban.Job.Cfg.Generate
{
    interface ICfgCodeRender : ICodeRender<DefTable>, IRender
    {
        string Render(DefBean b);

        string Render(DefTable c);
    }
}
