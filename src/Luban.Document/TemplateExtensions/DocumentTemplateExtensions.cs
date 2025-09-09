using Luban.Defs;
using Luban.Document.TypeVisitors;
using Luban.TemplateExtensions;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Document.TemplateExtensions
{
    public class DocumentTemplateExtensions : ScriptObject
    {
        private static Dictionary<DefTypeBase, HashSet<DefTypeBase>> _referenceTree = new();

        public static void AddReference(DefTypeBase from, DefTypeBase to)
        {
            if (!_referenceTree.TryGetValue(from, out var set))
            {
                set = new HashSet<DefTypeBase>();
                _referenceTree.Add(from, set);
            }
            set.Add(to);
        }

        public static string GetDataFilePath(DefTypeBase defTypeBase)
        {
            (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(defTypeBase.DefineFile));
            var workspace = FileUtil.Standardize(Environment.CurrentDirectory);
            if (actualFile.StartsWith(workspace))
            {
                var relativePath = actualFile.Substring(workspace.Length).TrimStart('/');
                return Result(sheetName, relativePath);
            }
            return Result(sheetName, actualFile);

            string Result(string sheetName, string file)
            {
                if (string.IsNullOrEmpty(sheetName))
                {
                    return file;
                }
                else
                {
                    return $"{sheetName}@{file}";
                }
            }
        }

        public static bool HasReferences(DefTypeBase from)
        {
            return _referenceTree.ContainsKey(from);
        }

        public static DefTypeBase[] GetReferences(DefTypeBase from)
        {
            if (_referenceTree.TryGetValue(from, out var set))
            {
                return set.ToArray();
            }
            return Array.Empty<DefTypeBase>();
        }

        public static string GetDefDocumentPath(DefTypeBase def)
        {
            var dirName = def switch
            {
                DefEnum e => "枚举",
                DefBean b => "结构",
                DefTable t => "表",
                _ => throw new Exception($"unknown def type:{def.GetType().FullName}"),
            };
            var fullName = def.FullName;
            var topModule = TypeUtil.MakeFullName(GenerationContext.Current.TopModule, fullName);
            var fullPath = fullName.Replace('.', '/');
            // 最后一部分前面加上@，表示是文件名
            var parts = fullPath.Split('/');
            parts[^1] = "@" + parts[^1];
            var path = string.Join("/", parts);
            return $"{dirName}/{path}";
        }

        public static string GetSideBarLabel(DefTypeBase def)
        {
            if (def is DefBean b)
            {
                if (!string.IsNullOrEmpty(b.Alias))
                {
                    return $"{b.Name} - {b.Alias}";
                }
            }
            return DefaultReturn();


            string DefaultReturn()
            {
                if (!string.IsNullOrEmpty(def.Comment))
                {
                    return $"{def.Name} - {def.Comment}";
                }
                return def.Name;
            }
        }

        public static string GetTTypeDocumentPath(TType type)
        {
            if (type is TEnum e)
            {
                return GetDefDocumentPath(e.DefEnum);
            }
            else if (type is TBean b)
            {
                return GetDefDocumentPath(b.DefBean);
            }
            else
            {
                return "";
            }
        }

        public static string GetTableGroups(DefTable table)
        {
            if (table.Groups.Count == 0)
            {
                return GetDefaultGroups();
            }
            return string.Join(", ", table.Groups);
        }

        public static string GetBeanGroups(DefBean bean)
        {
            if (bean.Groups.Count == 0)
            {
                return GetDefaultGroups();
            }
            return string.Join(", ", bean.Groups);
        }

        public static string GetEnumGroups(DefEnum e)
        {
            if (e.Groups.Count == 0)
            {
                return GetDefaultGroups();
            }
            return string.Join(", ", e.Groups);
        }

        public static string GetFieldGroups(DefField field)
        {
            if (field.Groups.Count == 0)
            {
                return GetDefaultGroups();
            }
            return string.Join(", ", field.Groups);
        }

        private static string GetDefaultGroups()
        {
            var globalGroups = GenerationContext.GlobalConf.Groups;
            var defaultGroups = globalGroups.Where(g => g.IsDefault).SelectMany(g => g.Names).Distinct();
            return string.Join(", ", defaultGroups);
        }

        public static DefTable GetFieldRefTable(DefField field)
        {
            if (TypeTemplateExtension.CanGenerateRef(field))
            {
                return TypeTemplateExtension.GetRefTable(field);
            }
            else if (TypeTemplateExtension.CanGenerateCollectionRef(field))
            {
                return TypeTemplateExtension.GetCollectionRefTable(field);
            }
            else
            {
                return null;
            }
        }

        public static string GetTypeNameAndLink(TType type, string rootPath)
        {
            return type.Apply(DocumentTypeNameVisitor.Ins, rootPath);
        }

        public static string ParseCharForMarkdown(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                switch (c)
                {
                    case '\\':
                    case '`':
                    case '*':
                    case '_':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                    case '#':
                    case '+':
                    case '-':
                    case '.':
                    case '!':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
