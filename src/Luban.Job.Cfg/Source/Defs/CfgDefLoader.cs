using Luban.Common.Utils;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.RawDefs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
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

        private readonly List<string> _tableOptionalAttrs = new List<string> { "index", "mode", "group", "patch_input", "comment", "define_from_file", "output" };
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
            string output = XmlUtil.GetOptionalAttribute(e, "output");
            AddTable(defineFile, name, module, valueType, index, mode, group, comment, defineFromFile, input, patchInput, tags, output);
        }

        private void AddTable(string defineFile, string name, string module, string valueType, string index, string mode, string group,
            string comment, bool defineFromExcel, string input, string patchInput, string tags, string outputFileName)
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
                OutputFile = outputFileName,
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

        private async Task<CfgBean> LoadTableValueTypeDefineFromFileAsync(Table table, string dataDir)
        {
            var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, table.InputFiles, dataDir);
            var file = inputFileInfos[0];
            RawSheetTableDefInfo tableDefInfo;
            if (!ExcelTableValueTypeDefInfoCacheManager.Instance.TryGetTableDefInfo(file.MD5, file.SheetName, out tableDefInfo))
            {
                var source = new ExcelDataSource();
                var stream = new MemoryStream(await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5));
                tableDefInfo = source.LoadTableDefInfo(file.OriginFile, file.SheetName, stream);
                ExcelTableValueTypeDefInfoCacheManager.Instance.AddTableDefInfoToCache(file.MD5, file.SheetName, tableDefInfo);
            }

            var cb = new CfgBean() { Namespace = table.Namespace, Name = table.ValueType, Comment = "" };
