using Luban.Job.Common.Defs;
using System.Collections.Generic;

namespace Luban.Job.Common.Generate
{
    public interface ICodeRender<T> where T : DefTypeBase
    {
        string RenderAny(DefTypeBase o);

        string Render(DefEnum c);

        string RenderService(string name, string module, List<T> tables);
    }
}
