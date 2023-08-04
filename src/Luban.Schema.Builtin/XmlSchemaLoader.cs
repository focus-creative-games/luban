using System.Xml.Linq;
using Luban.Core.Defs;
using Luban.Core.RawDefs;
using Luban.Core.Schema;
using Luban.Core.Utils;

namespace Luban.Schema.Default;

[SchemaLoader("", "xml")]
public class XmlSchemaLoader : ISchemaLoader
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static ISchemaLoader Create(string type)
    {
        return new XmlSchemaLoader();
    }

    private readonly Dictionary<string, Action<XElement>> _tagHandlers = new();

    private string _fileName;

    private ISchemaCollector _schemaCollector;

    private readonly Stack<string> _namespaceStack = new();

    private string CurNamespace => _namespaceStack.TryPeek(out var ns) ? ns : "";

    private XmlSchemaLoader()
    {

        _tagHandlers.Add("module", AddModule);
        _tagHandlers.Add("enum", AddEnum);
        _tagHandlers.Add("bean", AddBean);
        _tagHandlers.Add("externaltype", AddExternalType);
        _tagHandlers.Add("table", AddTable);
        _tagHandlers.Add("refgroup", AddRefGroup);
    }

    public void Load(string fileName, ISchemaCollector collector)
    {
        _fileName = fileName;
        _schemaCollector = collector;
        
        XElement doc = XmlUtil.Open(fileName);
        AddModule(doc);
    }

    private void AddModule(XElement me)
    {
        var name = XmlUtil.GetOptionalAttribute(me, "name")?.Trim();
        _namespaceStack.Push(_namespaceStack.Count > 0 ? TypeUtil.MakeFullName(_namespaceStack.Peek(), name) : name);

        // 加载所有module定义,允许嵌套
        foreach (XElement e in me.Elements())
        {
            var tagName = e.Name.LocalName;
            if (_tagHandlers.TryGetValue(tagName, out var handler))
            {
                handler(e);
            }
            else
            {
                throw new LoadDefException($"定义文件:{_fileName} module:{CurNamespace} 不支持 tag:{tagName}");
            }
        }
        _namespaceStack.Pop();
    }

    protected void AddBean(XElement e)
    {
        AddBean(e, "");
    }

    private static readonly List<string> _beanOptinsAttrs1 = new List<string> { "compatible", "value_type", "comment", "tags", "externaltype" };
    private static readonly List<string> _beanRequireAttrs1 = new List<string> { "id", "name" };

    private static readonly List<string> _beanOptinsAttrs2 = new List<string> { "id", "parent", "compatible", "value_type", "comment", "tags"};
    private static readonly List<string> _beanRequireAttrs2 = new List<string> { "name" };


    protected void TryGetUpdateParent(XElement e, ref string parent)
    {
        string selfDefParent = XmlUtil.GetOptionalAttribute(e, "parent");
        if (!string.IsNullOrEmpty(selfDefParent))
        {
            if (!string.IsNullOrEmpty(parent))
            {
                throw new Exception($"嵌套在'{parent}'中定义的子bean:'{XmlUtil.GetRequiredAttribute(e, "name")}' 不能再定义parent:{selfDefParent} 属性");
            }
            parent = selfDefParent;
        }
    }

    static string CreateType(XElement e, string key)
    {
        return XmlUtil.GetRequiredAttribute(e, key);
    }

    private static readonly List<string> _enumOptionalAttrs = new List<string> { "flags", "comment", "tags", "unique", "group" };
    private static readonly List<string> _enumRequiredAttrs = new List<string> { "name" };
    
    private static readonly List<string> _enumItemOptionalAttrs = new List<string> { "value", "alias", "comment", "tags" };
    private static readonly List<string> _enumItemRequiredAttrs = new List<string> { "name" };

    private void AddEnum(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _enumOptionalAttrs, _enumRequiredAttrs);
        var en = new RawEnum()
        {
            Name = XmlUtil.GetRequiredAttribute(e, "name").Trim(),
            Namespace = CurNamespace,
            Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            IsFlags = XmlUtil.GetOptionBoolAttribute(e, "flags"),
            Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
            IsUniqueItemId = XmlUtil.GetOptionBoolAttribute(e, "unique", true),
            Groups = XmlSchemaUtil.CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
            Items = new (),
        };

        foreach (XElement item in e.Elements())
        {
            XmlSchemaUtil.ValidAttrKeys(_fileName, item, _enumItemOptionalAttrs, _enumItemRequiredAttrs);
            en.Items.Add(new EnumItem()
            {
                Name = XmlUtil.GetRequiredAttribute(item, "name"),
                Alias = XmlUtil.GetOptionalAttribute(item, "alias"),
                Value = XmlUtil.GetOptionalAttribute(item, "value"),
                Comment = XmlUtil.GetOptionalAttribute(item, "comment"),
                Tags = XmlUtil.GetOptionalAttribute(item, "tags"),
            });
        }
        s_logger.Trace("add enum:{@}", en);
        _schemaCollector.Add(en);
    }
    
    private static readonly List<string> _externalRequiredAttrs = new List<string> { "name", "origin_type_name" };
    
    private void AddExternalType(XElement e)
    {
        // XmlSchemaUtil.ValidAttrKeys(_fileName, e, null, _externalRequiredAttrs);
        // string name = XmlUtil.GetRequiredAttribute(e, "name");
        //
        // var et = new RawExternalType()
        // {
        //     Name = name,
        //     OriginTypeName = XmlUtil.GetRequiredAttribute(e, "origin_type_name"),
        // };
        // var mappers = new Dictionary<string, ExternalTypeMapper>();
        // foreach (XElement mapperEle in e.Elements())
        // {
        //     var tagName = mapperEle.Name.LocalName;
        //     if (tagName == "mapper")
        //     {
        //         var mapper = CreateMapper(name, mapperEle);
        //         string uniqKey = $"{mapper.Language}##{mapper.Selector}";
        //         if (mappers.ContainsKey(uniqKey))
        //         {
        //             throw new LoadDefException($"定义文件:{_fileName} externaltype name:{name} mapper(lan='{mapper.Language}',selector='{mapper.Selector}') 重复");
        //         }
        //         mappers.Add(uniqKey, mapper);
        //         et.Mappers.Add(mapper);
        //         s_logger.Trace("add mapper. externaltype:{} mapper:{@}", name, mapper);
        //     }
        //     else
        //     {
        //         throw new LoadDefException($"定义文件:{_fileName} externaltype:{name} 非法 tag:'{tagName}'");
        //     }
        // }
        // _schemaCollector.Add(et);
    }

    private static readonly List<string> _mapperOptionalAttrs = new List<string> { };
    private static readonly List<string> _mapperRequiredAttrs = new List<string> { "lan", "selector" };
    private ExternalTypeMapper CreateMapper(string externalType, XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _mapperOptionalAttrs, _mapperRequiredAttrs);
        var m = new ExternalTypeMapper()
        {
            Language = XmlUtil.GetRequiredAttribute(e, "lan"),
            Selector = XmlUtil.GetRequiredAttribute(e, "selector"),
        };
        foreach (XElement attrEle in e.Elements())
        {
            var tagName = attrEle.Name.LocalName;
            switch (tagName)
            {
                case "target_type_name":
                {
                    m.TargetTypeName = attrEle.Value;
                    break;
                }
                case "create_function":
                {
                    m.CreateFunction = attrEle.Value;
                    break;
                }
                default: throw new LoadDefException($"定义文件:{_fileName} external type:{externalType} 非法 tag:{tagName}");
            }
        }
        if (string.IsNullOrWhiteSpace(m.TargetTypeName))
        {
            throw new LoadDefException($"定义文件:{_fileName} external type:{externalType} lan:{m.Language} selector:{m.Selector} 没有定义 'target_type_name'");
        }
        return m;
    }
    
    
    private TableMode ConvertMode(string tableName, string modeStr, string indexStr)
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
                    throw new Exception($"定义文件:{_fileName} table:'{tableName}' mode={modeStr} 是单例表，不支持定义index属性");
                }
                mode = TableMode.ONE;
                break;
            }
            case "map":
            {
                if (!string.IsNullOrWhiteSpace(indexStr) && indexs.Length > 1)
                {
                    throw new Exception($"定义文件:'{_fileName}' table:'{tableName}' 是单主键表，index:'{indexStr}'不能包含多个key");
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

    private readonly List<string> _tableOptionalAttrs = new List<string> { "index", "mode", "group", "patch_input", "comment", "define_from_file", "output", "options" };
    private readonly List<string> _tableRequireAttrs = new List<string> { "name", "value", "input" };

    private void AddTable(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _tableOptionalAttrs, _tableRequireAttrs);
        string name = XmlUtil.GetRequiredAttribute(e, "name");
        string module = CurNamespace;
        string valueType = XmlUtil.GetRequiredAttribute(e, "value");
        bool defineFromFile = XmlUtil.GetOptionBoolAttribute(e, "define_from_file");
        string index = XmlUtil.GetOptionalAttribute(e, "index");
        string group = XmlUtil.GetOptionalAttribute(e, "group");
        string comment = XmlUtil.GetOptionalAttribute(e, "comment");
        string input = XmlUtil.GetRequiredAttribute(e, "input");
        string patchInput = XmlUtil.GetOptionalAttribute(e, "patch_input");
        string mode = XmlUtil.GetOptionalAttribute(e, "mode");
        string tags = XmlUtil.GetOptionalAttribute(e, "tags");
        string output = XmlUtil.GetOptionalAttribute(e, "output");
        string options = XmlUtil.GetOptionalAttribute(e, "options");
        AddTable(name, module, valueType, index, mode, group, comment, defineFromFile, input, patchInput, tags, output, options);
    }

    private void AddTable(string name, string module, string valueType, string index, string mode, string group,
        string comment, bool defineFromExcel, string input, string patchInput, string tags, string outputFileName, string options)
    {
        var p = new RawTable()
        {
            Name = name,
            Namespace = module,
            ValueType = valueType,
            LoadDefineFromFile = defineFromExcel,
            Index = index,
            Groups = XmlSchemaUtil.CreateGroups(group),
            Comment = comment,
            Mode = ConvertMode(name, mode, index),
            Tags = tags,
            OutputFile = outputFileName,
            Options = options,
        };
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new Exception($"定义文件:{_fileName} table:'{p.Name}' name:'{p.Name}' 不能为空");
        }
        if (string.IsNullOrWhiteSpace(valueType))
        {
            throw new Exception($"定义文件:{_fileName} table:'{p.Name}' value_type:'{valueType}' 不能为空");
        }
        p.InputFiles.AddRange(input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)));

        if (!string.IsNullOrWhiteSpace(patchInput))
        {
            foreach (var subPatchStr in patchInput.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                var nameAndDirs = subPatchStr.Split(':');
                if (nameAndDirs.Length != 2)
                {
                    throw new Exception($"定义文件:{_fileName} table:'{p.Name}' patch_input:'{subPatchStr}' 定义不合法");
                }
                var patchDirs = nameAndDirs[1].Split(',', ';').ToList();
                if (!p.PatchInputFiles.TryAdd(nameAndDirs[0], patchDirs))
                {
                    throw new Exception($"定义文件:{_fileName} table:'{p.Name}' patch_input:'{subPatchStr}' 子patch:'{nameAndDirs[0]}' 重复");
                }
            }
        }

        _schemaCollector.Add(p);
    }

    
    private static readonly List<string> _fieldOptionalAttrs = new()
    {
        "ref",
        "path",
        "group",
        "comment",
        "tags",
    };

    private static readonly List<string> _fieldRequireAttrs = new List<string> { "name", "type" };

    protected RawField CreateField(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _fieldOptionalAttrs, _fieldRequireAttrs);

        string typeStr = XmlUtil.GetRequiredAttribute(e, "type");

        string refStr = XmlUtil.GetOptionalAttribute(e, "ref");
        if (!string.IsNullOrWhiteSpace(refStr))
        {
            typeStr = typeStr + "#(ref=" + refStr + ")";
        }
        string pathStr = XmlUtil.GetOptionalAttribute(e, "path");
        if (!string.IsNullOrWhiteSpace(pathStr))
        {
            typeStr = typeStr + "#(path=" + pathStr + ")";
        }

        return CreateField(XmlUtil.GetRequiredAttribute(e, "name"),
            typeStr,
            XmlUtil.GetOptionalAttribute(e, "group"),
            XmlUtil.GetOptionalAttribute(e, "comment"),
            XmlUtil.GetOptionalAttribute(e, "tags"),
            false
        );
    }

    private RawField CreateField(string name, string type, string group,
        string comment, string tags,
        bool ignoreNameValidation)
    {
        var f = new RawField()
        {
            Name = name,
            Groups = XmlSchemaUtil.CreateGroups(group),
            Comment = comment,
            Tags = tags,
            IgnoreNameValidation = ignoreNameValidation,
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

    private static readonly List<string> _beanOptinsAttrs = new List<string> { "parent", "value_type", "alias", "sep", "comment", "tags", "group", "externaltype" };
    private static readonly List<string> _beanRequireAttrs = new List<string> { "name" };

    protected void AddBean(XElement e, string parent)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _beanOptinsAttrs, _beanRequireAttrs);
        TryGetUpdateParent(e, ref parent);
        var b = new RawBean()
        {
            Name = XmlUtil.GetRequiredAttribute(e, "name"),
            Namespace = CurNamespace,
            Parent = parent,
            IsValueType = XmlUtil.GetOptionBoolAttribute(e, "value_type"),
            Alias = XmlUtil.GetOptionalAttribute(e, "alias"),
            Sep = XmlUtil.GetOptionalAttribute(e, "sep"),
            Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
            Groups = XmlSchemaUtil.CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
            Fields = new(),
        };
        var childBeans = new List<XElement>();

        bool defineAnyChildBean = false;
        foreach (XElement fe in e.Elements())
        {
            switch (fe.Name.LocalName)
            {
                case "var":
                {
                    if (defineAnyChildBean)
                    {
                        throw new LoadDefException($"定义文件:{_fileName} 类型:{b.FullName} 的多态子bean必须在所有成员字段 <var> 之后定义");
                    }
                    b.Fields.Add(CreateField(fe)); ;
                    break;
                }
                case "bean":
                {
                    defineAnyChildBean = true;
                    childBeans.Add(fe);
                    break;
                }
                default:
                {
                    throw new LoadDefException($"定义文件:{_fileName} 类型:{b.FullName} 不支持 tag:{fe.Name}");
                }
            }
        }
        s_logger.Trace("add bean:{@bean}", b);
        _schemaCollector.Add(b);

        var fullname = b.FullName;
        foreach (var cb in childBeans)
        {
            AddBean(cb, fullname);
        }
    }

    private void AddRefGroup(XElement e)
    {
        _schemaCollector.Add(XmlSchemaUtil.CreateRefGroup(_fileName, e));
    }
}