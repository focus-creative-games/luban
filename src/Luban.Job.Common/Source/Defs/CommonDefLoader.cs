using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luban.Job.Common.Defs
{

    public abstract class CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        protected IAgent Agent { get; }

        public string RootDir { get; private set; }

        public bool IsBeanDefaultCompatible { get; protected set; }

        public bool IsBeanFieldMustDefineId { get; protected set; }

        private readonly Dictionary<string, Action<XElement>> _rootDefineHandlers = new Dictionary<string, Action<XElement>>();
        private readonly Dictionary<string, Action<string, XElement>> _moduleDefineHandlers = new();

        protected readonly Stack<string> _namespaceStack = new Stack<string>();

        protected string TopModule { get; private set; }

        protected readonly List<PEnum> _enums = new List<PEnum>();
        protected readonly List<Bean> _beans = new List<Bean>();
        protected readonly HashSet<string> _externalSelectors = new();
        protected readonly Dictionary<string, ExternalType> _externalTypes = new();

        protected readonly Dictionary<string, string> _options = new();

        protected CommonDefLoader(IAgent agent)
        {
            Agent = agent;

            _rootDefineHandlers.Add("topmodule", SetTopModule);
            _rootDefineHandlers.Add("option", AddOption);
            _rootDefineHandlers.Add("externalselector", AddExternalSelector);

            _moduleDefineHandlers.Add("module", AddModule);
            _moduleDefineHandlers.Add("enum", AddEnum);
            _moduleDefineHandlers.Add("bean", AddBean);
            _moduleDefineHandlers.Add("externaltype", AddExternalType);
        }

        public string RootXml => _rootXml;
        private string _rootXml;

        public async Task LoadAsync(string rootXml)
        {
            _rootXml = rootXml;

            RootDir = FileUtil.GetParent(rootXml);

            XElement doc = await Agent.OpenXmlAsync(rootXml);

            foreach (XElement e in doc.Elements())
            {
                var tagName = e.Name.LocalName;
                if (tagName == "import")
                {
                    await AddImportAsync(XmlUtil.GetRequiredAttribute(e, "name"));
                    continue;
                }
                if (_rootDefineHandlers.TryGetValue(tagName, out var handler))
                {
                    handler(e);
                }
                else
                {
                    throw new LoadDefException($"定义文件:{rootXml} 非法 tag:{tagName}");
                }
            }
        }

        protected void RegisterRootDefineHandler(string name, Action<XElement> handler)
        {
            _rootDefineHandlers.Add(name, handler);
        }

        protected void RegisterModuleDefineHandler(string name, Action<string, XElement> handler)
        {
            _moduleDefineHandlers.Add(name, handler);
        }

        protected string CurNamespace => _namespaceStack.Count > 0 ? _namespaceStack.Peek() : "";

        protected void BuildCommonDefines(DefinesCommon defines)
        {
            defines.TopModule = TopModule;
            defines.Enums = _enums;
            defines.Beans = _beans;
            defines.ExternalSelectors = _externalSelectors;
            defines.ExternalTypes = _externalTypes;
            defines.Options = _options;
        }

        #region root handler

        private void SetTopModule(XElement e)
        {
            this.TopModule = XmlUtil.GetOptionalAttribute(e, "name");
        }

        private static readonly List<string> _optionRequireAttrs = new List<string> { "name", "value", };

        private void AddOption(XElement e)
        {
            ValidAttrKeys(_rootXml, e, null, _optionRequireAttrs);
            string name = XmlUtil.GetRequiredAttribute(e, "name");
            if (!_options.TryAdd(name, XmlUtil.GetRequiredAttribute(e, "value")))
            {
                throw new LoadDefException($"option name:'{name}' duplicate");
            }
        }

        private async Task AddImportAsync(string xmlFile)
        {
            var rootFileName = FileUtil.GetFileName(_rootXml);

            var xmlFullPath = FileUtil.Combine(RootDir, xmlFile);
            s_logger.Trace("import {file} {full_path}", xmlFile, xmlFullPath);

            var fileOrDirContent = await Agent.GetFileOrDirectoryAsync(xmlFullPath, ".xml");

            if (fileOrDirContent.IsFile)
            {
                s_logger.Trace("== file:{file}", xmlFullPath);
                AddModule(xmlFullPath, XmlUtil.Open(xmlFullPath, await Agent.GetFromCacheOrReadAllBytesAsync(xmlFullPath, fileOrDirContent.Md5)));
            }
            else
            {
                // 如果是目录,则递归导入目录下的所有 .xml 定义文件
                foreach (var subFile in fileOrDirContent.SubFiles)
                {
                    var subFileName = subFile.FilePath;
                    s_logger.Trace("sub import xmlfile:{file} root file:{root}", subFileName, rootFileName);
                    // 有时候 root 定义文件会跟 module定义文件放在一个目录. 当以目录形式导入子module时，不希望导入它
                    if (FileUtil.GetFileName(subFileName) == rootFileName)
                    {
                        s_logger.Trace("ignore import root file:{root}", subFileName);
                        continue;
                    }
                    string subFullPath = subFileName;
                    AddModule(subFullPath, XmlUtil.Open(subFullPath, await Agent.GetFromCacheOrReadAllBytesAsync(subFullPath, subFile.MD5)));
                }
            }
        }

        #endregion


        #region module handler

        private void AddModule(string defineFile, XElement me)
        {
            var name = XmlUtil.GetOptionalAttribute(me, "name")?.Trim();
            //if (string.IsNullOrEmpty(name))
            //{
            //    throw new LoadDefException($"xml:{CurImportFile} contains module which's name is empty");
            //}

            _namespaceStack.Push(_namespaceStack.Count > 0 ? TypeUtil.MakeFullName(_namespaceStack.Peek(), name) : name);

            // 加载所有module定义,允许嵌套
            foreach (XElement e in me.Elements())
            {
                var tagName = e.Name.LocalName;
                if (_moduleDefineHandlers.TryGetValue(tagName, out var handler))
                {
                    if (tagName != "module")
                    {
                        handler(defineFile, e);
                    }
                    else
                    {
                        handler(defineFile, e);
                    }
                }
                else
                {
                    throw new LoadDefException($"定义文件:{defineFile} module:{CurNamespace} 不支持 tag:{tagName}");
                }
            }
            _namespaceStack.Pop();
        }

        private static readonly List<string> _fieldRequireAttrs = new List<string> { "name", "type", };
        private static readonly List<string> _fieldOptionalAttrs = new List<string> { "id", "comment", "tags" };

        protected virtual Field CreateField(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _fieldOptionalAttrs, _fieldRequireAttrs);
            var f = new Field()
            {
                Id = XmlUtil.GetOptionIntAttribute(e, "id"),
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Type = CreateType(e, "type"),
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
                Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
            };
            return f;
        }

        protected void AddBean(string defineFile, XElement e)
        {
            AddBean(defineFile, e, "");
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

        protected virtual void AddBean(string defineFile, XElement e, string parent)
        {

            if (IsBeanFieldMustDefineId)
            {
                ValidAttrKeys(defineFile, e, _beanOptinsAttrs1, _beanRequireAttrs1);
            }
            else
            {
                ValidAttrKeys(defineFile, e, _beanOptinsAttrs2, _beanRequireAttrs2);
            }
            TryGetUpdateParent(e, ref parent);

            var b = new Bean()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name").Trim(),
                Namespace = CurNamespace,
                Parent = parent,
                TypeId = XmlUtil.GetOptionIntAttribute(e, "id"),
                IsSerializeCompatible = XmlUtil.GetOptionBoolAttribute(e, "compatible", IsBeanDefaultCompatible),
                IsValueType = XmlUtil.GetOptionBoolAttribute(e, "value_type"),
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
                Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
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
                            throw new LoadDefException($"定义文件:{defineFile} 类型:{b.FullName} 的多态子bean必须在所有成员字段 <var> 之前定义");
                        }
                        b.Fields.Add(CreateField(defineFile, fe)); ;
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
                        throw new LoadDefException($"定义文件:{defineFile} 类型:{b.FullName} 不支持 tag:{fe.Name}");
                    }
                }
            }
            s_logger.Trace("add bean:{@bean}", b);
            _beans.Add(b);

            var fullname = b.FullName;
            foreach (var cb in childBeans)
            {
                AddBean(defineFile, cb, fullname);
            }
        }

        protected static string CreateType(XElement e, string key)
        {
            return XmlUtil.GetRequiredAttribute(e, key);
        }

        protected void ValidAttrKeys(string defineFile, XElement e, List<string> optionKeys, List<string> requireKeys)
        {
            foreach (var k in e.Attributes())
            {
                var name = k.Name.LocalName;
                if (!requireKeys.Contains(name) && (optionKeys != null && !optionKeys.Contains(name)))
                {
                    throw new LoadDefException($"定义文件:{defineFile} module:{CurNamespace} 定义:{e} 包含未知属性 attr:{name}");
                }
            }
            foreach (var k in requireKeys)
            {
                if (e.Attribute(k) == null)
                {
                    throw new LoadDefException($"定义文件:{defineFile} module:{CurNamespace} 定义:{e} 缺失属性 attr:{k}");
                }
            }
        }

        private static readonly List<string> _enumOptionalAttrs = new List<string> { "flags", "comment", "tags", "unique" };
        private static readonly List<string> _enumRequiredAttrs = new List<string> { "name" };


        private static readonly List<string> _enumItemOptionalAttrs = new List<string> { "value", "alias", "comment", "tags" };
        private static readonly List<string> _enumItemRequiredAttrs = new List<string> { "name" };

        protected void AddEnum(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _enumOptionalAttrs, _enumRequiredAttrs);
            var en = new PEnum()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name").Trim(),
                Namespace = CurNamespace,
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
                IsFlags = XmlUtil.GetOptionBoolAttribute(e, "flags"),
                Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
                IsUniqueItemId = XmlUtil.GetOptionBoolAttribute(e, "unique", true),
            };

            foreach (XElement item in e.Elements())
            {
                ValidAttrKeys(defineFile, item, _enumItemOptionalAttrs, _enumItemRequiredAttrs);
                en.Items.Add(new EnumItem()
                {
                    Name = XmlUtil.GetRequiredAttribute(item, "name"),
                    Alias = XmlUtil.GetOptionalAttribute(item, "alias"),
                    Value = XmlUtil.GetOptionalAttribute(item, "value"),
                    Comment = XmlUtil.GetOptionalAttribute(item, "comment"),
                    Tags = XmlUtil.GetOptionalAttribute(item, "tags"),
                });
            }
            s_logger.Trace("add enum:{@enum}", en);
            _enums.Add(en);
        }

        private static readonly List<string> _selectorRequiredAttrs = new List<string> { "name" };
        private void AddExternalSelector(XElement e)
        {
            ValidAttrKeys(_rootXml, e, null, _selectorRequiredAttrs);
            string name = XmlUtil.GetRequiredAttribute(e, "name");
            if (!_externalSelectors.Add(name))
            {
                throw new LoadDefException($"定义文件:{_rootXml} externalselector name:{name} 重复");
            }
            s_logger.Trace("add selector:{}", name);
        }

        private static readonly List<string> _externalRequiredAttrs = new List<string> { "name", "origin_type_name" };
        private void AddExternalType(string defineFile, XElement e)
        {
            ValidAttrKeys(_rootXml, e, null, _externalRequiredAttrs);
            string name = XmlUtil.GetRequiredAttribute(e, "name");

            if (_externalTypes.ContainsKey(name))
            {
                throw new LoadDefException($"定义文件:{_rootXml} externaltype:{name} 重复");
            }

            var et = new ExternalType()
            {
                Name = name,
                OriginTypeName = XmlUtil.GetRequiredAttribute(e, "origin_type_name"),
            };
            var mappers = new Dictionary<string, ExternalTypeMapper>();
            foreach (XElement mapperEle in e.Elements())
            {
                var tagName = mapperEle.Name.LocalName;
                if (tagName == "mapper")
                {
                    var mapper = CreateMapper(defineFile, name, mapperEle);
                    string uniqKey = $"{mapper.Lan}##{mapper.Selector}";
                    if (mappers.ContainsKey(uniqKey))
                    {
                        throw new LoadDefException($"定义文件:{_rootXml} externaltype name:{name} mapper(lan='{mapper.Lan}',selector='{mapper.Selector}') 重复");
                    }
                    mappers.Add(uniqKey, mapper);
                    et.Mappers.Add(mapper);
                    s_logger.Trace("add mapper. externaltype:{} mapper:{@}", name, mapper);
                }
                else
                {
                    throw new LoadDefException($"定义文件:{defineFile} externaltype:{name} 非法 tag:'{tagName}'");
                }
            }
            _externalTypes.Add(name, et);
        }

        private static readonly List<string> _mapperOptionalAttrs = new List<string> { };
        private static readonly List<string> _mapperRequiredAttrs = new List<string> { "lan", "selector" };
        private ExternalTypeMapper CreateMapper(string defineFile, string externalType, XElement e)
        {
            ValidAttrKeys(_rootXml, e, _mapperOptionalAttrs, _mapperRequiredAttrs);
            var m = new ExternalTypeMapper()
            {
                Lan = DefUtil.ParseLanguage(XmlUtil.GetRequiredAttribute(e, "lan")),
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
                    case "create_external_object_function":
                    {
                        m.CreateExternalObjectFunction = attrEle.Value;
                        break;
                    }
                    default: throw new LoadDefException($"定义文件:{defineFile} externaltype:{externalType} 非法 tag:{tagName}");
                }
            }
            if (string.IsNullOrWhiteSpace(m.TargetTypeName))
            {
                throw new LoadDefException($"定义文件:{defineFile} externaltype:{externalType} lan:{m.Lan} selector:{m.Selector} 没有定义 'target_type_name'");
            }
            return m;
        }
        #endregion
    }
}
