using Luban.Job.Cfg.DataSources.Excel;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{
    public enum ETableMode
    {
        ONE,
        MAP,
        LIST,
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

        public bool LoadDefineFromFile { get; set; }

        public ETableMode Mode { get; set; }

        public string Options { get; set; }

        public string Comment { get; set; }

        public string Tags { get; set; }

        public List<string> Groups { get; set; } = new List<string>();

        public List<string> InputFiles { get; set; } = new List<string>();

        public string OutputFile { get; set; }

        public Dictionary<string, List<string>> PatchInputFiles { get; set; } = new Dictionary<string, List<string>>();
    }
}
