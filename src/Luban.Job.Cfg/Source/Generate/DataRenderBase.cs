using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    abstract class DataRenderBase : IRender
    {
        public abstract void Render(GenContext ctx);
    }
}
