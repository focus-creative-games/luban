using System;

namespace Luban.Server.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ControllerAttribute : System.Attribute
    {
        public string Name { get; }

        public ControllerAttribute(string name)
        {
            Name = name;
        }
    }
}
