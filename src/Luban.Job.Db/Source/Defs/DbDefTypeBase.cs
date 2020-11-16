using Luban.Job.Common.Defs;

namespace Luban.Job.Db.Defs
{
    abstract class DbDefTypeBase : DefTypeBase
    {
        public DefAssembly Assembly => (DefAssembly)AssemblyBase;
    }
}
