using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ValidatorAttribute : Attribute
    {
        public string Name { get; }

        public ValidatorAttribute(string name)
        {
            Name = name;
        }
    }
}
