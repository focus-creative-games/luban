using CommandLine;

namespace Luban.Job.Common
{
    public class GenArgsBase
    {
        [Option('d', "define_file", Required = true, HelpText = "define file")]
        public string DefineFile { get; set; }

        [Option('c', "output_code_dir", Required = false, HelpText = "output code directory")]
        public string OutputCodeDir { get; set; }

        [Option("output_code_monolithic_file", Required = false, HelpText = "output monolithic code file. only effect when lan=rust,python,typescript,lua")]
        public string OutputCodeMonolithicFile { get; set; }

        [Option("typescript_bright_require_path", Required = false, HelpText = "bright require path in typescript")]
        public string TypescriptBrightRequirePath { get; set; }

        [Option("typescript_bright_package_name", Required = false, HelpText = "typescript bright package name")]
        public string TypescriptBrightPackageName { get; set; }

        [Option("use_puerts_bytebuf", Required = false, HelpText = "use puerts bytebuf class")]
        public bool UsePuertsByteBuf { get; set; }

        [Option("embed_bright_types", Required = false, HelpText = "use puerts bytebuf class")]
        public bool EmbedBrightTypes { get; set; }

        [Option("use_unity_vector", Required = false, HelpText = "use UnityEngine.Vector{2,3,4}")]
        public bool UseUnityVectors { get; set; }

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
            if (!string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath) && !string.IsNullOrWhiteSpace(this.TypescriptBrightPackageName))
            {
                errMsg = "can't use options --typescript_bright_require_path and --typescript_bright_package_name at the same time";
                return false;
            }
            bool hasBrightPathOrPacakge = !string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath) || !string.IsNullOrWhiteSpace(this.TypescriptBrightPackageName);
            if (!this.UsePuertsByteBuf && !hasBrightPathOrPacakge)
            {
                errMsg = "while --use_puerts_bytebuf is false, should provide option --typescript_bright_require_path or --typescript_bright_package_name";
                return false;
            }
            if (!this.EmbedBrightTypes && !hasBrightPathOrPacakge)
            {
                errMsg = "while --embed_bright_types is false, should provide option --typescript_bright_require_path or --typescript_bright_package_name";
                return false;
            }
            return true;
        }
    }
}
