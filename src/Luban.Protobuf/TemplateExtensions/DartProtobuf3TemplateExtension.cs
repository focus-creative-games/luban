using Luban.Defs;
using Scriban.Runtime;

namespace Luban.Dart.TemplateExtensions
{
    class DartProtobuf3TemplateExtension : ScriptObject
    {
        public static string DeclaringImportTable(DefTable table, string RootFolder)
        {
            return $"import '{RootFolder}{GetImportName(table.FullName)}.dart';\n";
        }
        public static string GetImportName(string name)
        {
            return name.Replace('.', '/');
        }
        public static string FormatNameLowerCamel(string s)
        {
            return char.ToLower(s[0]) + s.Substring(1);
        }

        public static string ProtoFullName(string typeName)
        {
            var name = string.Join("", typeName.Split('.'));

            return name.Substring(0, 1).ToUpper() + name.Substring(1);
        }
    }
}
