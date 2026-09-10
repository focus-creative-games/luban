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

using Luban.DataLoader.Builtin.Excel;
using Luban.Diagnostics;
using Luban.RawDefs;
using Luban.Schema;
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
        var source = SchemaSource.FromPath(fileName);
        var cb = new RawBean()
        {
            Source = source,
            Namespace = valueTypeNamespace,
            Name = valueTypeName,
            Comment = table.Comment,
            Parent = "",
            Groups = new(),
            Fields = new(),
            Tags = new Dictionary<string, string>(),
        };


        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));

        if (!File.Exists(actualFile))
        {
            if (Directory.Exists(fileName))
            {
                var files = FileUtil.GetFileOrDirectory(Directory.GetParent(fileName).FullName, fileName);
                var firstExcelFile = files.FirstOrDefault(f => FileUtil.IsExcelFile(f));
                if (firstExcelFile == null)
                {
                    throw new LubanException(source, "error.schema.excel_dir_required", table.Name, valueTypeFullName, source?.Display ?? fileName);
                }
                actualFile = firstExcelFile;
                source = SchemaSource.Create(actualFile, sheetName);
                cb.Source = source;
            }
            else
            {
                throw new LubanException(source ?? table.Source, "error.schema.input_not_found", table.Name, source?.Display ?? fileName);
            }
        }
        else if (!FileUtil.IsExcelFile(actualFile))
        {
            throw new LubanException(source, "error.schema.excel_file_required", table.Name, valueTypeFullName, source?.Display ?? fileName);
        }
        else
        {
            // Prefer actualFile + sheet after split (relative to conf).
            source = SchemaSource.Create(actualFile, sheetName);
            cb.Source = source;
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
                    throw new LubanException(source, "error.schema.invalid_title", source?.Display ?? fileName, name);
                }
                string actualName = splitName[0];
                string variantName = splitName[1];
                RawField rawField = cb.Fields.Find(f => f.Name == actualName);
                if (rawField == null)
                {
                    throw new LubanException(source, "error.schema.variant_field_not_found", source?.Display ?? fileName, actualName, name);
                }
                rawField.Variants.Add(variantName);
                continue;
            }

            var cf = new RawField()
            {
                Name = name,
                Groups = new List<string>(),
                Variants = new List<string>(),
                Tags = new Dictionary<string, string>(),
            };

            string[] attrs = StringUtil.SplitStringWithEscape(f.Type.Trim(), '&').Select(s => s.Trim()).ToArray();

            if (attrs.Length == 0 || string.IsNullOrWhiteSpace(attrs[0]))
            {
                throw new LubanException(source, "error.schema.title_type_missing", source?.Display ?? fileName, name);
            }

            cf.Comment = f.Desc;
            cf.Type = attrs[0];
            for (int i = 1; i < attrs.Length; i++)
            {
                var pair = attrs[i].Split('=', 2);
                if (pair.Length != 2)
                {
                    throw new LubanException(source, "error.schema.invalid_title_attr", source?.Display ?? fileName, name, attrs[i]);
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
                        throw new LubanException(source, "error.schema.title_type_attr", source?.Display ?? fileName, name, attrName, cf.Type, attrs[i]);
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
                        throw new LubanException(source, "error.schema.invalid_title_attr", source?.Display ?? fileName, name, attrs[i]);
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
