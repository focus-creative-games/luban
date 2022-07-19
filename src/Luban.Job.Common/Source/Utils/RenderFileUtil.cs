using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Utils
{
    public static class RenderFileUtil
    {

        public static string GetDefTypePath(string fullName, ELanguage lan)
        {
            switch (lan)
            {
                case ELanguage.CS: return fullName.Replace('.', '/') + ".cs";
                case ELanguage.JAVA: return fullName.Replace('.', '/') + ".java";
                case ELanguage.CPP: return fullName + ".cpp";
                case ELanguage.GO: return fullName + ".go";
                case ELanguage.LUA: return fullName.Replace('.', '_') + ".lua";
                case ELanguage.JAVASCRIPT: return fullName + ".js";
                case ELanguage.TYPESCRIPT: return fullName.Replace('.', '/') + ".ts";
                case ELanguage.RUST: return fullName.Replace('.', '_') + ".rs";
                case ELanguage.PROTOBUF: return fullName.Replace('.', '_') + ".pb";
                case ELanguage.GDSCRIPT: return fullName.Replace('.', '_') + ".gd";
                default: throw new NotSupportedException();
            }
        }

        public static string GetCsDefTypePath(string fullName)
        {
            return fullName.Replace('.', '/') + ".cs";
        }

        public static string GetGoDefTypePath(string fullName)
        {
            return fullName + ".go";
        }

        public static string GetUeCppDefTypeHeaderFilePath(string fullName)
        {
            return fullName.Replace('.', '_') + ".h";
        }

        public static string GetUeCppDefTypeHeaderFilePathWithoutSuffix(string fullName)
        {
            return fullName.Replace('.', '_');
        }

        public static string GetCppDefTypeCppFilePath(string fullName)
        {
            return fullName + ".cpp";
        }

        public static string GetCppDefTypeHeadFilePath(string fullName)
        {
            return fullName + ".h";
        }

        public static string GetTableDataPath(string fullName)
        {
            return fullName + ".bin";
        }

        private static readonly Dictionary<string, ELanguage> s_name2Lans = new()
        {
            { "cs", ELanguage.CS },
            { "java", ELanguage.JAVA },
            { "go", ELanguage.GO },
            { "cpp", ELanguage.CPP },
            { "lua", ELanguage.LUA },
            { "python", ELanguage.PYTHON },
            { "typescript", ELanguage.TYPESCRIPT },
            { "javascript", ELanguage.JAVASCRIPT },
            { "erlang", ELanguage.ERLANG },
            { "rust", ELanguage.RUST },
            { "protobuf", ELanguage.PROTOBUF },
            { "gdscript", ELanguage.GDSCRIPT },
        };

        public static ELanguage GetLanguage(string genType)
        {
            foreach (var (name, lan) in s_name2Lans)
            {
                if (genType.Contains(name))
                {
                    return lan;
                }
            }
            throw new ArgumentException($"can't guess Language from gen_type:{genType}");
        }

        public static string GetCommonTemplateDirName(ELanguage lan)
        {
            return lan switch
            {
                ELanguage.CS => "cs",
                ELanguage.JAVA => "java",
                ELanguage.GO => "go",
                ELanguage.CPP => "cpp",
                ELanguage.LUA => "lua",
                ELanguage.PYTHON => "python",
                ELanguage.TYPESCRIPT => "typescript",
                ELanguage.JAVASCRIPT => "javascript",
                ELanguage.ERLANG => "erlang",
                ELanguage.RUST => "rust",
                ELanguage.PROTOBUF => "protobuf",
                ELanguage.FLATBUFFERS => "flatbuffers",
                ELanguage.GDSCRIPT => "gdscript",
                _ => throw new Exception($"not support common template dir for lan:{lan}"),
            };
        }

        private static readonly Dictionary<string, string> s_name2Suxxifx = new()
        {
            { "json", "json" },
            { "bson", "bson" },
            { "lua", "lua" },
            { "bin", "bytes" },
            { "bidx", "bytes" },
            { "xml", "xml" },
            { "yaml", "yml" },
            { "yml", "yml" },
            { "erlang", "erl" },
            { "erl", "erl" },
            { "xlsx", "xlsx" },
            { "protobuf", "bytes" },
            { "msgpack", "bytes" },
            { "flatbuffers", "bytes" },
        };

        public static string GetOutputFileSuffix(string genType)
        {
            foreach (var (name, suffix) in s_name2Suxxifx)
            {
                if (genType.Contains(name))
                {
                    return suffix;
                }
            }
            throw new Exception($"not support output data type:{genType}");
        }

        public static string GetOutputFileName(string genType, string fileName, string fileExtension)
        {
            return $"{fileName}.{(string.IsNullOrEmpty(fileExtension) ? GetOutputFileSuffix(genType) : fileExtension)}";
        }

        public static string GetFileOrDefault(string preferFile, string candidateFile)
        {
            return string.IsNullOrWhiteSpace(preferFile) ? candidateFile : preferFile;
        }
    }
}