#if !LUBAN_LITE
            foreach (var (name, f) in tableDefInfo.FieldInfos)
            {
#else
            foreach (var e in tableDefInfo.FieldInfos)
            {
                var name = e.Key;
                var f = e.Value;
#endif
                var cf = new CfgField() { Name = name, Id = 0 };

                string[] attrs = f.Type.Trim().Split('&').Select(s => s.Trim()).ToArray();

                if (attrs.Length == 0 || string.IsNullOrWhiteSpace(attrs[0]))
                {
                    throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{name}' type missing!");
                }

                cf.Comment = f.Desc;

                cf.Type = attrs[0];

                for (int i = 1; i < attrs.Length; i++)
                {
#if !LUBAN_LITE
                    var pair = attrs[i].Split('=', 2);
#else
                    var pair = attrs[i].Split(new char[] { '=' }, 2);
#endif
                    if (pair.Length != 2)
                    {
                        throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{name}' attr:'{attrs[i]}' is invalid!");
                    }
                    var attrName = pair[0].Trim();
                    var attrValue = pair[1].Trim();
                    switch (attrName)
                    {
                        case "index":
                        case "ref":
                        case "path":
                        case "range":
                        {
                            cf.Type = cf.Type + "&(" + attrs[i] + ")";
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
                        case "tags":
                        {
                            cf.Tags = attrValue;
                            break;
                        }
                        default:
                        {
                            throw new Exception($"table:'{table.Name}' file:{file.OriginFile} title:'{name}' attr:'{attrs[i]}' is invalid!");
                        }
                    }
                }

                cb.Fields.Add(cf);
            }
            return cb;
        }

        private async Task LoadTableValueTypeDefinesFromFileAsync(string dataDir)
        {
            var loadTasks = new List<Task<CfgBean>>();
            foreach (var table in this._cfgTables.Where(t => t.LoadDefineFromFile))
            {
                loadTasks.Add(Task.Run(async () => await this.LoadTableValueTypeDefineFromFileAsync(table, dataDir)));
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
                    new CfgField() { Name = "output", Type = "string" },
                    new CfgField() { Name = "patch_input", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
                }
            })
            {
                AssemblyBase = new DefAssembly("", null, new List<string>(), Agent),
            };
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            var tableRecordType = TBean.Create(false, defTableRecordType, null);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true);
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
                    string patchInput = (data.GetField("patch_input") as DString).Value.Trim();
                    string tags = (data.GetField("tags") as DString).Value.Trim();
                    string outputFile = (data.GetField("output") as DString).Value.Trim();
                    AddTable(file.OriginFile, name, module, valueType, index, mode, group, comment, isDefineFromExcel, inputFile, patchInput, tags, outputFile);
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
                AssemblyBase = new DefAssembly("", null, new List<string>(), Agent),
            };
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            var tableRecordType = TBean.Create(false, defTableRecordType, null);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true);

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


            var ass = new DefAssembly("", null, new List<string>(), Agent);

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
                    new CfgField() { Name = "group", Type = "string" },
                    new CfgField() { Name = "comment", Type = "string" },
                    new CfgField() { Name = "tags", Type = "string" },
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
                    new CfgField() { Name = "fields", Type = "list,__FieldInfo__" },
                }
            })
            {
                AssemblyBase = ass,
            };
            ass.AddType(defTableRecordType);
            defTableRecordType.PreCompile();
            defTableRecordType.Compile();
            defTableRecordType.PostCompile();
            var tableRecordType = TBean.Create(false, defTableRecordType, null);

            foreach (var file in inputFileInfos)
            {
                var source = new ExcelDataSource();
                var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
                var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, null, bytes, true);

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
                            file.ActualFile,
                            (b.GetField("name") as DString).Value.Trim(),
                            (b.GetField("type") as DString).Value.Trim(),
                            (b.GetField("group") as DString).Value,
                            (b.GetField("comment") as DString).Value.Trim(),
                            (b.GetField("tags") as DString).Value.Trim(),
                            false
                            )).ToList(),
                    };
                    this._beans.Add(curBean);
                };
            }
        }

        public async Task LoadDefinesFromFileAsync(string dataDir)
        {
            await Task.WhenAll(LoadTableListFromFileAsync(dataDir), LoadEnumListFromFileAsync(dataDir), LoadBeanListFromFileAsync(dataDir));
            await LoadTableValueTypeDefinesFromFileAsync(dataDir);
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

        override protected Field CreateField(string defineFile, XElement e)
        {
            ValidAttrKeys(defineFile, e, _fieldOptionalAttrs, _fieldRequireAttrs);

            string typeStr = XmlUtil.GetRequiredAttribute(e, "type");

            string refStr = XmlUtil.GetOptionalAttribute(e, "ref");
            if (!string.IsNullOrWhiteSpace(refStr))
            {
                typeStr = typeStr + "&(ref=" + refStr + ")";
            }
            string pathStr = XmlUtil.GetOptionalAttribute(e, "path");
            if (!string.IsNullOrWhiteSpace(pathStr))
            {
                typeStr = typeStr + "&(path=" + pathStr + ")";
            }

            return CreateField(defineFile, XmlUtil.GetRequiredAttribute(e, "name"),
                typeStr,
                 XmlUtil.GetOptionalAttribute(e, "group"),
                 XmlUtil.GetOptionalAttribute(e, "comment"),
                 XmlUtil.GetOptionalAttribute(e, "tags"),
                 false
                );
        }

        private Field CreateField(string defileFile, string name, string type, string group,
            string comment, string tags,
            bool ignoreNameValidation)
        {
            var f = new CfgField()
            {
                Name = name,
                Groups = CreateGroups(group),
                Comment = comment,
                Tags = tags,
                IgnoreNameValidation = ignoreNameValidation,
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


            //FillValueValidator(f, refs, "ref");
            //FillValueValidator(f, path, "path"); // (ue4|unity|normal|regex);xxx;xxx
            //FillValueValidator(f, range, "range");

            //FillValidators(defileFile, "key_validator", keyValidator, f.KeyValidators);
            //FillValidators(defileFile, "value_validator", valueValidator, f.ValueValidators);
            //FillValidators(defileFile, "validator", validator, f.Validators);
            return f;
        }

        private static readonly List<string> _beanOptinsAttrs = new List<string> { "value_type", "alias", "sep", "comment", "tags" };
        private static readonly List<string> _beanRequireAttrs = new List<string> { "name" };

        override protected void AddBean(string defineFile, XElement e, string parent)
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
