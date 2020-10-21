using Luban.Common.Utils;
using Luban.Config.Common.RawDefs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Luban.Job.Cfg.Defs
{
    class CfgDefLoader : CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<Table> _cfgTables = new List<Table>();

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly List<Group> _cfgGroups = new List<Group>();

        private readonly List<string> _defaultGroups = new List<string>();

        public CfgDefLoader(RemoteAgent agent) : base(agent)
        {
            RegisterRootDefineHandler("service", AddService);
            RegisterRootDefineHandler("group", AddGroup);

            RegisterModuleDefineHandler("table", AddTable);

            IsBeanFieldMustDefineId = false;
        }

        public Defines BuildDefines()
        {
            return new Defines()
            {
                TopModule = TopModule,
                Consts = this._consts,
                Enums = _enums,
                Beans = _beans,
                Tables = _cfgTables,
                Services = _cfgServices,
                Groups = _cfgGroups,
            };
        }


        private static readonly List<string> _groupOptionalAttrs = new List<string> { "default" };
        private static readonly List<string> _groupRequireAttrs = new List<string> { "name" };

        private void AddGroup(XElement e)
        {
            ValidAttrKeys(e, _groupOptionalAttrs, _groupRequireAttrs);
            List<string> groupNames = CreateGroups(e.Attribute("name").Value);

            foreach (var g in groupNames)
            {
                if (_cfgGroups.Any(cg => cg.Names.Contains(g)))
                {
                    throw new Exception($"group名:{g} 重复");
                }
            }

            if (XmlUtil.GetOptionBoolAttribute(e, "default"))
            {
                this._defaultGroups.AddRange(groupNames);
            }
            _cfgGroups.Add(new Group() { Names = groupNames });
        }

        private void FillValueValidator(CfgField f, XAttribute e, string validatorName)
        {
            if (e != null)
            {
                var validator = new Validator() { Type = validatorName, Rule = e.Value };
                f.Validators.Add(validator);
                f.ValueValidators.Add(validator);
            }
        }

        private void FillValidators(XElement e, string key, List<Validator> result)
        {
            var attr = e.Attribute(key);
            if (attr != null)
            {
                foreach (var validatorStr in attr.Value.Split('#', StringSplitOptions.RemoveEmptyEntries))
                {
                    var sepIndex = validatorStr.IndexOf(':');
                    if (sepIndex < 0)
                    {
                        throw new Exception($"定义文件:{CurImportFile} 类型:{CurDefine} 字段:{e} key:{key} 不是合法的 validator 定义 (key1:value1#key2:value2 ...)");
                    }
                    result.Add(new Validator() { Type = validatorStr[..sepIndex], Rule = validatorStr[(sepIndex + 1)..] });
                }
            }
        }

        private readonly List<string> _serviceAttrs = new List<string> { "name", "manager", "group" };

        private void AddService(XElement e)
        {
            var name = XmlUtil.GetRequiredAttribute(e, "name");
            var manager = XmlUtil.GetRequiredAttribute(e, "manager");
            List<string> groups = CreateGroups(XmlUtil.GetOptionalAttribute(e, "group"));
            var refs = new List<string>();

            s_logger.Trace("service name:{name} manager:{manager}", name, manager);
            ValidAttrKeys(e, _serviceAttrs, _serviceAttrs);
            foreach (XElement ele in e.Elements())
            {
                string tagName = ele.Name.LocalName;
                s_logger.Trace("service {service_name} tag: {name} {value}", name, tagName, ele);
                switch (tagName)
                {
                    case "ref":
                    {
                        refs.Add(XmlUtil.GetRequiredAttribute(ele, "name"));
                        break;
                    }
                    default:
                    {
                        throw new Exception($"service:{name} tag:{tagName} 非法");
                    }
                }
            }
            if (!ValidGroup(groups, out var invalidGroup))
            {
                throw new Exception($"service:{name} group:{invalidGroup} 不存在");
            }
            _cfgServices.Add(new Service() { Name = name, Manager = manager, Groups = groups, Refs = refs });
        }

        private readonly List<string> _tableOptionalAttrs = new List<string> { "index", "mode", "group" };
        private readonly List<string> _tableRequireAttrs = new List<string> { "name", "value", "input" };


        private readonly Dictionary<string, Table> _name2CfgTable = new Dictionary<string, Table>();

        private static List<string> CreateGroups(string s)
        {
            return s.Split(',', ';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private bool ValidGroup(List<string> groups, out string invalidGroup)
        {
            foreach (var g in groups)
            {
                if (!this._cfgGroups.Any(cg => cg.Names.Contains(g)))
                {
                    invalidGroup = g;
                    return false;
                }
            }
            invalidGroup = null;
            return true;
        }

        private ETableMode ConvertMode(string tableName, string modeStr, string indexStr)
        {
            ETableMode mode;
            switch (modeStr)
            {
                case "one":
                {
                    if (!string.IsNullOrWhiteSpace(indexStr))
                    {
                        throw new Exception($"定义文件:{CurImportFile} table:{tableName} mode=one 是单例表，不支持定义index属性");
                    }
                    mode = ETableMode.ONE;
                    break;
                }
                case "map":
                {
                    if ((string.IsNullOrWhiteSpace(indexStr) || indexStr.Split(',').Length != 1))
                    {
                        throw new Exception($"定义文件:{CurImportFile} table:{tableName} 是单键表，必须在index属性里指定1个key");
                    }
                    mode = ETableMode.MAP;
                    break;
                }
                case "bmap":
                {
                    if ((string.IsNullOrWhiteSpace(indexStr) || indexStr.Split(',').Length != 2))
                    {
                        throw new Exception($"定义文件:{CurImportFile} table:{tableName} 是双键表，必须在index属性里指定2个key");
                    }
                    mode = ETableMode.BMAP;
                    break;
                }
                case "":
                {
                    // 当 mode 属性为空时, 智能根据 index 值推测表类型
                    // 如果index为空或一个键,则为 MAP类型
                    // 如果index为2个键，则为 BMAP类型
                    var indexs = indexStr.Split(',').Select(s => s.Trim()).ToList();
                    switch (indexs.Count)
                    {
                        case 0:
                        case 1: mode = ETableMode.MAP; break;
                        case 2: mode = ETableMode.BMAP; break;
                        default: throw new Exception($"定义文件:{CurImportFile} table:{tableName} 最多只能有两个 index");
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

        private void AddTable(XElement e)
        {
            ValidAttrKeys(e, _tableOptionalAttrs, _tableRequireAttrs);

            var p = new Table()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                ValueType = XmlUtil.GetRequiredAttribute(e, "value"),
                Index = XmlUtil.GetOptionalAttribute(e, "index"),
                Groups = CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
            };
            p.Mode = ConvertMode(p.Name, XmlUtil.GetOptionalAttribute(e, "mode"), p.Index);

            if (p.Groups.Count == 0)
            {
                p.Groups = this._defaultGroups;
            }
            else if (!ValidGroup(p.Groups, out var invalidGroup))
            {
                throw new Exception($"定义文件:{CurImportFile} table:{p.Name} group:{invalidGroup} 不存在");
            }
            p.InputFiles.AddRange(XmlUtil.GetRequiredAttribute(e, "input").Split(','));

            if (!_name2CfgTable.TryAdd(p.Name, p))
            {
                var exist = _name2CfgTable[p.Name];
                throw new Exception($"定义文件:{CurImportFile} table:{p.Namespace}.{p.Name} 与 {exist.Namespace}.{exist.Name} 重复");
            }
            _cfgTables.Add(p);
        }



        private static readonly List<string> _fieldOptionalAttrs = new List<string> {
            "index", "sep", "validator", "key_validator", "value_validator",
            "ref", "path", "range", "multi_rows", "group", "res", "convert" };

        private static readonly List<string> _fieldRequireAttrs = new List<string> { "name", "type" };

        protected override Field CreateField(XElement e)
        {
            ValidAttrKeys(e, _fieldOptionalAttrs, _fieldRequireAttrs);
            var f = new CfgField()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Index = XmlUtil.GetOptionalAttribute(e, "index"),
                Sep = XmlUtil.GetOptionalAttribute(e, "sep"),
                IsMultiRow = XmlUtil.GetOptionBoolAttribute(e, "multi_rows"),
                Groups = CreateGroups(XmlUtil.GetOptionalAttribute(e, "group")),
                Resource = XmlUtil.GetOptionalAttribute(e, "res"),
                Converter = XmlUtil.GetOptionalAttribute(e, "convert"),
            };

            // 字段与table的默认组不一样。
            // table 默认只属于default=1的组
            // 字段默认属于所有组
            if (f.Groups.Count == 0)
            {

            }
            else if (!ValidGroup(f.Groups, out var invalidGroup))
            {
                throw new Exception($"定义文件:{CurImportFile} field:{e} group:{invalidGroup} 不存在");
            }
            f.Type = CreateType(e, "type");


            FillValueValidator(f, e.Attribute("ref"), "ref");
            FillValueValidator(f, e.Attribute("path"), "path"); // (ue4|normal|regex);xxx;xxx
            FillValueValidator(f, e.Attribute("range"), "range");

            FillValidators(e, "key_validator", f.KeyValidators);
            FillValidators(e, "value_validator", f.ValueValidators);
            FillValidators(e, "validator", f.Validators);
            return f;
        }

        private static readonly List<string> _beanOptinsAttrs = new List<string> { "value_type", "alias", "sep" };
        private static readonly List<string> _beanRequireAttrs = new List<string> { "name" };

        protected override void AddBean(XElement e, string parent)
        {
            ValidAttrKeys(e, _beanOptinsAttrs, _beanRequireAttrs);

            var b = new CfgBean()
            {
                Name = XmlUtil.GetRequiredAttribute(e, "name"),
                Namespace = CurNamespace,
                Parent = parent.Length > 0 ? parent : "",
                TypeId = 0,
                IsSerializeCompatible = true,
                IsValueType = XmlUtil.GetOptionBoolAttribute(e, "value_type"),
                Alias = XmlUtil.GetOptionalAttribute(e, "alias"),
                Sep = XmlUtil.GetOptionalAttribute(e, "sep"),
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
                            throw new LoadDefException($"定义文件:{CurImportFile} 类型:{b.FullName} 的多态子bean必须在所有成员字段 <var> 之前定义");
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
                        throw new LoadDefException($"定义文件:{CurImportFile} 类型:{b.FullName} 不支持 tag:{fe.Name}");
                    }
                }
            }
            s_logger.Trace("add bean:{@bean}", b);
            _beans.Add(b);

            var fullname = b.FullName;
            foreach (var cb in childBeans)
            {
                AddBean(cb, fullname);
            }
        }
    }
}
