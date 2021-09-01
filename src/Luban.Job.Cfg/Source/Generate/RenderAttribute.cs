using System;

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
