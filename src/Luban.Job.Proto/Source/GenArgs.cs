using CommandLine;
using Luban.Job.Common;

namespace Luban.Job.Proto
{
    public class GenArgs : GenArgsBase
    {
        [Option('g', "gen_type", Required = true, HelpText = "cs,lua,java,typescript")]
        public string GenType { get; set; }

        [Option('s', "service", Required = true, HelpText = "service")]
        public string Service { get; set; }
    }
}
