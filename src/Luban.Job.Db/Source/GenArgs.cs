using CommandLine;
using Luban.Job.Common;

namespace Luban.Job.Db
{
    class GenArgs : GenArgsBase
    {
        [Option('g', "gen_type", Required = true, HelpText = "cs,typescript ")]
        public string GenType { get; set; }
    }
}
