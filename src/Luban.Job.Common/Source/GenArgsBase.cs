using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common
{
    public class GenArgsBase
    {
        [Option('d', "define_file", Required = true, HelpText = "define file")]
        public string DefineFile { get; set; }

        [Option('c', "output_code_dir", Required = false, HelpText = "output code directory")]
        public string OutputCodeDir { get; set; }

        [Option("typescript_bright_require_path", Required = false, HelpText = "bright require path in typescript")]
        public string TypescriptBrightRequirePath { get; set; }

        [Option("use_puerts_bytebuf", Required = false, HelpText = "use puerts bytebuf class")]
        public bool UsePuertsByteBuf { get; set; }

        [Option("embed_bright_types", Required = false, HelpText = "use puerts bytebuf class")]
        public bool EmbedBrightTypes { get; set; }



        public bool ValidateOutouptCodeDir(ref string errMsg)
        {
            if (string.IsNullOrWhiteSpace(this.OutputCodeDir))
            {
                errMsg = "--outputcodedir missing";
                return false;
            }
            return true;
        }

        public bool ValidateTypescriptRequire(string genType, ref string errMsg)
        {
            if (genType.Contains("typescript"))
            {
                if (!this.UsePuertsByteBuf && string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath))
                {
                    errMsg = $"while use_puerts_bytebuf is false, should provide option --typescript_bright_require_path";
                    return false;
                }
                if (!this.EmbedBrightTypes && string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath))
                {
                    errMsg = $"while embed_bright_types is false, should provide option --typescript_bright_require_path";
                    return false;
                }
            }
            return true;
        }
    }
}
