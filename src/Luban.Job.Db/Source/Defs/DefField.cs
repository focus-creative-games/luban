using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;

namespace Luban.Job.Db.Defs
{
    class DefField : DefFieldBase
    {
        public DefAssembly Assembly => (DefAssembly)HostType.AssemblyBase;

        public string LogType => $"Log_{Name}";

        public string InternalName => "__" + Name;

        public string InternalNameWithThis => "this." + "__" + Name;

        public DefField(DefTypeBase host, Field f, int idOffset) : base(host, f, idOffset)
        {

        }
    }
}
