using Bright.Collections;
using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luban.Job.Cfg.Defs
{
    class CfgDefLoader : CommonDefLoader
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<string> _importExcelTableFiles = new();
        private readonly List<string> _importExcelEnumFiles = new();
        private readonly List<string> _importExcelBeanFiles = new();


        private readonly List<Patch> _patches = new();

        private readonly List<Table> _cfgTables = new List<Table>();

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly List<Group> _cfgGroups = new List<Group>();

        private readonly List<string> _defaultGroups = new List<string>();

        public CfgDefLoader(IAgent agent) : base(agent)
        {
            RegisterRootDefineHandler("importexcel", AddImportExcel);
            RegisterRootDefineHandler("patch", AddPatch);
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
                Patches = _patches,
                Enums = _enums,
                Beans = _beans,
                Tables = _cfgTables,
                Services = _cfgServices,
                Groups = _cfgGroups,
            };
        }

        private static readonly List<string> _excelImportRequireAttrs = new List<string> { "name", "type" };
        private void AddImportExcel(XElement e)
        {
            ValidAttrKeys(RootXml, e, null, _excelImportRequireAttrs);
            var importName = XmlUtil.GetRequiredAttribute(e, "name");
            if (string.IsNullOrWhiteSpace(importName))
            {
                throw new Exception("importexcel 属性name不能为空");
            }
            var type = XmlUtil.GetRequiredAttribute(e, "type");
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new Exception($"importexcel name:'{importName}' type属性不能为空");
            }
            switch (type)
            {
                case "table": this._importExcelTableFiles.Add(importName); break;
                case "enum": this._importExcelEnumFiles.Add(importName); break;
                case "bean": this._importExcelBeanFiles.Add(importName); break;
                default: throw new Exception($"importexcel name:'{importName}' type:'{type}' 不合法. 有效值为 table|enum|bean");
            }
        }

        private static readonly List<string> _patchRequireAttrs = new List<string> { "name" };
        private void AddPatch(XElement e)
        {
            ValidAttrKeys(RootXml, e, null, _patchRequireAttrs);
            var patchName = e.Attribute("name").Value;
            if (string.IsNullOrWhiteSpace(patchName))
            {
                throw new Exception("patch 属性name不能为空");
            }
            if (this._patches.Any(b => b.Name == patchName))
            {
                throw new Exception($"patch '{patchName}' 重复");
            }
            _patches.Add(new Patch(patchName));
        }

        private static readonly List<string> _groupOptionalAttrs = new List<string> { "default" };
        private static readonly List<string> _groupRequireAttrs = new List<string> { "name" };

        private void AddGroup(XElement e)
        {
            ValidAttrKeys(RootXml, e, _groupOptionalAttrs, _groupRequireAttrs);
            List<string> groupNames = CreateGroups(e.Attribute("name").Value);

            foreach (var g in groupNames)
            {
                if (_cfgGroups.Any(cg => cg.Names.Contains(g)))
                {
                    throw new Exception($"group名:'{g}' 重复");
                }
            }

            if (XmlUtil.GetOptionBoolAttribute(e, "default"))
            {
                this._defaultGroups.AddRange(groupNames);
            }
            _cfgGroups.Add(new Group() { Names = groupNames });
        }

        private void FillValueValidator(CfgField f, string attrValue, string validatorName)
        {
            if (!string.IsNullOrWhiteSpace(attrValue))
            {
                var validator = new Validator() { Type = validatorName, Rule = attrValue };
                f.Validators.Add(validator);
                f.ValueValidators.Add(validator);
            }
        }

        private void FillValidators(string defineFile, string key, string attr, List<Validator> result)
        {
            if (!string.IsNullOrWhiteSpace(attr))
            {
#if !LUBAN_ASSISTANT
                foreach (var validatorStr in attr.Split('#', StringSplitOptions.RemoveEmptyEntries))
#else
                foreach (var validatorStr in attr.Split('#'))
#endif
                {
                    var sepIndex = validatorStr.IndexOf(':');
                    if (sepIndex <= 0)
                    {
                        throw new Exception($"定义文件:{defineFile} key:'{key}' attr:'{attr}' 不是合法的 validator 定义 (key1:value1#key2:value2 ...)");
                    }
#if !LUBAN_ASSISTANT
                    result.Add(new Validator() { Type = validatorStr[..sepIndex], Rule = validatorStr[(sepIndex + 1)..] });
#else
                    result.Add(new Validator() { Type = validatorStr.Substring(0, sepIndex), Rule = validatorStr.Substring(sepIndex + 1, validatorStr.Length - sepIndex - 1) });
#endif
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
            ValidAttrKeys(RootXml, e, _serviceAttrs, _serviceAttrs);
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
                        throw new Exception($"service:'{name}' tag:'{tagName}' 非法");
                    }
                }
            }
            if (!ValidGroup(groups, out var invalidGroup))
            {
                throw new Exception($"service:'{name}' group:'{invalidGroup}' 不存在");
            }
            _cfgServices.Add(new Service() { Name = name, Manager = manager, Groups = groups, Refs = refs });
        }


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

        private ETableMode ConvertMode(string defineFile, string tableName, string modeStr, string indexStr)
        {
            ETableMode mode;
            switch (modeStr)
            {
                case "one":
                {
                    if (!string.IsNullOrWhiteSpace(indexStr))
                    {
                        throw new Exception($"定义文件:{defineFile} table:'{tableName}' mode=one 是单例表，不支持定义index属性");
                    }
                    mode = ETableMode.ONE;
                    break;
                }
                case "map":
                {
                    //if ((string.IsNullOrWhiteSpace(indexStr) || indexStr.Split(',').Length != 1))
                    //{
                    //    throw new Exception($"定义文件:{CurImportFile} table:{tableName} 是单键表，必须在index属性里指定1个key");
                    //}
                    mode = ETableMode.MAP;
                    break;
                }
                case "":
                {
                    mode = ETableMode.MAP;
                    break;
                }
                default:
                {
                    throw new ArgumentException($"不支持的 mode:{modeStr}");
                }
            }
            return mode;
        }

        private readonly List<string> _tableOptionalAttrs = new List<string> { "index", "mode", "group", "patch_input", "comment", "define_from_file" };
        private readonly List<string> _tableRequireAttrs = new List<string> { "name", "value", "input" };

        private void AddTable(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _tableOptionalAttrs, _tableRequireAttrs);
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
            AddTable(defineFile, name, module, valueType, index, mode, group, comment, defineFromFile, input, patchInput, tags);
        }

        private void AddTable(string defineFile, string name, string module, string valueType, string index, string mode, string group,
            string comment, bool defineFromExcel, string input, string patchInput, string tags)
        {
            var p = new Table()
            {
                Name = name,
                Namespace = module,
                ValueType = valueType,
                LoadDefineFromFile = defineFromExcel,
                Index = index,
                Groups = CreateGroups(group),
                Comment = comment,
                Mode = ConvertMode(defineFile, name, mode, index),
                Tags = tags,
            };

            if (p.Groups.Count == 0)
            {
                p.Groups = this._defaultGroups;
            }
            else if (!ValidGroup(p.Groups, out var invalidGroup))
            {
                throw new Exception($"定义文件:{defineFile} table:'{p.Name}' group:'{invalidGroup}' 不存在");
            }
            p.InputFiles.AddRange(input.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)));

            if (!string.IsNullOrWhiteSpace(patchInput))
            {
                foreach (var subPatchStr in patchInput.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    var nameAndDirs = subPatchStr.Split(':');
                    if (nameAndDirs.Length != 2)
                    {
                        throw new Exception($"定义文件:{defineFile} table:'{p.Name}' patch_input:'{subPatchStr}' 定义不合法");
                    }
                    var patchDirs = nameAndDirs[1].Split(',', ';').ToList();
                    if (!p.PatchInputFiles.TryAdd(nameAndDirs[0], patchDirs))
                    {
                        throw new Exception($"定义文件:{defineFile} table:'{p.Name}' patch_input:'{subPatchStr}' 子patch:'{nameAndDirs[0]}' 重复");
                    }
                }
            }

            _cfgTables.Add(p);
        }


        private static readonly List<string> _fieldOptionalAttrs = new()
        {
            "index",
            "sep",
            "validator",
            "key_validator",
            "value_validator",
            "ref",
            "path",
            "range",
            "multi_rows",
            "group",
            "res",
            "convert",
            "comment",
            "tags",
            "default",
            "orientation",
        };

        private static readonly List<string> _fieldRequireAttrs = new List<string> { "name", "type" };

        protected override Field CreateField(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _fieldOptionalAttrs, _fieldRequireAttrs);

            return CreateField(defineFile, XmlUtil.GetRequiredAttribute(e, "name"),
                XmlUtil.GetRequiredAttribute(e, "type"),
                XmlUtil.GetOptionalAttribute(e, "index"),
                 XmlUtil.GetOptionalAttribute(e, "sep"),
                 XmlUtil.GetOptionBoolAttribute(e, "multi_rows"),
                 XmlUtil.GetOptionalAttribute(e, "group"),
                 XmlUtil.GetOptionalAttribute(e, "res"),
                 XmlUtil.GetOptionalAttribute(e, "convert"),
                 XmlUtil.GetOptionalAttribute(e, "comment"),
                 XmlUtil.GetOptionalAttribute(e, "ref"),
                 XmlUtil.GetOptionalAttribute(e, "path"),
                 XmlUtil.GetOptionalAttribute(e, "range"),
                 XmlUtil.GetOptionalAttribute(e, "key_validator"),
                 XmlUtil.GetOptionalAttribute(e, "value_validator"),
                 XmlUtil.GetOptionalAttribute(e, "validator"),
                 XmlUtil.GetOptionalAttribute(e, "tags"),
                 false,
                 DefUtil.ParseOrientation(XmlUtil.GetOptionalAttribute(e, "orientation"))
                );
        }

        private Field CreateField(string defileFile, string name, string type, string index, string sep, bool isMultiRow, string group, string resource, string converter,
            string comment, string refs, string path, string range, string keyValidator, string valueValidator, string validator, string tags,
            bool ignoreNameValidation, bool isRowOrient)
        {
            var f = new CfgField()
            {
                Name = name,
                Index = index,
                Sep = sep,
                IsMultiRow = isMultiRow,
                Groups = CreateGroups(group),
                Resource = resource,
                Converter = converter,
                Comment = comment,
                Tags = tags,
                IgnoreNameValidation = ignoreNameValidation,
                IsRowOrient = isRowOrient,
            };

            // 字段与table的默认组不一样。
            // table 默认只属于default=1的组
            // 字段默认属于所有组
            if (f.Groups.Count == 0)
            {

            }
            else if (!ValidGroup(f.Groups, out var invalidGroup))
            {
                throw new Exception($"定义文件:{defileFile} field:'{name}' group:'{invalidGroup}' 不存在");
            }
            f.Type = type;


            FillValueValidator(f, refs, "ref");
            FillValueValidator(f, path, "path"); // (ue4|unity|normal|regex);xxx;xxx
            FillValueValidator(f, range, "range");

            FillValidators(defileFile, "key_validator", keyValidator, f.KeyValidators);
            FillValidators(defileFile, "value_validator", valueValidator, f.ValueValidators);
            FillValidators(defileFile, "validator", validator, f.Validators);
            return f;
        }

        private static readonly List<string> _beanOptinsAttrs = new List<string> { "value_type", "alias", "sep", "comment", "tags" };
        private static readonly List<string> _beanRequireAttrs = new List<string> { "name" };

        protected override void AddBean(string defineFile, XElement e, string parent)
        {
            ValidAttrKeys(defineFile, e, _beanOptinsAttrs, _beanRequireAttrs);

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
    }
}
