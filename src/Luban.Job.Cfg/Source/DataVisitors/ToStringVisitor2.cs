using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System.Collections.Generic;
using System.Text;

namespace Luban.Job.Cfg.DataVisitors
{
    class ToStringVisitor2 : ToStringVisitor
    {
        public static new ToStringVisitor2 Ins { get; } = new();

        public override string Accept(DEnum type)
        {
            return type.Value.ToString();
        }

        public override string Accept(DString type)
        {
            return DataUtil.EscapeString(type.Value);
        }
    }
}
