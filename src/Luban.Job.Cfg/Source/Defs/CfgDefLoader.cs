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


        private readonly List<Branch> _branches = new();

        private readonly List<Table> _cfgTables = new List<Table>();

        private readonly List<Service> _cfgServices = new List<Service>();

        private readonly List<Group> _cfgGroups = new List<Group>();

        private readonly List<string> _defaultGroups = new List<string>();

        public CfgDefLoader(RemoteAgent agent) : base(agent)
        {
            RegisterRootDefineHandler("importexcel", AddImportExcel);
            RegisterRootDefineHandler("branch", AddBranch);
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
                Branches = _branches,
                Consts = this._consts,
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
            ValidAttrKeys(e, null, _excelImportRequireAttrs);
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

        private static readonly List<string> _branchRequireAttrs = new List<string> { "name" };
        private void AddBranch(XElement e)
        {
            ValidAttrKeys(e, null, _branchRequireAttrs);
            var branchName = e.Attribute("name").Value;
            if (string.IsNullOrWhiteSpace(branchName))
            {
                throw new Exception("branch 属性name不能为空");
            }
            if (this._branches.Any(b => b.Name == branchName))
            {
                throw new Exception($"branch '{branchName}' 重复");
            }
            _branches.Add(new Branch(branchName));
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

        private void FillValidators(string key, string attr, List<Validator> result)
        {
            if (!string.IsNullOrWhiteSpace(attr))
            {
                foreach (var validatorStr in attr.Split('#', StringSplitOptions.RemoveEmptyEntries))
                {
                    var sepIndex = validatorStr.IndexOf(':');
                    if (sepIndex < 0)
                    {
                        throw new Exception($"定义文件:{CurImportFile} 类型:'{CurDefine}' key:'{key}' attr:'{attr}' 不是合法的 validator 定义 (key1:value1#key2:value2 ...)");
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

        private ETableMode ConvertMode(string tableName, string modeStr, string indexStr)
        {
            ETableMode mode;
            switch (modeStr)
            {
                case "one":
                {
                    if (!string.IsNullOrWhiteSpace(indexStr))
                    {
                        throw new Exception($"定义文件:{CurImportFile} table:'{tableName}' mode=one 是单例表，不支持定义index属性");
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

        private readonly List<string> _tableOptionalAttrs = new List<string> { "index", "mode", "group", "branch_input", "comment", "define_from_file" };
        private readonly List<string> _tableRequireAttrs = new List<string> { "name", "value", "input" };

        private void AddTable(XElement e)
        {
            ValidAttrKeys(e, _tableOptionalAttrs, _tableRequireAttrs);
            string name = XmlUtil.GetRequiredAttribute(e, "name");
            string module = CurNamespace;
            string valueType = XmlUtil.GetRequiredAttribute(e, "value");
            bool defineFromFile = XmlUtil.GetOptionBoolAttribute(e, "define_from_file");
            string index = XmlUtil.GetOptionalAttribute(e, "index");
            string group = XmlUtil.GetOptionalAttribute(e, "group");
            string comment = XmlUtil.GetOptionalAttribute(e, "comment");
            string input = XmlUtil.GetRequiredAttribute(e, "input");
            string branchInput = XmlUtil.GetOptionalAttribute(e, "branch_input");
            string mode = XmlUtil.GetOptionalAttribute(e, "mode");
            string tags = XmlUtil.GetOptionalAttribute(e, "tags");
            AddTable(CurImportFile, name, module, valueType, index, mode, group, comment, defineFromFile, input, branchInput, tags);
        }

        private void AddTable(string defineFile, string name, string module, string valueType, string index, string mode, string group,
            string comment, bool defineFromExcel, string input, string branchInput, string tags)
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
                Mode = ConvertMode(name, mode, index),
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

            if (!string.IsNullOrWhiteSpace(branchInput))
            {
                foreach (var subBranchStr in branchInput.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    var nameAndDirs = subBranchStr.Split(':');
                    if (nameAndDirs.Length != 2)
                    {
                        throw new Exception($"定义文件:{defineFile} table:'{p.Name}' branch_input:'{subBranchStr}' 定义不合法");
                    }
                    var branchDirs = nameAndDirs[1].Split(',', ';').ToList();
                    if (!p.BranchInputFiles.TryAdd(nameAndDirs[0], branchDirs))
                    {
                        throw new Exception($"定义文件:{defineFile} table:'{p.Name}' branch_input:'{subBranchStr}' 子branch:'{nameAndDirs[0]}' 重复");
                    }
                }
            }

            _cfgTables.Add(p);
        }

        private async Task<CfgBean> LoadTableRecordDefineFromFileAsync(Table table, string dataDir)
        {
            var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, table.InputFiles, dataDir);
            var file = inputFileInfos[0];
            var source = new ExcelDataSource();
            var stream = new MemoryStream(await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5));
            var sheet = source.LoadFirstSheet(file.OriginFile, file.SheetName, stream);

            var cb = new CfgBean() { Namespace = table.Namespace, Name = table.ValueType, };

            var rc = sheet.RowColumns;
            var attrRow = sheet.RowColumns[0];
            var titleRow = sheet.RowColumns[1];
            // 有可能没有注释行，此时使用标题行，这个是必须有的
            var descRow = sheet.TitleRows >= 3 ? sheet.RowColumns[2] : titleRow;
            foreach (var f in sheet.RootFields)
            {
                var cf = new CfgField() { Name = f.Name, Id = 0 };

                string[] attrs = (attrRow[f.FromIndex].Value?.ToString() ?? "").Trim().Split('&').Select(s => s.Trim()).ToArray();

                if (attrs.Length == 0 || string.IsNullOrWhiteSpace(attrs[0]))
                {
                    throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{f.Name}' type missing!");
                }

                // 优先取desc行，如果为空,则取title行

                cf.Comment = descRow[f.FromIndex].Value?.ToString();
                if (string.IsNullOrWhiteSpace(cf.Comment))
                {
                    cf.Comment = titleRow[f.FromIndex].Value?.ToString();
                }
                if (string.IsNullOrWhiteSpace(cf.Comment))
                {
                    cf.Comment = "";
                }

                cf.Type = attrs[0];

                for (int i = 1; i < attrs.Length; i++)
                {
                    var pair = attrs[i].Split('=', 2);
                    if (pair.Length != 2)
                    {
                        throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{f.Name}' attr:'{attrs[i]}' is invalid!");
                    }
                    var attrName = pair[0].Trim();
                    var attrValue = pair[1].Trim();
                    switch (attrName)
                    {
                        case "index":
                        {
                            cf.Index = attrValue;
                            break;
                        }
                        case "sep":
                        {
                            cf.Sep = attrValue;
                            break;
                        }
                        case "ref":
                        case "path":
                        case "range":
                        {
                            var validator = new Validator() { Type = attrName, Rule = attrValue };
                            cf.Validators.Add(validator);
                            cf.ValueValidators.Add(validator);
                            break;
                        }
                        case "multi_rows":
                        {
                            cf.IsMultiRow = attrValue == "1" || attrValue.Equals("true", StringComparison.OrdinalIgnoreCase);
                            break;
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
                        case "convert":
                        {
                            cf.Converter = attrValue;
                            break;
                        }
                        case "default":
                        {
                            cf.DefaultValue = attrValue;
                            break;
                        }
                        case "tags":
                        {
                            cf.Tags = attrValue;
                            break;
                        }
                        default:
                        {
                            throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{f.Name}' attr:'{attrs[i]}' is invalid!");
                        }
                    }
                }

                cb.Fields.Add(cf);
            }
            return cb;
        }

        private async Task LoadTableRecordDefinesFromFileAsync(string dataDir)
        {
            var loadTasks = new List<Task<CfgBean>>();
            foreach (var table in this._cfgTables.Where(t => t.LoadDefineFromFile))
            {
                loadTasks.Add(Task.Run(async () => await this.LoadTableRecordDefineFromFileAsync(table, dataDir)));
            }

            foreach (var task in loadTasks)
            {
                this._beans.Add(await task);
            }
        }

        private async Task LoadTableListFromFileAsync(string dataDir)
        {
            if (this._importExcelTableFiles.Count == 0)
            {
                return;
            }
            var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelTableFiles, dataDir);

            var defTableRecordType = new DefBean(new CfgBean()
            {
                Namespace = "__intern__",
                Name = "__TableRecord__",
                Parent = "",
                Alias = "",
                IsValueType = false,
                Sep = "",
                TypeId = 0,
                IsSerializeCompatible = false,
                Fields = new List<Field>
                {
                    new CfgField() { Name = "full_name", Type = "string" },
                    new CfgField() { Name = "value_type", Type = "string" },
                    new CfgField() { Name = "index", Type = "string" },
                    new CfgField() { Name = "mode", Type = "string" },
                    new CfgField() { Name = "group", Type = "string" },
                    new CfgField() { Name = "comment", Type = "string" },
                    new CfgField() { Name = "define_from_excel", Type = "bool" },
                    new CfgField() { Name = "input", Type = "string" },
                    new CfgField() { Name = "branch_input", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
                }
            })
            {
                AssemblyBase = new DefAssembly("", null, true, Agent),
            };
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            var tableRecordType = TBean.Create(false, defTableRecordType);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true, false);
                foreach (var r in records)
                {
                    DBean data = r.Data;
                    //s_logger.Info("== read text:{}", r.Data);
                    string fullName = (data.GetField("full_name") as DString).Value.Trim();
                    string name = TypeUtil.GetName(fullName);
                    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception($"file:{file.ActualFile} 定义了一个空的table类名");
                    }
                    string module = TypeUtil.GetNamespace(fullName);
                    string valueType = (data.GetField("value_type") as DString).Value.Trim();
                    string index = (data.GetField("index") as DString).Value.Trim();
                    string mode = (data.GetField("mode") as DString).Value.Trim();
                    string group = (data.GetField("group") as DString).Value.Trim();
                    string comment = (data.GetField("comment") as DString).Value.Trim();
                    bool isDefineFromExcel = (data.GetField("define_from_excel") as DBool).Value;
                    string inputFile = (data.GetField("input") as DString).Value.Trim();
                    string branchInput = (data.GetField("branch_input") as DString).Value.Trim();
                    string tags = (data.GetField("tags") as DString).Value.Trim();
                    AddTable(file.OriginFile, name, module, valueType, index, mode, group, comment, isDefineFromExcel, inputFile, branchInput, tags);
                };
            }
        }

        private async Task LoadEnumListFromFileAsync(string dataDir)
        {
            if (this._importExcelEnumFiles.Count == 0)
            {
                return;
            }
            var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelEnumFiles, dataDir);

            var defTableRecordType = new DefBean(new CfgBean()
            {
                Namespace = "__intern__",
                Name = "__EnumInfo__",
                Parent = "",
                Alias = "",
                IsValueType = false,
                Sep = "",
                TypeId = 0,
                IsSerializeCompatible = false,
                Fields = new List<Field>
                {
                    new CfgField() { Name = "full_name", Type = "string" },
                    new CfgField() { Name = "item", Type = "string" },
                    new CfgField() { Name = "alias", Type = "string" },
                    new CfgField() { Name = "value", Type = "int" },
                    new CfgField() { Name = "comment", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
                }
            })
            {
                AssemblyBase = new DefAssembly("", null, true, Agent),
            };
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            var tableRecordType = TBean.Create(false, defTableRecordType);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true, false);

                PEnum curEnum = null;
                foreach (var r in records)
                {
                    DBean data = r.Data;
                    //s_logger.Info("== read text:{}", r.Data);
                    string fullName = (data.GetField("full_name") as DString).Value.Trim();
                    string name = TypeUtil.GetName(fullName);
                    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception($"file:{file.ActualFile} 定义了一个空的enum类名");
                    }
                    string module = TypeUtil.GetNamespace(fullName);

                    if (curEnum == null || curEnum.Name != name || curEnum.Namespace != module)
                    {
                        curEnum = new PEnum() { Name = name, Namespace = module, IsFlags = false, Comment = "", IsUniqueItemId = true };
                        this._enums.Add(curEnum);
                    }

                    string item = (data.GetField("item") as DString).Value.Trim();
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        throw new Exception($"file:{file.ActualFile} module:'{module}' name:'{name}' 定义了一个空枚举项");
                    }
                    string alias = (data.GetField("alias") as DString).Value.Trim();
                    string value = (data.GetField("value") as DInt).Value.ToString();
                    string comment = (data.GetField("comment") as DString).Value.Trim();
                    string tags = (data.GetField("tags") as DString).Value.Trim();
                    curEnum.Items.Add(new EnumItem() { Name = item, Alias = alias, Value = value, Comment = comment, Tags = tags });
                };
            }
        }

        private async Task LoadBeanListFromFileAsync(string dataDir)
        {
            if (this._importExcelBeanFiles.Count == 0)
            {
                return;
            }
            var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelBeanFiles, dataDir);


            var ass = new DefAssembly("", null, true, Agent);

            var defBeanFieldType = new DefBean(new CfgBean()
            {
                Namespace = "__intern__",
                Name = "__FieldInfo__",
                Parent = "",
                Alias = "",
                IsValueType = false,
                Sep = "",
                TypeId = 0,
                IsSerializeCompatible = false,
                Fields = new List<Field>
                {
                    new CfgField() { Name = "name", Type = "string" },
                    new CfgField() { Name = "type", Type = "string" },
                    new CfgField() { Name = "sep", Type = "string" },
                    new CfgField() { Name = "is_multi_rows", Type = "bool" },
                    new CfgField() { Name = "index", Type = "string" },
                    new CfgField() { Name = "group", Type = "string" },
                    new CfgField() { Name = "ref", Type = "string", IgnoreNameValidation = true },
                    new CfgField() { Name = "path", Type = "string" },
                    new CfgField() { Name = "comment", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
                    new CfgField() { Name = "orientation", Type = "string" },
                }
            })
            {
                AssemblyBase = ass,
            };

            defBeanFieldType.PreCompile();
            defBeanFieldType.Compile();
            defBeanFieldType.PostCompile();

            ass.AddType(defBeanFieldType);

            var defTableRecordType = new DefBean(new CfgBean()
            {
                Namespace = "__intern__",
                Name = "__BeanInfo__",
                Parent = "",
                Alias = "",
                IsValueType = false,
                Sep = "",
                TypeId = 0,
                IsSerializeCompatible = false,
                Fields = new List<Field>
                {
                    new CfgField() { Name = "full_name", Type = "string" },
                    new CfgField() { Name = "sep", Type = "string" },
                    new CfgField() { Name = "comment", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
                    new CfgField() { Name = "fields", Type = "list,__FieldInfo__", IsMultiRow = true },
                }
            })
            {
                AssemblyBase = ass,
            };
            ass.AddType(defTableRecordType);
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            ass.MarkMultiRows();
            var tableRecordType = TBean.Create(false, defTableRecordType);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true, false);

                foreach (var r in records)
                {
                    DBean data = r.Data;
                    //s_logger.Info("== read text:{}", r.Data);
                    string fullName = (data.GetField("full_name") as DString).Value.Trim();
                    string name = TypeUtil.GetName(fullName);
                    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
                    {
                        throw new Exception($"file:{file.ActualFile} 定义了一个空bean类名");
                    }
                    string module = TypeUtil.GetNamespace(fullName);


                    string sep = (data.GetField("sep") as DString).Value.Trim();
                    string comment = (data.GetField("comment") as DString).Value.Trim();
                    string tags = (data.GetField("tags") as DString).Value.Trim();
                    DList fields = data.GetField("fields") as DList;
                    var curBean = new CfgBean()
                    {
                        Name = name,
                        Namespace = module,
                        Sep = sep,
                        Comment = comment,
                        Tags = tags,
                        Parent = "",
                        Fields = fields.Datas.Select(d => (DBean)d).Select(b => this.CreateField(
                            (b.GetField("name") as DString).Value.Trim(),
                            (b.GetField("type") as DString).Value.Trim(),
                            (b.GetField("index") as DString).Value.Trim(),
                            (b.GetField("sep") as DString).Value.Trim(),
                            (b.GetField("is_multi_rows") as DBool).Value,
                            (b.GetField("group") as DString).Value,
                            "",
                            "",
                            (b.GetField("comment") as DString).Value.Trim(),
                            (b.GetField("ref") as DString).Value.Trim(),
                            (b.GetField("path") as DString).Value.Trim(),
                            "",
                            "",
                            "",
                            "",
                            (b.GetField("tags") as DString).Value.Trim(),
                            false,
                            DefUtil.ParseOrientation((b.GetField("orientation") as DString).Value)
                            )).ToList(),
                    };
                    this._beans.Add(curBean);
                };
            }
        }

        public async Task LoadDefinesFromFileAsync(string dataDir)
        {
            await Task.WhenAll(LoadTableListFromFileAsync(dataDir), LoadEnumListFromFileAsync(dataDir), LoadBeanListFromFileAsync(dataDir));
            await LoadTableRecordDefinesFromFileAsync(dataDir);
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

        protected override Field CreateField(XElement e)
        {
            ValidAttrKeys(e, _fieldOptionalAttrs, _fieldRequireAttrs);

            return CreateField(XmlUtil.GetRequiredAttribute(e, "name"),
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

        private Field CreateField(string name, string type, string index, string sep, bool isMultiRow, string group, string resource, string converter,
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
                throw new Exception($"定义文件:{CurImportFile} field:'{name}' group:'{invalidGroup}' 不存在");
            }
            f.Type = type;


            FillValueValidator(f, refs, "ref");
            FillValueValidator(f, path, "path"); // (ue4|unity|normal|regex);xxx;xxx
            FillValueValidator(f, range, "range");

            FillValidators("key_validator", keyValidator, f.KeyValidators);
            FillValidators("value_validator", valueValidator, f.ValueValidators);
            FillValidators("validator", validator, f.Validators);
            return f;
        }

        private static readonly List<string> _beanOptinsAttrs = new List<string> { "value_type", "alias", "sep", "comment", "tags" };
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
