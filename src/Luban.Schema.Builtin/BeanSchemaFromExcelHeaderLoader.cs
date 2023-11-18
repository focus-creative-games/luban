using Luban.DataLoader.Builtin.Excel;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[BeanSchemaLoader("default")]
public class BeanSchemaFromExcelHeaderLoader : IBeanSchemaLoader
{
    public RawBean Load(string fileName, string beanFullName)
    {
        return LoadTableValueTypeDefineFromFile(fileName, beanFullName);
    }

    public static RawBean LoadTableValueTypeDefineFromFile(string fileName, string valueTypeFullName)
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
        using var inputStream = new FileStream(actualFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var tableDefInfo = SheetLoadUtil.LoadSheetTableDefInfo(actualFile, sheetName, inputStream);

        foreach (var (name, f) in tableDefInfo.FieldInfos)
        {
            var cf = new RawField()
            {
                Name = name,
                Groups = new List<string>(),
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
