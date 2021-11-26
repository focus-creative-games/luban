using Luban.Job.Common.Generate;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Luban.Job.Proto.Generate
{
    static class RenderFactory
    {
        static RenderFactory()
        {
            Dictionary<string, IRender> renders = new();
            foreach (var type in typeof(JobController).Assembly.DefinedTypes.Where(t => t.AsType().GetCustomAttributes(typeof(RenderAttribute)).Any()))
            {
                foreach (var attr in type.GetCustomAttributes<RenderAttribute>())
                {
                    renders.Add(attr.Name, (IRender)System.Activator.CreateInstance(type));
                }
            }

            s_renders = renders;
        }

        private static readonly Dictionary<string, IRender> s_renders;

        public static IRender CreateRender(string genType)
        {
            return s_renders.TryGetValue(genType, out var render) ? render : null;
        }
    }
}
