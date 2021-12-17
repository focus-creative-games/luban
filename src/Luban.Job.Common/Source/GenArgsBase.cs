using CommandLine;

namespace Luban.Job.Common
{
    public class GenArgsBase
    {
        [Option('d', "define_file", Required = true, HelpText = "define file")]
        public string DefineFile { get; set; }

        [Option('c', "output_code_dir", Required = false, HelpText = "output code directory")]
        public string OutputCodeDir { get; set; }

        [Option("output:code:monolithic_file", Required = false, HelpText = "output monolithic code file. only effect when lan=rust,python,typescript,lua,protobuf,flatbuffers")]
        public string OutputCodeMonolithicFile { get; set; }

        [Option("naming_convention:module", Required = false, HelpText = "naming convention of module. can be language_recommend,none,camelCase,PascalCase,under_scores")]
        public string NamingConventionModuleStr { get; set; }

        public NamingConvention NamingConventionModule { get; set; }

        [Option("naming_convention:type", Required = false, HelpText = "naming convention of enum and bean. can be language_recommend,none,camelCase,PascalCase,under_scores")]
        public string NamingConventionTypeStr { get; set; }

        public NamingConvention NamingConventionType { get; set; }

        [Option("naming_convention:bean_member", Required = false, HelpText = "naming convention of bean member. can be language_recommend,none,camelCase,PascalCase,under_scores")]
        public string NamingConventionBeanMemberStr { get; set; }

        public NamingConvention NamingConventionBeanMember { get; set; }

        [Option("naming_convention:enum_member", Required = false, HelpText = "naming convention of enum member. can be language_recommend,none,camelCase,PascalCase,under_scores")]
        public string NamingConventionEnumMemberStr { get; set; }

        public NamingConvention NamingConventionEnumMember { get; set; }

        [Option("access_bean_member", Required = false, HelpText = "mode of bean field. can be  language_recommend,variable,getter_setter,property")]
        public string AccessConventionBeanMemberStr { get; set; }

        // luanguage options

        [Option("typescript:bright_require_path", Required = false, HelpText = "bright require path in typescript")]
        public string TypescriptBrightRequirePath { get; set; }

        [Option("typescript:bright_package_name", Required = false, HelpText = "typescript bright package name")]
        public string TypescriptBrightPackageName { get; set; }

        [Option("typescript:use_puerts_bytebuf", Required = false, HelpText = "use puerts bytebuf class")]
        public bool TypescriptUsePuertsByteBuf { get; set; }

        [Option("typescript:embed_bright_types", Required = false, HelpText = "use puerts bytebuf class")]
        public bool TypescriptEmbedBrightTypes { get; set; }

        [Option("cs:use_unity_vector", Required = false, HelpText = "use UnityEngine.Vector{2,3,4}")]
        public bool CsUseUnityVectors { get; set; }

        [Option("go:bright_module_name", Required = false, HelpText = "go bright module name")]
        public string GoBrightModuleName { get; set; }

        [Option("external:selectors", Required = false, HelpText = "external selectors")]
        public string ExternalSelectors { get; set; }

        public AccessConvention AccessConventionBeanMember { get; set; }

        public bool ValidateOutouptCodeDir(ref string errMsg)
        {
            if (string.IsNullOrWhiteSpace(this.OutputCodeDir))
            {
                errMsg = "--outputcodedir missing";
                return false;
            }
            return true;
        }

        public static bool TryParseNamingConvention(string nc, out NamingConvention result)
        {
            switch (nc)
            {
                case null:
                case "":
                case "language_recommend": result = NamingConvention.LanguangeRecommend; return true;
                case "none": result = NamingConvention.None; return true;
                case "camelCase": result = NamingConvention.CameraCase; return true;
                case "PascalCase": result = NamingConvention.PascalCase; return true;
                case "under_scores": result = NamingConvention.UnderScores; return true;
                default: result = NamingConvention.Invalid; return false;
            }
        }

        public static bool TryParseAccessConvention(string fm, out AccessConvention ac)
        {
            //return string.IsNullOrEmpty(fm) || fm == "language_recommend" || fm == "variable" || fm == "getter_setter" || fm == "property";
            switch (fm)
            {
                case null:
                case "":
                case "language_recommnd": ac = AccessConvention.LanguangeRecommend; return true;
                case "variable": ac = AccessConvention.Variable; return true;
                case "getter_setter": ac = AccessConvention.GetterSetter; return true;
                case "property": ac = AccessConvention.Property; return true;
                default: ac = AccessConvention.Invalid; return false;
            }
        }

        public bool ValidateTypescriptRequire(string genType, ref string errMsg)
        {
            if (!string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath) && !string.IsNullOrWhiteSpace(this.TypescriptBrightPackageName))
            {
                errMsg = "can't use options --typescript_bright_require_path and --typescript_bright_package_name at the same time";
                return false;
            }
            bool hasBrightPathOrPacakge = !string.IsNullOrWhiteSpace(this.TypescriptBrightRequirePath) || !string.IsNullOrWhiteSpace(this.TypescriptBrightPackageName);
            if (!this.TypescriptUsePuertsByteBuf && !hasBrightPathOrPacakge)
            {
                errMsg = "while --use_puerts_bytebuf is false, should provide option --typescript_bright_require_path or --typescript_bright_package_name";
                return false;
            }
            if (!this.TypescriptEmbedBrightTypes && !hasBrightPathOrPacakge)
            {
                errMsg = "while --embed_bright_types is false, should provide option --typescript_bright_require_path or --typescript_bright_package_name";
                return false;
            }
            return true;
        }

        public bool ValidateGoRequire(string genType, ref string errMsg)
        {
            if (string.IsNullOrWhiteSpace(this.GoBrightModuleName))
            {
                errMsg = "option '--go:bright_module_name <module name> ' missing";
                return false;
            }
            return true;
        }

        public bool ValidateConvention(ref string errMsg)
        {
            if (!TryParseNamingConvention(NamingConventionModuleStr, out var m))
            {
                errMsg = "--naming_convention_module invalid! valid values: language_recommend,none,camelCase,PascalCase,under_scores";
                return false;
            }
            NamingConventionModule = m;
            if (!TryParseNamingConvention(NamingConventionTypeStr, out var t))
            {
                errMsg = "--naming_convention_type invalid! valid values: language_recommend,none,camelCase,PascalCase,under_scores";
                return false;
            }
            NamingConventionType = t;
            if (!TryParseNamingConvention(NamingConventionBeanMemberStr, out var bm))
            {
                errMsg = "--naming_convention_bean_member invalid! valid values: language_recommend,none,camelCase,PascalCase,under_scores";
                return false;
            }
            NamingConventionBeanMember = bm;
            if (!TryParseNamingConvention(NamingConventionEnumMemberStr, out var em))
            {
                errMsg = "--naming_convention_enum_member invalid! valid values: language_recommend,none,camelCase,PascalCase,under_scores";
                return false;
            }
            NamingConventionEnumMember = em;
            if (!TryParseAccessConvention(AccessConventionBeanMemberStr, out var acbm))
            {
                errMsg = "--access_bean_member invalid! valid values: language_recommend,variable,getter_setter,property";
                return false;
            }
            AccessConventionBeanMember = acbm;
            return true;
        }
    }
}
