// namespace Luban.Plugin.Loader;
//
// public class ExcelSchemaLoader : ISchemaLoader
// {
//     private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
//     
//     public void Load(string fileName, ISchemaCollector collector)
//     {
//         throw new NotImplementedException();
//     }
//     
//
//     private readonly Dictionary<string, Action<XElement>> _rootDefineHandlers = new Dictionary<string, Action<XElement>>();
//     private readonly Dictionary<string, Action<string, XElement>> _moduleDefineHandlers = new();
//
//     protected readonly Stack<string> _namespaceStack = new Stack<string>();
//
//     protected string CurNamespace => _namespaceStack.Count > 0 ? _namespaceStack.Peek() : "";
//     
//     private async Task<RawBean> LoadTableValueTypeDefineFromFileAsync(RawTable rawTable, string dataDir)
//     {
//         var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(rawTable.InputFiles, dataDir);
//         var file = inputFileInfos[0];
//         RawSheetTableDefInfo tableDefInfo;
//         if (!ExcelTableValueTypeDefInfoCacheManager.Instance.TryGetTableDefInfo(file.MD5, file.SheetName, out tableDefInfo))
//         {
//             var source = new ExcelRowColumnDataSource();
//             var stream = new MemoryStream(await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5));
//             tableDefInfo = source.LoadTableDefInfo(file.OriginFile, file.SheetName, stream);
//             ExcelTableValueTypeDefInfoCacheManager.Instance.AddTableDefInfoToCache(file.MD5, file.SheetName, tableDefInfo);
//         }
//
//         var (valueType, tags) = DefUtil.ParseType(rawTable.ValueType);
//         var ns = TypeUtil.GetNamespace(valueType);
//         string valueTypeNamespace = string.IsNullOrEmpty(ns) ? rawTable.Namespace : ns;
//         string valueTypeName = TypeUtil.GetName(valueType);
//         RawBean parentRawBean = null;
//         if (tags.TryGetValue("parent", out string parentType))
//         {
//             var parentNs = TypeUtil.GetNamespace(parentType);
//             string parentNamespace = string.IsNullOrEmpty(parentNs) ? rawTable.Namespace : parentNs;
//             string parentName = TypeUtil.GetName(parentType);
//             parentType = string.Join(".", parentNamespace, parentName);
//             parentRawBean = _beans.FirstOrDefault(x => x.FullName == parentType);
//         }
//         var cb = new RawBean() { Namespace = valueTypeNamespace, Name = valueTypeName, Comment = "", Parent = parentType };
//         if (parentRawBean != null)
//         {
//             foreach (var parentField in parentRawBean.Fields)
//             {
//                 if (!tableDefInfo.FieldInfos.Any(x => x.Key == parentField.Name && x.Value.Type == parentField.Type))
//                 {
//                     throw new Exception($"table:'{rawTable.Name}' file:{file.OriginFile} title:缺失父类字段：'{parentField.Type} {parentField.Name}'");
//                 }
//             }
//         }
//
//         foreach (var (name, f) in tableDefInfo.FieldInfos)
//         {
//             if (parentRawBean != null && parentRawBean.Fields.Any(x => x.Name == name && x.Type == f.Type))
//             {
//                 continue;
//             }
//             var cf = new RawField() { Name = name, Id = 0 };
//
//             string[] attrs = f.Type.Trim().Split('&').Select(s => s.Trim()).ToArray();
//
//             if (attrs.Length == 0 || string.IsNullOrWhiteSpace(attrs[0]))
//             {
//                 throw new Exception($"table:'{rawTable.Name}' file:{file.OriginFile} title:'{name}' type missing!");
//             }
//
//             cf.Comment = f.Desc;
//             cf.Type = attrs[0];
//             for (int i = 1; i < attrs.Length; i++)
//             {
//                 var pair = attrs[i].Split('=', 2);
//                 if (pair.Length != 2)
//                 {
//                     throw new Exception($"table:'{rawTable.Name}' file:{file.OriginFile} title:'{name}' attr:'{attrs[i]}' is invalid!");
//                 }
//                 var attrName = pair[0].Trim();
//                 var attrValue = pair[1].Trim();
//                 switch (attrName)
//                 {
//                     case "index":
//                     case "ref":
//                     case "path":
//                     case "range":
//                     case "sep":
//                     case "regex":
//                     {
//                         throw new Exception($"table:'{rawTable.Name}' file:{file.OriginFile} title:'{name}' attr:'{attrName}' 属于type的属性，必须用#分割，尝试'{cf.Type}#{attrs[i]}'");
//                     }
//                     case "group":
//                     {
//                         cf.Groups = attrValue.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
//                         break;
//                     }
//                     case "comment":
//                     {
//                         cf.Comment = attrValue;
//                         break;
//                     }
//                     case "tags":
//                     {
//                         cf.Tags = attrValue;
//                         break;
//                     }
//                     default:
//                     {
//                         throw new Exception($"table:'{rawTable.Name}' file:{file.OriginFile} title:'{name}' attr:'{attrs[i]}' is invalid!");
//                     }
//                 }
//             }
//
//             if (!string.IsNullOrEmpty(f.Groups))
//             {
//                 cf.Groups = f.Groups.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
//             }
//
//             cb.Fields.Add(cf);
//         }
//         return cb;
//     }
//
//     private async Task LoadTableValueTypeDefinesFromFileAsync(string dataDir)
//     {
//         var loadTasks = new List<Task<RawBean>>();
//         foreach (var table in this._cfgTables.Where(t => t.LoadDefineFromFile))
//         {
//             loadTasks.Add(Task.Run(async () => await this.LoadTableValueTypeDefineFromFileAsync(table, dataDir)));
//         }
//
//         foreach (var task in loadTasks)
//         {
//             this._beans.Add(await task);
//         }
//     }
//
//     private async Task LoadTableListFromFileAsync(string dataDir)
//     {
//         if (this._importExcelTableFiles.Count == 0)
//         {
//             return;
//         }
//         var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelTableFiles, dataDir);
//
//         var defTableRecordType = new DefBean(new RawBean()
//         {
//             Namespace = "__intern__",
//             Name = "__TableRecord__",
//             Parent = "",
//             Alias = "",
//             IsValueType = false,
//             Sep = "",
//             TypeId = 0,
//             IsSerializeCompatible = false,
//             Fields = new List<RawField>
//             {
//                 new RawField() { Name = "full_name", Type = "string" },
//                 new RawField() { Name = "value_type", Type = "string" },
//                 new RawField() { Name = "index", Type = "string" },
//                 new RawField() { Name = "mode", Type = "string" },
//                 new RawField() { Name = "group", Type = "string" },
//                 new RawField() { Name = "comment", Type = "string" },
//                 new RawField() { Name = "define_from_file", Type = "bool" },
//                 new RawField() { Name = "input", Type = "string" },
//                 new RawField() { Name = "output", Type = "string" },
//                 new RawField() { Name = "patch_input", Type = "string" },
//                 new RawField() { Name = "tags", Type = "string" },
//                 new RawField() { Name = "options", Type = "string" },
//             }
//         })
//         {
//             Assembly = new DefAssembly("", null, new List<string>(), Agent),
//         };
//         defTableRecordType.PreCompile();
//         defTableRecordType.Compile();
//         defTableRecordType.PostCompile();
//         var tableRecordType = TBean.Create(false, defTableRecordType, null);
//
//         foreach (var file in inputFileInfos)
//         {
//             var source = new ExcelRowColumnDataSource();
//             var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
//             (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(file.OriginFile));
//             var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, actualFile, sheetName, bytes, true, null);
//             foreach (var r in records)
//             {
//                 DBean data = r.Data;
//                 //s_logger.Info("== read text:{}", r.Data);
//                 string fullName = (data.GetField("full_name") as DString).Value.Trim();
//                 string name = TypeUtil.GetName(fullName);
//                 if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
//                 {
//                     throw new Exception($"file:{file.ActualFile} 定义了一个空的table类名");
//                 }
//                 string module = TypeUtil.GetNamespace(fullName);
//                 string valueType = (data.GetField("value_type") as DString).Value.Trim();
//                 string index = (data.GetField("index") as DString).Value.Trim();
//                 string mode = (data.GetField("mode") as DString).Value.Trim();
//                 string group = (data.GetField("group") as DString).Value.Trim();
//                 string comment = (data.GetField("comment") as DString).Value.Trim();
//                 bool isDefineFromExcel = (data.GetField("define_from_file") as DBool).Value;
//                 string inputFile = (data.GetField("input") as DString).Value.Trim();
//                 string patchInput = (data.GetField("patch_input") as DString).Value.Trim();
//                 string tags = (data.GetField("tags") as DString).Value.Trim();
//                 string outputFile = (data.GetField("output") as DString).Value.Trim();
//                 string options = (data.GetField("options") as DString).Value.Trim();
//                 AddTable(file.OriginFile, name, module, valueType, index, mode, group, comment, isDefineFromExcel, inputFile, patchInput, tags, outputFile, options);
//             };
//         }
//     }
//
//     private async Task LoadEnumListFromFileAsync(string dataDir)
//     {
//         if (this._importExcelEnumFiles.Count == 0)
//         {
//             return;
//         }
//         var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelEnumFiles, dataDir);
//
//
//         var ass = new DefAssembly("", null, new List<string>(), Agent);
//
//         var enumItemType = new DefBean(new RawBean()
//         {
//             Namespace = "__intern__",
//             Name = "__EnumItem__",
//             Parent = "",
//             Alias = "",
//             IsValueType = false,
//             Sep = "",
//             TypeId = 0,
//             IsSerializeCompatible = false,
//             Fields = new List<RawField>
//             {
//                 new RawField() { Name = "name", Type = "string" },
//                 new RawField() { Name = "alias", Type = "string" },
//                 new RawField() { Name = "value", Type = "string" },
//                 new RawField() { Name = "comment", Type = "string" },
//                 new RawField() { Name = "tags", Type = "string" },
//             }
//         })
//         {
//             Assembly = ass,
//         };
//         ass.AddType(enumItemType);
//         enumItemType.PreCompile();
//         enumItemType.Compile();
//         enumItemType.PostCompile();
//
//         var defTableRecordType = new DefBean(new RawBean()
//         {
//             Namespace = "__intern__",
//             Name = "__EnumInfo__",
//             Parent = "",
//             Alias = "",
//             IsValueType = false,
//             Sep = "",
//             TypeId = 0,
//             IsSerializeCompatible = false,
//             Fields = new List<RawField>
//             {
//                 new RawField() { Name = "full_name", Type = "string" },
//                 new RawField() { Name = "comment", Type = "string" },
//                 new RawField() { Name = "flags", Type = "bool" },
//                 new RawField() { Name = "tags", Type = "string" },
//                 new RawField() { Name = "unique", Type = "bool" },
//                 new RawField() { Name = "items", Type = "list,__EnumItem__" },
//             }
//         })
//         {
//             Assembly = ass,
//         };
//         ass.AddType(defTableRecordType);
//         defTableRecordType.PreCompile();
//         defTableRecordType.Compile();
//         defTableRecordType.PostCompile();
//         var tableRecordType = TBean.Create(false, defTableRecordType, null);
//
//         foreach (var file in inputFileInfos)
//         {
//             var source = new ExcelRowColumnDataSource();
//             var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
//             (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(file.OriginFile));
//             var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, actualFile, sheetName, bytes, true, null);
//
//             foreach (var r in records)
//             {
//                 DBean data = r.Data;
//                 //s_logger.Info("== read text:{}", r.Data);
//                 string fullName = (data.GetField("full_name") as DString).Value.Trim();
//                 string name = TypeUtil.GetName(fullName);
//                 if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
//                 {
//                     throw new Exception($"file:{file.ActualFile} 定义了一个空的enum类名");
//                 }
//                 string module = TypeUtil.GetNamespace(fullName);
//
//                 DList items = (data.GetField("items") as DList);
//
//                 var curEnum = new RawEnum()
//                 {
//                     Name = name,
//                     Namespace = module,
//                     IsFlags = (data.GetField("flags") as DBool).Value,
//                     Tags = (data.GetField("tags") as DString).Value,
//                     Comment = (data.GetField("comment") as DString).Value,
//                     IsUniqueItemId = (data.GetField("unique") as DBool).Value,
//                     Items = items.Datas.Cast<DBean>().Select(d => new EnumItem()
//                     {
//                         Name = (d.GetField("name") as DString).Value,
//                         Alias = (d.GetField("alias") as DString).Value,
//                         Value = (d.GetField("value") as DString).Value,
//                         Comment = (d.GetField("comment") as DString).Value,
//                         Tags = (d.GetField("tags") as DString).Value,
//                     }).ToList(),
//                 };
//                 this._enums.Add(curEnum);
//             };
//         }
//     }
//
//     private async Task LoadBeanListFromFileAsync(string dataDir)
//     {
//         if (this._importExcelBeanFiles.Count == 0)
//         {
//             return;
//         }
//         var inputFileInfos = await DataLoaderUtil.CollectInputFilesAsync(this.Agent, this._importExcelBeanFiles, dataDir);
//
//
//         var ass = new DefAssembly("", null, new List<string>(), Agent);
//
//         var defBeanFieldType = new DefBean(new RawBean()
//         {
//             Namespace = "__intern__",
//             Name = "__FieldInfo__",
//             Parent = "",
//             Alias = "",
//             IsValueType = false,
//             Sep = "",
//             TypeId = 0,
//             IsSerializeCompatible = false,
//             Fields = new List<RawField>
//             {
//                 new RawField() { Name = "name", Type = "string" },
//                 new RawField() { Name = "type", Type = "string" },
//                 new RawField() { Name = "group", Type = "string" },
//                 new RawField() { Name = "comment", Type = "string" },
//                 new RawField() { Name = "tags", Type = "string" },
//             }
//         })
//         {
//             Assembly = ass,
//         };
//
//         defBeanFieldType.PreCompile();
//         defBeanFieldType.Compile();
//         defBeanFieldType.PostCompile();
//
//         ass.AddType(defBeanFieldType);
//
//         var defTableRecordType = new DefBean(new RawBean()
//         {
//             Namespace = "__intern__",
//             Name = "__BeanInfo__",
//             Parent = "",
//             Alias = "",
//             IsValueType = false,
//             Sep = "",
//             TypeId = 0,
//             IsSerializeCompatible = false,
//             Fields = new List<RawField>
//             {
//                 new RawField() { Name = "full_name", Type = "string" },
//                 new RawField() {Name =  "parent", Type = "string" },
//                 new RawField() { Name = "sep", Type = "string" },
//                 new RawField() { Name = "alias", Type = "string" },
//                 new RawField() { Name = "comment", Type = "string" },
//                 new RawField() { Name = "tags", Type = "string" },
//                 new RawField() { Name = "fields", Type = "list,__FieldInfo__" },
//             }
//         })
//         {
//             Assembly = ass,
//         };
//         ass.AddType(defTableRecordType);
//         defTableRecordType.PreCompile();
//         defTableRecordType.Compile();
//         defTableRecordType.PostCompile();
//         var tableRecordType = TBean.Create(false, defTableRecordType, null);
//
//         foreach (var file in inputFileInfos)
//         {
//             var source = new ExcelRowColumnDataSource();
//             var bytes = await this.Agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5);
//             var records = DataLoaderUtil.LoadCfgRecords(tableRecordType, file.OriginFile, file.SheetName, bytes, true, null);
//
//             foreach (var r in records)
//             {
//                 DBean data = r.Data;
//                 //s_logger.Info("== read text:{}", r.Data);
//                 string fullName = (data.GetField("full_name") as DString).Value.Trim();
//                 string name = TypeUtil.GetName(fullName);
//                 if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(name))
//                 {
//                     throw new Exception($"file:'{file.ActualFile}' 定义了一个空bean类名");
//                 }
//                 string module = TypeUtil.GetNamespace(fullName);
//
//                 string parent = (data.GetField("parent") as DString).Value.Trim();
//                 string sep = (data.GetField("sep") as DString).Value.Trim();
//                 string alias = (data.GetField("alias") as DString).Value.Trim();
//                 string comment = (data.GetField("comment") as DString).Value.Trim();
//                 string tags = (data.GetField("tags") as DString).Value.Trim();
//                 DList fields = data.GetField("fields") as DList;
//                 var curBean = new RawBean()
//                 {
//                     Name = name,
//                     Namespace = module,
//                     Sep = sep,
//                     Alias = alias,
//                     Comment = comment,
//                     Tags = tags,
//                     Parent = parent,
//                     Fields = fields.Datas.Select(d => (DBean)d).Select(b => this.CreateField(
//                         file.ActualFile,
//                         (b.GetField("name") as DString).Value.Trim(),
//                         (b.GetField("type") as DString).Value.Trim(),
//                         (b.GetField("group") as DString).Value,
//                         (b.GetField("comment") as DString).Value.Trim(),
//                         (b.GetField("tags") as DString).Value.Trim(),
//                         false
//                     )).ToList(),
//                 };
//                 this._beans.Add(curBean);
//             };
//         }
//     }
// }