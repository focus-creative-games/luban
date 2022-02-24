using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.TypeVisitors
{
    public class CsEditorUnderlyingDefineTypeName : CsUnderingDefineTypeName
    {
        public static new CsEditorUnderlyingDefineTypeName Ins { get; } = new CsEditorUnderlyingDefineTypeName();

        public override string Accept(TText type)
        {
            return CfgConstStrings.EditorTextTypeName;
        }

        public override string Accept(TDateTime type)
        {
            return "string";
        }
    }
}
