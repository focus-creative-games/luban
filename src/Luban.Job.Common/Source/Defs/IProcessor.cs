using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Defs
{
    public interface IProcessor
    {
        void Compile(DefFieldBase def);
    }
}
