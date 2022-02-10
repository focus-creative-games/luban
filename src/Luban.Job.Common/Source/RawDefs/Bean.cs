using System.Collections.Generic;

namespace Luban.Job.Common.RawDefs
{
    public class Bean
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string FullName => Namespace.Length > 0 ? Namespace + "." + Name : Name;

        public string Parent { get; set; }

        public bool IsValueType { get; set; }

        public int TypeId { get; set; }

        public bool IsSerializeCompatible { get; set; }

        public string Comment { get; set; }

        public string Tags { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();
    }
}
