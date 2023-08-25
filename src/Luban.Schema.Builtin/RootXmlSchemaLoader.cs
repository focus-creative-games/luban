using System.Xml.Linq;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaLoader("root", "xml")]
public class RootXmlSchemaLoader : SchemaLoaderBase, IRootSchemaLoader
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    private readonly Dictionary<string, Action<XElement>> _tagHandlers = new();
    
    private readonly List<SchemaFileInfo> _importFiles = new();
    
    public IReadOnlyList<SchemaFileInfo> ImportFiles => _importFiles;

    private string _xmlFileName;
    private string _curDir;

    public RootXmlSchemaLoader()
    {
        _tagHandlers.Add("externalselector", AddExternalSelector);
        _tagHandlers.Add("import", AddImport);
        _tagHandlers.Add("target", AddTarget);
        _tagHandlers.Add("group", AddGroup);
        _tagHandlers.Add("refgroup", AddRefGroup);
    }

    public override void Load(string fileName)
    {
        s_logger.Debug("load root xml schema file:{}", fileName);
        _xmlFileName = fileName;
        _curDir = Directory.GetParent(fileName).FullName;
        XElement doc = XmlUtil.Open(fileName);

        foreach (XElement e in doc.Elements())
        {
            var tagName = e.Name.LocalName;
            if (_tagHandlers.TryGetValue(tagName, out var handler))
            {
                handler(e);
            }
            else
            {
                throw new LoadDefException($"定义文件:{fileName} 非法 tag:{tagName}");
            }
        }
    }
    
     private static readonly List<string> _importRequireAttrs = new() { "name" };
     private static readonly List<string> _importOptionalAttrs = new() { "type" };
     
    private void AddImport(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_xmlFileName, e, _importOptionalAttrs, _importRequireAttrs);
        var importName = XmlUtil.GetRequiredAttribute(e, "name");
        if (string.IsNullOrWhiteSpace(importName))
        {
            throw new Exception("import 属性name不能为空");
        }
        var type = XmlUtil.GetOptionalAttribute(e, "type");
        foreach (var subFile in FileUtil.GetFileOrDirectory(Path.Combine(_curDir, importName)))
        {
            // ignore root.xml self
            if (Path.GetFileName(subFile) != Path.GetFileName(_xmlFileName))
            {
                _importFiles.Add(new SchemaFileInfo(){ FileName = subFile, Type = type});
            }
        }
    }

    private static readonly List<string> _groupOptionalAttrs = new() { "default" };
    private static readonly List<string> _groupRequireAttrs = new() { "name" };

    private void AddGroup(XElement e)
    {
        XmlSchemaUtil.ValidAttrKeys(_xmlFileName, e, _groupOptionalAttrs, _groupRequireAttrs);
        List<string> groupNames = SchemaLoaderUtil.CreateGroups(XmlUtil.GetRequiredAttribute(e, "name"));
        bool isDefault = XmlUtil.GetOptionBoolAttribute(e, "default");
        Collector.Add(new RawGroup(){ Names = groupNames, IsDefault = isDefault});
    }

    private readonly List<string> _targetAttrs = new() { "name", "manager", "group", "topModule" };

    private void AddTarget(XElement e)
    {
        var name = XmlUtil.GetRequiredAttribute(e, "name");
        var manager = XmlUtil.GetRequiredAttribute(e, "manager");
        var topModule = XmlUtil.GetOptionalAttribute(e, "topModule");
        List<string> groups = SchemaLoaderUtil.CreateGroups(XmlUtil.GetOptionalAttribute(e, "group"));
        XmlSchemaUtil.ValidAttrKeys(_xmlFileName, e, _targetAttrs, _targetAttrs);
        Collector.Add(new RawTarget() { Name = name, Manager = manager, Groups = groups, TopModule = topModule});
    }

    private void AddRefGroup(XElement e)
    {
        Collector.Add(XmlSchemaUtil.CreateRefGroup(_xmlFileName, e));
    }
    
    private static readonly List<string> _selectorRequiredAttrs = new() { "name" };
    private void AddExternalSelector(XElement e)
    {
        // ValidAttrKeys(_rootXml, e, null, _selectorRequiredAttrs);
        // string name = XmlUtil.GetRequiredAttribute(e, "name");
        // if (!_externalSelectors.Add(name))
        // {
        //     throw new LoadDefException($"定义文件:{_rootXml} external selector name:{name} 重复");
        // }
        // s_logger.Trace("add selector:{}", name);
    }
}