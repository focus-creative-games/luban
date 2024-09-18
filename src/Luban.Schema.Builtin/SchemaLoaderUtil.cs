using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

public static class SchemaLoaderUtil
{
    public static List<string> CreateGroups(string s)
    {
        return s.Split(',', ';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public static RawTable CreateTable(string schemaFile, string name, string module, string valueType, string index, string mode, string group,
        string comment, bool readSchemaFromFile, string input, string tags, string outputFileName)
    {
        var p = new RawTable()
        {
            Name = name,
            Namespace = module,
            ValueType = valueType,
            ReadSchemaFromFile = readSchemaFromFile,
            Index = index,
            Groups = CreateGroups(group),
            Comment = comment,
            Mode = ConvertMode(schemaFile, name, mode, index),
            Tags = DefUtil.ParseAttrs(tags),
            OutputFile = outputFileName,
        };
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception($"定义文件:{schemaFile} table:'{p.Name}' name:'{p.Name}' 不能为空");
        }
        if (string.IsNullOrWhiteSpace(valueType))
        {
            throw new Exception($"定义文件:{schemaFile} table:'{p.Name}' value_type:'{valueType}' 不能为空");
        }
        p.InputFiles.AddRange(input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)));

        // if (!string.IsNullOrWhiteSpace(patchInput))
        // {
        //     foreach (var subPatchStr in patchInput.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)))
        //     {
        //         var nameAndDirs = subPatchStr.Split(':');
        //         if (nameAndDirs.Length != 2)
        //         {
        //             throw new Exception($"定义文件:{schemaFile} table:'{p.Name}' patch_input:'{subPatchStr}' 定义不合法");
        //         }
        //         var patchDirs = nameAndDirs[1].Split(',', ';').ToList();
        //         if (!p.PatchInputFiles.TryAdd(nameAndDirs[0], patchDirs))
        //         {
        //             throw new Exception($"定义文件:{schemaFile} table:'{p.Name}' patch_input:'{subPatchStr}' 子patch:'{nameAndDirs[0]}' 重复");
        //         }
        //     }
        // }

        return p;
    }

    public static TableMode ConvertMode(string schemaFile, string tableName, string modeStr, string indexStr)
    {
        TableMode mode;
        string[] indexs = indexStr.Split(',', '+');
        switch (modeStr)
        {
            case "one":
            case "single":
            case "singleton":
            {
                if (!string.IsNullOrWhiteSpace(indexStr))
                {
                    throw new Exception($"定义文件:{schemaFile} table:'{tableName}' mode={modeStr} 是单例表，不支持定义index属性");
                }
                mode = TableMode.ONE;
                break;
            }
            case "map":
            {
                if (!string.IsNullOrWhiteSpace(indexStr) && indexs.Length > 1)
                {
                    throw new Exception($"定义文件:'{schemaFile}' table:'{tableName}' 是单主键表，index:'{indexStr}'不能包含多个key");
                }
                mode = TableMode.MAP;
                break;
            }
            case "list":
            {
                mode = TableMode.LIST;
                break;
            }
            case "":
            {
                if (string.IsNullOrWhiteSpace(indexStr) || indexs.Length == 1)
                {
                    mode = TableMode.MAP;
                }
                else
                {
                    mode = TableMode.LIST;
                }
                break;
            }
            default:
            {
                throw new ArgumentException($"不支持的 mode:{modeStr}");
            }
        }
        return mode;
    }

    public static RawField CreateField(string schemaFile, string name, string alias, string type, string group,
        string comment, string tags, string variants,
        bool ignoreNameValidation)
    {
        var f = new RawField()
        {
            Name = name,
            Alias = alias,
            Groups = CreateGroups(group),
            Comment = comment,
            Tags = DefUtil.ParseAttrs(tags),
            Variants = DefUtil.ParseVariant(variants),
            NotNameValidation = ignoreNameValidation,
        };

        f.Type = type;

        //FillValueValidator(f, refs, "ref");
        //FillValueValidator(f, path, "path"); // (ue4|unity|normal|regex);xxx;xxx
        //FillValueValidator(f, range, "range");

        //FillValidators(defileFile, "key_validator", keyValidator, f.KeyValidators);
        //FillValidators(defileFile, "value_validator", valueValidator, f.ValueValidators);
        //FillValidators(defileFile, "validator", validator, f.Validators);
        return f;
    }
}
