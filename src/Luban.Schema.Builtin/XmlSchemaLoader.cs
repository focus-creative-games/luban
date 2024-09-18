using System.Xml.Linq;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaLoader("", "xml")]
public class XmlSchemaLoader : SchemaLoaderBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Dictionary<string, Action<XElement>> _tagHandlers = new();

    private string _fileName;

    private readonly Stack<string> _namespaceStack = new();

    private string CurNamespace => _namespaceStack.TryPeek(out var ns) ? ns : "";

    public XmlSchemaLoader()
    {
        _tagHandlers.Add("module", AddModule);
        _tagHandlers.Add("enum", AddEnum);
        _tagHandlers.Add("bean", AddBean);
        _tagHandlers.Add("table", AddTable);
        _tagHandlers.Add("refgroup", AddRefGroup);
    }

    public override void Load(string fileName)
    {
        _fileName = fileName;
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

    private static readonly List<string> _enumOptionalAttrs = new() { "flags", "comment", "tags", "unique", "group" };
    private static readonly List<string> _enumRequiredAttrs = new() { "name" };

    private static readonly List<string> _enumItemOptionalAttrs = new() { "value", "alias", "comment", "tags" };
    private static readonly List<string> _enumItemRequiredAttrs = new() { "name" };

    private void AddEnum(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _enumOptionalAttrs, _enumRequiredAttrs);
        var en = new RawEnum()
        {
            Name = XmlUtil.GetRequiredAttribute(e, "name").Trim(),
            Namespace = CurNamespace,
            Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            IsFlags = XmlUtil.GetOptionBoolAttribute(e, "flags"),
            Tags = DefUtil.ParseAttrs(XmlUtil.GetOptionalAttribute(e, "tags")),
            IsUniqueItemId = XmlUtil.GetOptionBoolAttribute(e, "unique", true),
            Groups = SchemaLoaderUtil.CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
            Items = new(),
            TypeMappers = new(),
        };

        foreach (XElement item in e.Elements())
        {
            switch (item.Name.LocalName)
            {
                case "var":
                {
                    XmlSchemaUtil.ValidAttrKeys(_fileName, item, _enumItemOptionalAttrs, _enumItemRequiredAttrs);
                    en.Items.Add(new EnumItem()
                    {
                        Name = XmlUtil.GetRequiredAttribute(item, "name").Trim(),
                        Alias = XmlUtil.GetOptionalAttribute(item, "alias"),
                        Value = XmlUtil.GetOptionalAttribute(item, "value"),
                        Comment = XmlUtil.GetOptionalAttribute(item, "comment"),
                        Tags = DefUtil.ParseAttrs(XmlUtil.GetOptionalAttribute(item, "tags")),
                    });
                    break;
                }
                case "mapper":
                {
                    en.TypeMappers.Add(CreateTypeMapper(item, en.FullName));
                    break;
                }
                default:
                {
                    throw new Exception($"不支持的enum子节点:{item.Name.LocalName}");
                }
            }
        }
        s_logger.Trace("add enum:{@}", en);
        Collector.Add(en);
    }

    private readonly List<string> _tableOptionalAttrs = new() { "index", "mode", "group", "comment", "readSchemaFromFile", "output", "tags" };
    private readonly List<string> _tableRequireAttrs = new() { "name", "value", "input" };

    private void AddTable(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _tableOptionalAttrs, _tableRequireAttrs);
        string name = XmlUtil.GetRequiredAttribute(e, "name");
        string module = CurNamespace;
        string valueType = XmlUtil.GetRequiredAttribute(e, "value");
        bool defineFromFile = XmlUtil.GetOptionBoolAttribute(e, "readSchemaFromFile");
        if (string.IsNullOrEmpty(TypeUtil.GetNamespace(valueType)))
        {
            valueType = TypeUtil.MakeFullName(module, valueType);
        }
        string index = XmlUtil.GetOptionalAttribute(e, "index");
        string group = XmlUtil.GetOptionalAttribute(e, "group");
        string comment = XmlUtil.GetOptionalAttribute(e, "comment");
        string input = XmlUtil.GetRequiredAttribute(e, "input");
        string mode = XmlUtil.GetOptionalAttribute(e, "mode");
        string tags = XmlUtil.GetOptionalAttribute(e, "tags");
        string output = XmlUtil.GetOptionalAttribute(e, "output");
        Collector.Add(SchemaLoaderUtil.CreateTable(_fileName, name, module, valueType, index, mode, group, comment, defineFromFile, input, tags, output));
    }

    private static readonly List<string> _fieldOptionalAttrs = new()
    {
        // "ref",
        // "path",
        "alias",
        "group",
        "comment",
        "tags",
        "variants",
    };

    private static readonly List<string> _fieldRequireAttrs = new() { "name", "type" };

    protected RawField CreateField(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _fieldOptionalAttrs, _fieldRequireAttrs);

        string typeStr = XmlUtil.GetRequiredAttribute(e, "type");

        // string refStr = XmlUtil.GetOptionalAttribute(e, "ref");
        // if (!string.IsNullOrWhiteSpace(refStr))
        // {
        //     typeStr = typeStr + "#(ref=" + refStr + ")";
        // }
        // string pathStr = XmlUtil.GetOptionalAttribute(e, "path");
        // if (!string.IsNullOrWhiteSpace(pathStr))
        // {
        //     typeStr = typeStr + "#(path=" + pathStr + ")";
        // }

        return SchemaLoaderUtil.CreateField(_fileName,
            XmlUtil.GetRequiredAttribute(e, "name"),
            XmlUtil.GetOptionalAttribute(e, "alias"),
            typeStr,
            XmlUtil.GetOptionalAttribute(e, "group"),
            XmlUtil.GetOptionalAttribute(e, "comment"),
            XmlUtil.GetOptionalAttribute(e, "tags"),
            XmlUtil.GetOptionalAttribute(e, "variants"),
            false
        );
    }

    private static readonly List<string> _beanOptionsAttrs = new() { "parent", "valueType", "alias", "sep", "comment", "tags", "group" };
    private static readonly List<string> _beanRequireAttrs = new() { "name" };

    private TypeMapper CreateTypeMapper(XElement e, string fullName)
    {
        var opts = new Dictionary<string, string>();
        foreach (XElement optionEle in e.Elements())
        {
            string key = XmlUtil.GetRequiredAttribute(optionEle, "name");
            string value = XmlUtil.GetRequiredAttribute(optionEle, "value");
            if (!opts.TryAdd(key, value))
            {
                throw new Exception($"CreateTypeMapper {fullName} option:{key} 重复定义");
            }
        }
        var mapper = new TypeMapper()
        {
            Targets = XmlUtil.GetRequiredAttribute(e, "target").Split(',', ';', '|').ToList(),
            CodeTargets = XmlUtil.GetRequiredAttribute(e, "codeTarget").Split(',', ';', '|').ToList(),
            Options = opts,
        };
        return mapper;
    }


    protected void AddBean(XElement e)
    {
        AddBean(e, "");
    }

    protected void AddBean(XElement e, string parent)
    {
        XmlSchemaUtil.ValidAttrKeys(_fileName, e, _beanOptionsAttrs, _beanRequireAttrs);
        TryGetUpdateParent(e, ref parent);
        var b = new RawBean()
        {
            Name = XmlUtil.GetRequiredAttribute(e, "name"),
            Namespace = CurNamespace,
            Parent = parent,
            IsValueType = XmlUtil.GetOptionBoolAttribute(e, "valueType"),
            Alias = XmlUtil.GetOptionalAttribute(e, "alias"),
            Sep = XmlUtil.GetOptionalAttribute(e, "sep"),
            Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            Tags = DefUtil.ParseAttrs(XmlUtil.GetOptionalAttribute(e, "tags")),
            Groups = SchemaLoaderUtil.CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
            Fields = new(),
            TypeMappers = new(),
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
                    b.Fields.Add(CreateField(fe));
                    ;
                    break;
                }
                case "mapper":
                {
                    b.TypeMappers.Add(CreateTypeMapper(fe, b.FullName));
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
        Collector.Add(b);

        var fullname = b.FullName;
        foreach (var cb in childBeans)
        {
            AddBean(cb, fullname);
        }
    }

    private void AddRefGroup(XElement e)
    {
        Collector.Add(XmlSchemaUtil.CreateRefGroup(_fileName, e));
    }
}
