// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.Defs;
using Luban.Diagnostics;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;

namespace Luban.Schema.Builtin;

public static class SchemaLoaderUtil
{
    public static List<string> CreateGroups(string s)
    {
        return s.Split(',', ';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public static RawTable CreateTable(string schemaFile, string name, string module, string valueType, string index, string mode, string group,
        string comment, bool readSchemaFromFile, string input, string tags, string outputFileName, string variant = null)
    {
        var source = SchemaSource.FromPath(schemaFile);
        var p = new RawTable()
        {
            Source = source,
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
            Variants = DefUtil.ParseVariant(variant ?? ""),
        };
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new LubanException(source, "error.schema.table_empty_name", source?.Display ?? schemaFile, p.Name);
        }
        if (string.IsNullOrWhiteSpace(valueType))
        {
            throw new LubanException(source, "error.schema.table_empty_value_type", source?.Display ?? schemaFile, p.Name, valueType);
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
        var source = SchemaSource.FromPath(schemaFile);
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
                    throw new LubanException(source, "error.schema.singleton_index", source?.Display ?? schemaFile, tableName, modeStr);
                }
                mode = TableMode.ONE;
                break;
            }
            case "map":
            {
                if (!string.IsNullOrWhiteSpace(indexStr) && indexs.Length > 1)
                {
                    throw new LubanException(source, "error.schema.map_multi_index", source?.Display ?? schemaFile, tableName, indexStr);
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
