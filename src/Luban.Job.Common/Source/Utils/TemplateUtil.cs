using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Utils
{
    public static class TemplateUtil
    {
        public static TemplateContext CreateDefaultTemplateContext()
        {
            return new TemplateContext()
            {
                LoopLimit = 0,
                NewLine = "\n",
            };
        }
    }
}
