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

using System.Text.Json;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaLoader("", "json")]
public class JsonSchemaLoader : SchemaLoaderBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _fileName;
    private readonly Stack<string> _namespaceStack = new();

    private string CurNamespace => _namespaceStack.TryPeek(out var ns) ? ns : "";

    public override void Load(string fileName)
    {
        s_logger.Debug("JsonSchemaLoader.Load called with fileName: {fileName}", fileName);
        
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentException($"FileName cannot be null or empty: {fileName}");
        }
        
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException($"Schema file not found: {fileName}");
        }
        
        _fileName = fileName;
        
        try
        {
            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var doc = JsonDocument.Parse(stream, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });
            ProcessRoot(doc.RootElement);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "Error loading JSON schema from file: {fileName}", fileName);
            throw;
        }
    }

    private void ProcessRoot(JsonElement root)
    {
        if (root.TryGetProperty("modules", out var modules) && modules.ValueKind == JsonValueKind.Array)
        {
            foreach (var module in modules.EnumerateArray())
            {
                ProcessModule(module);
            }
        }
        else
        {
            // If no modules array, treat the root as a module
            ProcessModule(root);
        }
    }

    private void ProcessModule(JsonElement module)
    {
        var name = GetOptionalString(module, "name")?.Trim();
        _namespaceStack.Push(_namespaceStack.Count > 0 ? TypeUtil.MakeFullName(_namespaceStack.Peek(), name) : name);

        // Process enums
        if (module.TryGetProperty("enums", out var enums) && enums.ValueKind == JsonValueKind.Array)
        {
            foreach (var enumDef in enums.EnumerateArray())
            {
                AddEnum(enumDef);
            }
        }

        // Process beans
        if (module.TryGetProperty("beans", out var beans) && beans.ValueKind == JsonValueKind.Array)
        {
            foreach (var bean in beans.EnumerateArray())
            {
                AddBean(bean);
            }
        }

        // Process tables
        if (module.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
        {
            foreach (var table in tables.EnumerateArray())
            {
                AddTable(table);
            }
        }

        // Process refgroups
        if (module.TryGetProperty("refgroups", out var refgroups) && refgroups.ValueKind == JsonValueKind.Array)
        {
            foreach (var refgroup in refgroups.EnumerateArray())
            {
                AddRefGroup(refgroup);
            }
        }

        // Process constaliases
        if (module.TryGetProperty("constaliases", out var constAliases) && constAliases.ValueKind == JsonValueKind.Array)
        {
            foreach (var constAlias in constAliases.EnumerateArray())
            {
                AddConstAlias(constAlias);
            }
        }

        _namespaceStack.Pop();
    }

    private void AddEnum(JsonElement enumDef)
    {
        var en = new RawEnum()
        {
            Name = GetRequiredString(enumDef, "name").Trim(),
            Namespace = CurNamespace,
            Comment = GetOptionalString(enumDef, "comment"),
            IsFlags = GetOptionalBool(enumDef, "flags"),
            Tags = DefUtil.ParseAttrs(GetOptionalString(enumDef, "tags")),
            IsUniqueItemId = GetOptionalBool(enumDef, "unique", true),
            Groups = SchemaLoaderUtil.CreateGroups(GetOptionalString(enumDef, "group")),
            Items = new(),
            TypeMappers = new(),
        };

        if (enumDef.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in items.EnumerateArray())
            {
                en.Items.Add(new EnumItem()
                {
                    Name = GetRequiredString(item, "name").Trim(),
                    Alias = GetOptionalString(item, "alias"),
                    Value = GetOptionalString(item, "value"),
                    Comment = GetOptionalString(item, "comment"),
                    Tags = DefUtil.ParseAttrs(GetOptionalString(item, "tags")),
                });
            }
        }

        s_logger.Trace("add enum:{@}", en);
        Collector.Add(en);
    }

    private void AddBean(JsonElement beanDef)
    {
        AddBean(beanDef, "");
    }

    private void AddBean(JsonElement beanDef, string parent)
    {
        var b = new RawBean()
        {
            Name = GetRequiredString(beanDef, "name"),
            Namespace = CurNamespace,
            Parent = parent,
            IsValueType = GetOptionalBool(beanDef, "valueType"),
            Alias = GetOptionalString(beanDef, "alias"),
            Sep = GetOptionalString(beanDef, "sep"),
            Comment = GetOptionalString(beanDef, "comment"),
            Tags = DefUtil.ParseAttrs(GetOptionalString(beanDef, "tags")),
            Groups = SchemaLoaderUtil.CreateGroups(GetOptionalString(beanDef, "group")),
            Fields = new(),
            TypeMappers = new(),
        };

        // Update parent if specified in this bean
        string selfDefParent = GetOptionalString(beanDef, "parent");
        if (!string.IsNullOrEmpty(selfDefParent))
        {
            if (!string.IsNullOrEmpty(parent))
            {
                throw new Exception($"嵌套在'{parent}'中定义的子bean:'{GetRequiredString(beanDef, "name")}' 不能再定义parent:{selfDefParent} 属性");
            }
            b.Parent = selfDefParent;
        }

        var childBeans = new List<JsonElement>();

        if (beanDef.TryGetProperty("fields", out var fields) && fields.ValueKind == JsonValueKind.Array)
        {
            foreach (var field in fields.EnumerateArray())
            {
                b.Fields.Add(CreateField(field));
            }
        }

        s_logger.Trace("add bean:{@bean}", b);
        Collector.Add(b);

        // Process child beans
        if (beanDef.TryGetProperty("beans", out var beans) && beans.ValueKind == JsonValueKind.Array)
        {
            var fullname = b.FullName;
            foreach (var childBean in beans.EnumerateArray())
            {
                AddBean(childBean, fullname);
            }
        }
    }

    private void AddTable(JsonElement tableDef)
    {
        string name = GetRequiredString(tableDef, "name");
        string module = CurNamespace;
        string valueType = GetRequiredString(tableDef, "value");
        bool defineFromFile = GetOptionalBool(tableDef, "readSchemaFromFile");
        
        if (string.IsNullOrEmpty(TypeUtil.GetNamespace(valueType)))
        {
            valueType = TypeUtil.MakeFullName(module, valueType);
        }
        
        string index = GetOptionalString(tableDef, "index");
        string group = GetOptionalString(tableDef, "group");
        string comment = GetOptionalString(tableDef, "comment");
        string input = GetRequiredString(tableDef, "input");
        string mode = GetOptionalString(tableDef, "mode");
        string tags = GetOptionalString(tableDef, "tags");
        string output = GetOptionalString(tableDef, "output");

        Collector.Add(SchemaLoaderUtil.CreateTable(_fileName, name, module, valueType, index, mode, group, comment, defineFromFile, input, tags, output));
    }

    private void AddRefGroup(JsonElement refgroupDef)
    {
        string name = GetRequiredString(refgroupDef, "name");
        var items = new List<string>();

        if (refgroupDef.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in itemsElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    items.Add(item.GetString());
                }
            }
        }

        var refGroup = new RawRefGroup()
        {
            Name = name,
            Refs = items,
        };

        Collector.Add(refGroup);
    }

    private void AddConstAlias(JsonElement constAliasDef)
    {
        string name = GetRequiredString(constAliasDef, "name");
        string alias = GetRequiredString(constAliasDef, "value");
        if (string.IsNullOrEmpty(name))
        {
            throw new LoadDefException($"定义文件:{_fileName} 中的constalias的name不能为空");
        }
        if (string.IsNullOrEmpty(alias))
        {
            throw new LoadDefException($"定义文件:{_fileName} 中的constalias `{name}`的value不能为空");
        }
        Collector.AddConstAlias(name, alias);
    }

    private RawField CreateField(JsonElement fieldDef)
    {
        return SchemaLoaderUtil.CreateField(_fileName,
            GetRequiredString(fieldDef, "name"),
            GetOptionalString(fieldDef, "alias"),
            GetRequiredString(fieldDef, "type"),
            GetOptionalString(fieldDef, "group"),
            GetOptionalString(fieldDef, "comment"),
            GetOptionalString(fieldDef, "tags"),
            GetOptionalString(fieldDef, "variants"),
            false
        );
    }

    private string GetRequiredString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
        {
            return prop.GetString();
        }
        throw new LoadDefException($"定义文件:{_fileName} 缺少必需的属性 '{propertyName}'");
    }

    private string GetOptionalString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String ? prop.GetString() : null;
    }

    private bool GetOptionalBool(JsonElement element, string propertyName, bool defaultValue = false)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.True || prop.ValueKind == JsonValueKind.False ? prop.GetBoolean() : defaultValue;
    }
}