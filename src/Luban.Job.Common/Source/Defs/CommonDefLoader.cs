using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luban.Job.Common.Defs
{
    public class LoadDefException : Exception
    {
        public LoadDefException()
        {
        }

        public LoadDefException(string message) : base(message)
        {
        }

        public LoadDefException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LoadDefException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public abstract class CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        protected RemoteAgent Agent { get; }

        public string RootDir { get; private set; }

        public bool IsBeanDefaultCompatible { get; protected set; }

        public bool IsBeanFieldMustDefineId { get; protected set; }

        private readonly Dictionary<string, Action<XElement>> _rootDefineHandlers = new Dictionary<string, Action<XElement>>();
        private readonly Dictionary<string, Action<string, XElement>> _moduleDefineHandlers = new();

        protected readonly Stack<string> _namespaceStack = new Stack<string>();

        protected string TopModule { get; private set; }

        protected readonly List<Const> _consts = new List<Const>();
        protected readonly List<PEnum> _enums = new List<PEnum>();
        protected readonly List<Bean> _beans = new List<Bean>();

        protected CommonDefLoader(RemoteAgent agent)
        {
            Agent = agent;

            _rootDefineHandlers.Add("topmodule", SetTopModule);

            _moduleDefineHandlers.Add("module", AddModule);
            _moduleDefineHandlers.Add("const", AddConst);
            _moduleDefineHandlers.Add("enum", AddEnum);
            _moduleDefineHandlers.Add("bean", AddBean);
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

        #region root handler

        private void SetTopModule(XElement e)
        {
            this.TopModule = XmlUtil.GetRequiredAttribute(e, "name");
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
            var name = XmlUtil.GetOptionalAttribute(me, "name");
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

        private static readonly List<string> _beanOptinsAttrs1 = new List<string> { "compatible", "value_type", "comment", "tags" };
        private static readonly List<string> _beanRequireAttrs1 = new List<string> { "id", "name" };

        private static readonly List<string> _beanOptinsAttrs2 = new List<string> { "id", "compatible", "value_type", "comment", "tags" };
        private static readonly List<string> _beanRequireAttrs2 = new List<string> { "name" };

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
            var b = new Bean()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                Parent = parent.Length > 0 ? parent : "",
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


        private static readonly List<string> _constRequiredAttrs = new List<string> { "name" };
        private static readonly List<string> _constOptionalAttrs = new List<string> { "comment" };

        private static readonly List<string> _constItemRequiredAttrs = new List<string> { "name", "type" };
        private static readonly List<string> _constItemOptionalAttrs = new List<string> { "value", "comment" };

        protected void AddConst(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _constOptionalAttrs, _constRequiredAttrs);
            var c = new Const()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
            };
            foreach (XElement item in e.Elements())
            {
                ValidAttrKeys(defineFile, item, _constItemOptionalAttrs, _constItemRequiredAttrs);
                c.Items.Add(new ConstItem()
                {
                    Name = XmlUtil.GetRequiredAttribute(item, "name"),
                    Type = CreateType(item, "type"),
                    Value = XmlUtil.GetRequiredAttribute(item, "value"),
                    Comment = XmlUtil.GetOptionalAttribute(item, "comment"),
                });
            }
            s_logger.Trace("add const {@const}", c);
            _consts.Add(c);
        }

        private static readonly List<string> _enumOptionalAttrs = new List<string> { "flags", "comment", "tags" };
        private static readonly List<string> _enumRequiredAttrs = new List<string> { "name" };


        private static readonly List<string> _enumItemOptionalAttrs = new List<string> { "value", "alias", "comment", "tags" };
        private static readonly List<string> _enumItemRequiredAttrs = new List<string> { "name" };

        protected void AddEnum(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _enumOptionalAttrs, _enumRequiredAttrs);
            var en = new PEnum()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                Comment = XmlUtil.GetOptionalAttribute(e, "comment"),
                IsFlags = XmlUtil.GetOptionBoolAttribute(e, "flags"),
                Tags = XmlUtil.GetOptionalAttribute(e, "tags"),
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
        #endregion
    }
}
