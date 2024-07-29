using Luban.CustomBehaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Schema;

[AttributeUsage(AttributeTargets.Class)]
public class TableImporterAttribute : BehaviourBaseAttribute
{
    public TableImporterAttribute(string name) : base(name)
    {
    }
}
