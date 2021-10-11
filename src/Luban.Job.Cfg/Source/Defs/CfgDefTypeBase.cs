using Luban.Common.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;

namespace Luban.Job.Cfg.Defs
{
    public abstract class CfgDefTypeBase : DefTypeBase
    {
        public DefAssembly Assembly => (DefAssembly)AssemblyBase;
    }
}
