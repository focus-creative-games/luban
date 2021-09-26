using System.Collections.Generic;

namespace Luban.Job.Common.RawDefs
{
    public class ConstItem
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public string Comment { get; set; }
    }

    public class Const
    {

        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public List<ConstItem> Items { get; set; } = new List<ConstItem>();
    }
}
