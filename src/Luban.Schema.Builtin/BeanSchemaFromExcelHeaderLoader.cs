using Luban.DataLoader.Builtin.Excel;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[BeanSchemaLoader("default")]
public class BeanSchemaFromExcelHeaderLoader : IBeanSchemaLoader
{
    public RawBean Load(string fileName, string beanFullName, RawTable table)
    {
        return LoadTableValueTypeDefineFromFile(fileName, beanFullName, table);
    }

    public static RawBean LoadTableValueTypeDefineFromFile(string fileName, string valueTypeFullName, RawTable table)
    {
        var valueTypeNamespace = TypeUtil.GetNamespace(valueTypeFullName);
        string valueTypeName = TypeUtil.GetName(valueTypeFullName);
        var cb = new RawBean()
        {
            Namespace = valueTypeNamespace,
            Name = valueTypeName,
            Comment = "",
            Parent = "",
            Groups = new(),
            Fields = new(),
        };


        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));

        if (!File.Exists(actualFile))
        {
            if (Directory.Exists(fileName))
            {
                var files = FileUtil.GetFileOrDirectory(fileName);
                var firstExcelFile = files.FirstOrDefault(f => FileUtil.IsExcelFile(f));
                if (firstExcelFile == null)
                {
                    throw new Exception($"table: '{table.Name}' valueType:'{valueTypeFullName}' directory:'{fileName}', 当table的ReadSchemaFromFile为true时，目录下必须有excel文件");
                }
                actualFile = firstExcelFile;
            }
            else
            {
                throw new Exception($"table '{table.Name}' intput path:'{fileName}' not found");
            }
        }
        else if (!FileUtil.IsExcelFile(actualFile))
        {
            throw new Exception($"table: '{table.Name}' valueType:'{valueTypeFullName}' file:'{fileName}'，当table的ReadSchemaFromFile为true时，文件必须是excel文件");
        }

        using var inputStream = new FileStream(actualFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        var tableDefInfo = SheetLoadUtil.LoadSheetTableDefInfo(actualFile, sheetName, inputStream);

        foreach (var (name, f) in tableDefInfo.FieldInfos)
        {
            if (name.Contains('@'))
            {
                var splitName = name.Split('@');
                if (splitName.Length != 2)
                {
                    throw new Exception($"file:{fileName} title:'{name}' is invalid!");
                }
                string actualName = splitName[0];
                string variantName = splitName[1];
                RawField rawField = cb.Fields.Find(f => f.Name == actualName);
                if (rawField == null)
                {
                    throw new Exception($"file:{fileName} field:{actualName} not found for variant field:'{name}' not found!");
                }
                rawField.Variants.Add(variantName);
                continue;
            }

            var cf = new RawField()
            {
                Name = name,
                Groups = new List<string>(),
                Variants = new List<string>(),
            };

            string[] attrs = f.Type.Trim().Split('&').Select(s => s.Trim()).ToArray();

            if (attrs.Length == 0 || string.IsNullOrWhiteSpace(attrs[0]))
            {
                throw new Exception($"file:{fileName} title:'{name}' type missing!");
            }

            cf.Comment = f.Desc;
            cf.Type = attrs[0];
            for (int i = 1; i < attrs.Length; i++)
            {
                var pair = attrs[i].Split('=', 2);
                if (pair.Length != 2)
                {
                    throw new Exception($"file:{fileName} title:'{name}' attr:'{attrs[i]}' is invalid!");
                }
                var attrName = pair[0].Trim();
                var attrValue = pair[1].Trim();
                switch (attrName)
                {
                    case "index":
                    case "ref":
                    case "path":
                    case "range":
                    case "sep":
                    case "regex":
                    {
                        throw new Exception($"file:{fileName} title:'{name}' attr:'{attrName}' 属于type的属性，必须用#分割，尝试'{cf.Type}#{attrs[i]}'");
                    }
                    case "group":
                    {
                        cf.Groups = attrValue.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        break;
                    }
                    case "comment":
                    {
                        cf.Comment = attrValue;
                        break;
                    }
                    case "tags":
                    {
                        cf.Tags = DefUtil.ParseAttrs(attrValue);
                        break;
                    }
                    default:
                    {
                        throw new Exception($"file:{fileName} title:'{name}' attr:'{attrs[i]}' is invalid!");
                    }
                }
            }

            if (!string.IsNullOrEmpty(f.Groups))
            {
                cf.Groups = f.Groups.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }

            cb.Fields.Add(cf);
        }
        return cb;
    }
}
