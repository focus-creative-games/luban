using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Proto.RawDefs
{

    public class PProto
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public string Comment { get; set; }

        public List<Field> Fields { get; set; } = new List<Field>();
    }
}
