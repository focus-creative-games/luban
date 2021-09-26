using Luban.Job.Common.RawDefs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.RawDefs
{

    public class Validator
    {
        public string Type { get; set; }

        public string Rule { get; set; }
    }

    public class CfgField : Field
    {
        public string Index { get; set; } = "";

        public string Sep { get; set; } = "";

        public bool IsMultiRow { get; set; }

        public string Resource { get; set; } = "";

        public string Converter { get; set; } = "";

        public string DefaultValue { get; set; } = "";

        public bool IsRowOrient { get; set; } = true;

        public List<string> Groups { get; set; } = new List<string>();

        public List<Validator> KeyValidators { get; } = new List<Validator>();

        public List<Validator> ValueValidators { get; } = new List<Validator>();

        public List<Validator> Validators { get; } = new List<Validator>();
    }
}
