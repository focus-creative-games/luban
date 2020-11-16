using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;

namespace Luban.Job.Proto.Defs
{
    class DefBean : DefBeanBase
    {
        public DefBean(Bean b) : base(b)
        {

        }

        protected override DefFieldBase CreateField(Field f, int idOffset)
        {
            return new DefField(this, f, idOffset);
        }
    }
}
