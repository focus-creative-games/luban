using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.TypeVisitors
{
    public class CsEditorNeedInitVisitor : CsNeedInitVisitor
    {
        public static new CsEditorNeedInitVisitor Ins { get; } = new CsEditorNeedInitVisitor();

        public override bool Accept(TEnum type)
        {
            return true;
        }

        public override bool Accept(TDateTime type)
        {
            return true;
        }
    }
}
