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

using System.Xml.Linq;
using Luban.DataLoader;
using Luban.DataLoader.Builtin.Excel;
using Luban.Datas;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaLoader("table", "xlsx", "xls", "xlsm", "csv")]
[SchemaLoader("bean", "xlsx", "xls", "xlsm", "csv")]
[SchemaLoader("enum", "xlsx", "xls", "xlsm", "csv")]
public class ExcelSchemaLoader : SchemaLoaderBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public override void Load(string fileName)
    {
        switch (Type)
        {
            case "table":
                LoadTableListFromFile(fileName);
                break;
            case "bean":
                LoadBeanListFromFile(fileName);
                break;
            case "enum":
                LoadEnumListFromFile(fileName);
                break;
            default:
                throw new Exception($"unknown type:{Type}");
        }
    }


    private void LoadTableListFromFile(string fileName)
    {
        var defTableRecordType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__TableRecord__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = "full_name", Type = "string" },
                new() { Name = "value_type", Type = "string" },
                new() { Name = "index", Type = "string" },
                new() { Name = "mode", Type = "string" },
                new() { Name = "group", Type = "string" },
                new() { Name = "comment", Type = "string" },
                new() { Name = "read_schema_from_file", Type = "bool" },
                new() { Name = "input", Type = "string" },
                new() { Name = "output", Type = "string" },
                new() { Name = "tags", Type = "string" },
            }
        })
        {
            Assembly = new DefAssembly(new RawAssembly()
            {
                Targets = new List<RawTarget> { new() { Name = "default", Manager = "Tables" } },
            }, "default", new List<string>(), null, null),
        };
        defTableRecordType.PreCompile();
        defTableRecordType.Compile();
        defTableRecordType.PostCompile();
        var tableRecordType = TBean.Create(false, defTableRecordType, null);

        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));
        var records = DataLoaderManager.Ins.LoadTableFile(tableRecordType, actualFile, sheetName, new Dictionary<string, string>());
        foreach (var r in records)
        {
            DBean data = r.Data;
            //s_logger.Info("== read text:{}", r.Data);
            string fullName = (data.GetField("full_name") as DString).Value.Trim();
            string name = TypeUtil.GetName(fullName);
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"file:{actualFile} 定义了一个空的table类名");
            }
            string module = TypeUtil.GetNamespace(fullName);
            string valueType = (data.GetField("value_type") as DString).Value.Trim();
            string index = (data.GetField("index") as DString).Value.Trim();
            string mode = (data.GetField("mode") as DString).Value.Trim();
            string group = (data.GetField("group") as DString).Value.Trim();
            string comment = (data.GetField("comment") as DString).Value.Trim();
            bool readSchemaFromFile = (data.GetField("read_schema_from_file") as DBool).Value;
            if (readSchemaFromFile && string.IsNullOrEmpty(TypeUtil.GetNamespace(valueType)))
            {
                valueType = TypeUtil.MakeFullName(module, valueType);
            }
            string inputFile = (data.GetField("input") as DString).Value.Trim();
            // string patchInput = (data.GetField("patch_input") as DString).Value.Trim();
            string tags = (data.GetField("tags") as DString).Value.Trim();
            string outputFile = (data.GetField("output") as DString).Value.Trim();
            // string options = (data.GetField("options") as DString).Value.Trim(); 
            var table = SchemaLoaderUtil.CreateTable(fileName, name, module, valueType, index, mode, group, comment, readSchemaFromFile, inputFile, tags, outputFile);
            Collector.Add(table);
        }
        ;
    }

    private void LoadEnumListFromFile(string fileName)
    {
        var ass = new DefAssembly(new RawAssembly()
        {
            Targets = new List<RawTarget> { new() { Name = "default", Manager = "Tables" } },
        }, "default", new List<string>(), null, null);

        var enumItemType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__EnumItem__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = "name", Type = "string" },
                new() { Name = "alias", Type = "string" },
                new() { Name = "value", Type = "string" },
                new() { Name = "comment", Type = "string" },
                new() { Name = "tags", Type = "string" },
            }
        })
        {
            Assembly = ass,
        };
        ass.AddType(enumItemType);
        enumItemType.PreCompile();
        enumItemType.Compile();
        enumItemType.PostCompile();

        var defTableRecordType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__EnumInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = "full_name", Type = "string" },
                new() { Name = "comment", Type = "string" },
                new() { Name = "flags", Type = "bool" },
                new() { Name = "group", Type = "string" },
                new() { Name = "tags", Type = "string" },
                new() { Name = "unique", Type = "bool" },
                new() { Name = "items", Type = "list,__EnumItem__" },
            }
        })
        {
            Assembly = ass,
        };
        ass.AddType(defTableRecordType);
        defTableRecordType.PreCompile();
        defTableRecordType.Compile();
        defTableRecordType.PostCompile();
        var tableRecordType = TBean.Create(false, defTableRecordType, null);

        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));
        var records = DataLoaderManager.Ins.LoadTableFile(tableRecordType, actualFile, sheetName, new Dictionary<string, string>());

        foreach (var r in records)
        {
            DBean data = r.Data;
            //s_logger.Info("== read text:{}", r.Data);
            string fullName = (data.GetField("full_name") as DString).Value.Trim();
            string name = TypeUtil.GetName(fullName);
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"file:{fileName} 定义了一个空的enum类名");
            }
            string module = TypeUtil.GetNamespace(fullName);

            DList items = (data.GetField("items") as DList);

            var curEnum = new RawEnum()
            {
                Name = name,
                Namespace = module,
                IsFlags = (data.GetField("flags") as DBool).Value,
                Tags = DefUtil.ParseAttrs((data.GetField("tags") as DString).Value),
                Comment = (data.GetField("comment") as DString).Value,
                IsUniqueItemId = (data.GetField("unique") as DBool).Value,
                Groups = SchemaLoaderUtil.CreateGroups((data.GetField("group") as DString).Value.Trim()),
                Items = items.Datas.Cast<DBean>().Select(d => new EnumItem()
                {
                    Name = (d.GetField("name") as DString).Value.Trim(),
                    Alias = (d.GetField("alias") as DString).Value,
                    Value = (d.GetField("value") as DString).Value,
                    Comment = (d.GetField("comment") as DString).Value,
                    Tags = DefUtil.ParseAttrs((d.GetField("tags") as DString).Value),
                }).ToList(),
            };
            Collector.Add(curEnum);
        }
        ;
    }

    private void LoadBeanListFromFile(string fileName)
    {
        var ass = new DefAssembly(new RawAssembly()
        {
            Targets = new List<RawTarget> { new() { Name = "default", Manager = "Tables" } },
        }, "default", new List<string>(), null, null);

        var defBeanFieldType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__FieldInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = "name", Type = "string" },
                new() { Name = "alias", Type = "string" },
                new() { Name = "type", Type = "string" },
                new() { Name = "group", Type = "string" },
                new() { Name = "comment", Type = "string" },
                new() { Name = "tags", Type = "string" },
                new() { Name = "variants", Type = "string" },
            }
        })
        {
            Assembly = ass,
        };

        defBeanFieldType.PreCompile();
        defBeanFieldType.Compile();
        defBeanFieldType.PostCompile();

        ass.AddType(defBeanFieldType);

        var defTableRecordType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__BeanInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = "full_name", Type = "string" },
                new() {Name =  "parent", Type = "string" },
                new() { Name = "valueType", Type = "bool" },
                new() { Name = "sep", Type = "string" },
                new() { Name = "alias", Type = "string" },
                new() { Name = "comment", Type = "string" },
                new() { Name = "tags", Type = "string" },
                new() { Name = "group", Type = "string" },
                new() { Name = "fields", Type = "list,__FieldInfo__" },
            }
        })
        {
            Assembly = ass,
        };
        ass.AddType(defTableRecordType);
        defTableRecordType.PreCompile();
        defTableRecordType.Compile();
        defTableRecordType.PostCompile();
        var tableRecordType = TBean.Create(false, defTableRecordType, null);
        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));
        var records = DataLoaderManager.Ins.LoadTableFile(tableRecordType, actualFile, sheetName, new Dictionary<string, string>());

        foreach (var r in records)
        {
            DBean data = r.Data;
            //s_logger.Info("== read text:{}", r.Data);
            string fullName = (data.GetField("full_name") as DString).Value.Trim();
            string name = TypeUtil.GetName(fullName);
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"file:'{fileName}' 定义了一个空bean类名");
            }
            string module = TypeUtil.GetNamespace(fullName);

            string parent = (data.GetField("parent") as DString).Value.Trim();
            string sep = (data.GetField("sep") as DString).Value.Trim();
            string alias = (data.GetField("alias") as DString).Value.Trim();
            string comment = (data.GetField("comment") as DString).Value.Trim();
            string tags = (data.GetField("tags") as DString).Value.Trim();
            string group = (data.GetField("group") as DString).Value.Trim();
            DList fields = data.GetField("fields") as DList;
            var curBean = new RawBean()
            {
                Name = name,
                Namespace = module,
                IsValueType = ((DBool)data.GetField("valueType")).Value,
                Sep = sep,
                Alias = alias,
                Comment = comment,
                Tags = DefUtil.ParseAttrs(tags),
                Groups = SchemaLoaderUtil.CreateGroups(group),
                Parent = parent,
                Fields = fields.Datas.Select(d => (DBean)d).Select(b => SchemaLoaderUtil.CreateField(
                    fileName,
                    (b.GetField("name") as DString).Value.Trim(),
                    (b.GetField("alias") as DString).Value.Trim(),
                    (b.GetField("type") as DString).Value.Trim(),
                    (b.GetField("group") as DString).Value,
                    (b.GetField("comment") as DString).Value.Trim(),
                    (b.GetField("tags") as DString).Value.Trim(),
                    (b.GetField("variants") as DString).Value.Trim(),
                    false
                )).ToList(),
            };
            Collector.Add(curBean);
        }
        ;
    }
}
