using CommandLine;
using Luban.Job.Common;

namespace Luban.Job.Cfg
{
    public class GenArgs : GenArgsBase
    {
        [Option('s', "service", Required = true, HelpText = "service")]
        public string Service { get; set; }

        [Option("gen_types", Required = true, HelpText = "code_cs_bin,code_cs_dotnet_json,code_cs_unity_json,code_cs_unity_editor_json,code_lua_bin,code_java_bin,code_java_json,code_go_bin,code_go_json,code_cpp_bin,code_cpp_ue_editor_json,code_python2_json,code_python3_json,code_typescript_bin,code_typescript_json,code_rust_json,code_protobuf,code_template,code_flatbuffers,code_gdscript_json,data_bin,data_bidx,data_lua,data_json,data_json2,data_json_monolithic,data_bson,data_yaml,data_xml,data_resources,data_template,data_protobuf_bin,data_protobuf_json,data_flatbuffers_json,convert_json,convert_lua,convert_xlsx . can be multi")]
        public string GenType { get; set; }

        [Option("input_data_dir", Required = true, HelpText = "input data dir")]
        public string InputDataDir { get; set; }

        [Option('v', "validate_root_dir", Required = false, HelpText = "validate root directory")]
        public string ValidateRootDir { get; set; }

        [Option("output_data_dir", Required = false, HelpText = "output data directory")]
        public string OutputDataDir { get; set; }

        [Option("input:convert:data_dir", Required = false, HelpText = "override input data dir with convert data dir")]
        public string InputConvertDataDir { get; set; }

        [Option("output:tables", Required = false, HelpText = "override tables in export group with this list")]
        public string OutputTables { get; set; }

        [Option("output:include_tables", Required = false, HelpText = "include tables")]
        public string OutputIncludeTables { get; set; }

        [Option("output:exclude_tables", Required = false, HelpText = "exclude tables")]
        public string OutputExcludeTables { get; set; }

        [Option("output:data:resource_list_file", Required = false, HelpText = "output resource list file")]
        public string OutputDataResourceListFile { get; set; }

        [Option("output:data:json_monolithic_file", Required = false, HelpText = "output monolithic json file")]
        public string OutputDataJsonMonolithicFile { get; set; }

        [Option("output:data:file_extension", Required = false, HelpText = "data file name extension. default choose by gen_type")]
        public string OutputDataFileExtension { get; set; }

        [Option("output:data:compact_json", Required = false, HelpText = "output compact json data. drop blank characters. ")]
        public bool OutputDataCompactJson { get; set; }

        [Option("output:exclude_tags", Required = false, HelpText = "export exclude tags. default export all tags")]
        public string OutputExcludeTags { get; set; } = "";

        [Option("template:data:file", Required = false, HelpText = "template name. use with gen_types=data_template")]
        public string TemplateDataFile { get; set; }

        [Option("template:code:dir", Required = false, HelpText = "code template dir. use with gen_types=code_template")]
        public string TemplateCodeDir { get; set; }

        [Option("template:convert:file", Required = false, HelpText = "convert template file name. use with gen_tpes=convert_template")]
        public string TemplateConvertFile { get; set; }

        [Option("output:convert:file_extension", Required = false, HelpText = "output convert file extension. default guess by convert template name")]
        public string OutputConvertFileExtension { get; set; }

        [Option("l10n:timezone", Required = false, HelpText = "timezone")]
        public string L10nTimeZone { get; set; }

        [Option("l10n:input_text_files", Required = false, HelpText = "input l10n text table files. can be multi, sep by ','")]
        public string L10nInputTextTableFiles { get; set; }

        [Option("l10n:text_field_name", Required = false, HelpText = "text value field name of text table files. default is text")]
        public string L10nTextValueFieldName { get; set; }

        [Option("l10n:output_not_translated_text_file", Required = false, HelpText = "the file save not translated l10n texts.")]
        public string L10nOutputNotTranslatedTextFile { get; set; }

        [Option("l10n:patch", Required = false, HelpText = "patch name")]
        public string L10nPatchName { get; set; }

        [Option("l10n:patch_input_data_dir", Required = false, HelpText = "patch input data root dir")]
        public string L10nPatchInputDataDir { get; set; }
    }
}
