using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;

namespace Luban.Job.Proto.Defs
{
    class DefField : DefFieldBase
    {
        public DefAssembly Assembly => (DefAssembly)HostType.AssemblyBase;

        public DefField(DefTypeBase host, Field f, int idOffset) : base(host, f, idOffset)
        {

        }
    }
}
