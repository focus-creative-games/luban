using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;

namespace Luban.Job.Db.Defs
{
    class DefField : DefFieldBase
    {
        public DefAssembly Assembly => (DefAssembly)HostType.AssemblyBase;

        public DefField(DbDefTypeBase host, Field f, int idOffset) : base(host, f, idOffset)
        {

        }
    }
}
