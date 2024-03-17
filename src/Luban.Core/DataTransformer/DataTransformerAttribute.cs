using Luban.CustomBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.DataTransformer;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class DataTransformerAttribute : BehaviourBaseAttribute
{
    public DataTransformerAttribute(string name) : base(name)
    {
    }
}
