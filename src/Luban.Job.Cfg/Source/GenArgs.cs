using CommandLine;
using Luban.Job.Common;

namespace Luban.Job.Cfg
{
    public class GenArgs : GenArgsBase
    {
        [Option("input_data_dir", Required = true, HelpText = "input data dir")]
        public string InputDataDir { get; set; }

        [Option('v', "validate_root_dir", Required = false, HelpText = "validate root directory")]
        public string ValidateRootDir { get; set; }

        [Option("output_data_dir", Required = true, HelpText = "output data directory")]
        public string OutputDataDir { get; set; }

        [Option("output_data_resource_list_file", Required = false, HelpText = "output resource list file")]
        public string OutputDataResourceListFile { get; set; }

        [Option("output_data_json_monolithic_file", Required = false, HelpText = "output monolithic json file")]
        public string OutputDataJsonMonolithicFile { get; set; }

        [Option("gen_types", Required = true, HelpText = "code_cs_bin,code_cs_json,code_cs_unity_json,code_lua_bin,code_java_bin,code_java_json,code_go_bin,code_go_json,code_cpp_bin,code_python3_json,code_typescript_bin,code_typescript_json,code_rust_json,data_bin,data_lua,data_json,data_json2,data_json_monolithic,data_resources,data_template . can be multi")]
        public string GenType { get; set; }

        [Option("template_name", Required = false, HelpText = "template name. use with gen_types=data_template")]
        public string TemplateName { get; set; }

        [Option("data_file_extension", Required = false, HelpText = "data file name extension. default choose by gen_type")]
        public string DataFileExtension { get; set; }

        [Option('s', "service", Required = true, HelpText = "service")]
        public string Service { get; set; }

        [Option("export_test_data", Required = false, HelpText = "export test data")]
        public bool ExportTestData { get; set; } = false;

        [Option('t', "l10n_timezone", Required = false, HelpText = "timezone")]
        public string TimeZone { get; set; }

        [Option("input_l10n_text_files", Required = false, HelpText = "input l10n text table files. can be multi, sep by ','")]
        public string InputTextTableFiles { get; set; }

        [Option("l10n_text_field_name", Required = false, HelpText = "text value field name of text table files. default is text")]
        public string TextValueFieldName { get; set; }

        [Option("output_l10n_not_translated_text_file", Required = false, HelpText = "the file save not translated l10n texts.")]
        public string OutputNotTranslatedTextFile { get; set; }

        [Option("patch", Required = false, HelpText = "patch name")]
        public string PatchName { get; set; }

        [Option("patch_input_data_dir", Required = false, HelpText = "patch input data root dir")]
        public string PatchInputDataDir { get; set; }
    }
}
