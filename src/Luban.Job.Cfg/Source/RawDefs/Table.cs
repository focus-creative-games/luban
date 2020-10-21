using System.Collections.Generic;

namespace Luban.Config.Common.RawDefs
{
    public enum ETableMode
    {
        ONE,
        MAP,
        BMAP, //双主键 map
    }

    public class CfgInputFile
    {
        public string OriginFile { get; set; }

        public string ActualFile { get; set; }

        public string MD5 { get; set; }

        public bool MultiRecord { get; set; }
    }

    public class Table
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Index { get; set; }

        public string ValueType { get; set; }

        public ETableMode Mode { get; set; }

        public List<string> Groups { get; set; } = new List<string>();

        public List<string> InputFiles { get; set; } = new List<string>();
    }
}
