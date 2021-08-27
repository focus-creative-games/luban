using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class RenderAttribute : System.Attribute
    {
        public string Name { get; }

        public RenderAttribute(string name)
        {
            Name = name;
        }
    }
}
