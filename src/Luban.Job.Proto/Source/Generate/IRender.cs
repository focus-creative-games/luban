using Luban.Job.Proto.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    interface IRender
    {
        void Render(GenContext ctx);

        string RenderAny(object o);

        string RenderStubs(string name, string module, List<DefProto> protos, List<DefRpc> rpcs);
    }
}
