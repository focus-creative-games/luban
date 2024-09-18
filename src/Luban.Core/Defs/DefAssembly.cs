using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;

namespace Luban.Defs;

public class DefAssembly
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public Dictionary<string, DefTypeBase> Types { get; } = new();

    public List<DefTypeBase> TypeList { get; } = new();

    private readonly Dictionary<string, DefTypeBase> _notCaseSenseTypes = new();

    private readonly HashSet<string> _namespaces = new();

    private readonly Dictionary<string, DefTypeBase> _notCaseSenseNamespaces = new();

    private readonly List<RawTarget> _targets;

    public RawTarget Target { get; }

    public IReadOnlyList<RawTarget> Targets => _targets;

    public RawTarget GetTarget(string targetName)
    {
        return _targets.Find(t => t.Name == targetName);
    }

    private readonly List<DefTable> _exportTables;

    public List<DefTable> ExportTables => _exportTables;

    private Dictionary<string, string> _variants;

    public bool TryGetVariantName(string variantKey, out string variantName)
    {
        if (_variants == null)
        {
            variantName = "";
            return false;
        }
        return _variants.TryGetValue(variantKey, out variantName);
    }

    public DefAssembly(RawAssembly assembly, string target, List<string> outputTables, List<RawGroup> groupDefs, Dictionary<string, string> variants)
    {
        _targets = assembly.Targets;
        Target = GetTarget(target);
        if (Target == null)
        {
            throw new Exception($"target:{target} is invalid");
        }
        _variants = variants;

        foreach (var g in assembly.RefGroups)
        {
            AddRefGroup(g);
        }

        foreach (var e in assembly.Enums)
        {
            AddType(new DefEnum(e));
        }

        foreach (var b in assembly.Beans)
        {
            AddType(new DefBean(b));
        }

        foreach (var p in assembly.Tables)
        {
            var table = new DefTable(p);
            AddType(table);
            AddCfgTable(table);
        }

        _targets.AddRange(assembly.Targets);

        List<DefTable> originTables = GetAllTables();
        if (outputTables.Count == 0)
        {
            _exportTables = originTables.Where(t => NeedExport(t.Groups, groupDefs)).ToList();
        }
        else
        {
            _exportTables = new List<DefTable>();
            foreach (var tableName in outputTables)
            {
                DefTable table = GetCfgTable(tableName);
                if (table != null)
                {
                    _exportTables.Add(table);
                }
                else
                {
                    throw new Exception($"outputTable:{tableName} not found");
                }
            }
        }

        foreach (var table in _exportTables)
        {
            table.IsExported = true;
        }

        foreach (var type in TypeList)
        {
            type.Assembly = this;
        }

        foreach (var type in TypeList)
        {
            type.PreCompile();
        }
        foreach (var type in TypeList)
        {
            type.Compile();
        }

        foreach (var type in TypeList)
        {
            type.PostCompile();
        }
    }

    public bool NeedExport(List<string> groups, List<RawGroup> groupDefs)
    {
        if (groups.Count == 0)
        {
            return groupDefs == null || Target.Groups.Any(g => groupDefs.FirstOrDefault(gd => gd.Names.Contains(g))?.IsDefault == true);
        }
        return groups.Any(g => Target.Groups.Contains(g));
    }


    private readonly Dictionary<string, DefRefGroup> _refGroups = new();

    public Dictionary<string, DefTable> TablesByName { get; } = new();

    public Dictionary<string, DefTable> TablesByFullName { get; } = new();


    private readonly Dictionary<(DefTypeBase, bool), TType> _cacheDefTTypes = new();

    public void AddCfgTable(DefTable table)
    {
        if (!TablesByFullName.TryAdd(table.FullName, table))
        {
            throw new Exception($"table:'{table.FullName}' duplicated");
        }
        if (!TablesByName.TryAdd(table.Name, table))
        {
            throw new Exception($"table:'{table.FullName} 与 table:'{TablesByName[table.Name].FullName}' 的表名重复(不同模块下也不允许定义同名表，将来可能会放开限制)");
        }
    }

    public DefTable GetCfgTable(string name)
    {
        return TablesByFullName.TryGetValue(name, out var t) ? t : null;
    }


    public List<DefTable> GetAllTables()
    {
        return TypeList.Where(t => t is DefTable).Cast<DefTable>().ToList();
    }

    private void AddRefGroup(RawRefGroup g)
    {
        if (_refGroups.ContainsKey(g.Name))
        {
            throw new Exception($"refgroup:{g.Name} 重复");
        }
        _refGroups.Add(g.Name, new DefRefGroup(g));
    }

    public DefRefGroup GetRefGroup(string groupName)
    {
        return _refGroups.TryGetValue(groupName, out var refGroup) ? refGroup : null;
    }

    public void AddType(DefTypeBase type)
    {
        string fullName = type.FullName;
        if (Types.ContainsKey(fullName))
        {
            throw new Exception($"type:'{fullName}' duplicate");
        }

        if (!_notCaseSenseTypes.TryAdd(fullName.ToLower(), type))
        {
            throw new Exception($"type:'{fullName}' 和 type:'{_notCaseSenseTypes[fullName.ToLower()].FullName}' 类名小写重复. 在win平台有问题");
        }

        string namespaze = type.Namespace;
        if (_namespaces.Add(namespaze) && !_notCaseSenseNamespaces.TryAdd(namespaze.ToLower(), type))
        {
            throw new Exception($"type:'{fullName}' 和 type:'{_notCaseSenseNamespaces[namespaze.ToLower()].FullName}' 命名空间小写重复. 在win平台有问题，请修改定义并删除生成的代码目录后再重新生成");
        }

        Types.Add(fullName, type);
        TypeList.Add(type);
    }

    public DefTypeBase GetDefType(string fullName)
    {
        return Types.TryGetValue(fullName, out var type) ? type : null;
    }

    public DefTypeBase GetDefType(string module, string type)
    {
        if (Types.TryGetValue(TypeUtil.MakeFullName(module, type), out var t))
        {
            return t;
        }
        else if (Types.TryGetValue(type, out t))
        {
            return t;
        }
        else
        {
            return null;
        }
    }

    TType GetOrCreateTEnum(DefEnum defType, bool nullable, Dictionary<string, string> tags)
    {
        if (tags == null || tags.Count == 0)
        {
            if (_cacheDefTTypes.TryGetValue((defType, nullable), out var t))
            {
                return t;
            }
            else
            {
                return _cacheDefTTypes[(defType, nullable)] = TEnum.Create(nullable, defType, tags);
            }
        }
        else
        {
            return TEnum.Create(nullable, defType, tags);
            ;
        }
    }

    TType GetOrCreateTBean(DefTypeBase defType, bool nullable, Dictionary<string, string> tags)
    {
        if (tags == null || tags.Count == 0)
        {
            if (_cacheDefTTypes.TryGetValue((defType, nullable), out var t))
            {
                return t;
            }
            else
            {
                return _cacheDefTTypes[(defType, nullable)] = TBean.Create(nullable, (DefBean)defType, tags);
            }
        }
        else
        {
            return TBean.Create(nullable, (DefBean)defType, tags);
        }
    }

    public TType GetDefTType(string module, string type, bool nullable, Dictionary<string, string> tags)
    {
        var defType = GetDefType(module, type);
        switch (defType)
        {
            case DefBean d:
                return GetOrCreateTBean(d, nullable, tags);
            case DefEnum d:
                return GetOrCreateTEnum(d, nullable, tags);
            default:
                return null;
        }
    }

    public TType CreateType(string module, string type, bool containerElementType)
    {
        type = DefUtil.TrimBracePairs(type);
        int sepIndex = DefUtil.IndexOfBaseTypeEnd(type);
        if (sepIndex > 0)
        {
            string containerTypeAndTags = DefUtil.TrimBracePairs(type.Substring(0, sepIndex));
            var elementTypeAndTags = type.Substring(sepIndex + 1);
            var (containerType, containerTags) = DefUtil.ParseTypeAndVaildAttrs(containerTypeAndTags);
            return CreateContainerType(module, containerType, containerTags, elementTypeAndTags.Trim());
        }
        else
        {
            return CreateNotContainerType(module, type, containerElementType);
        }
    }

    protected TType CreateNotContainerType(string module, string rawType, bool containerElementType)
    {
        bool defaultAble = true;
        bool nullable = false;
        // 去掉 rawType 两侧的匹配的 ()
        rawType = DefUtil.TrimBracePairs(rawType);
        var (type, tags) = DefUtil.ParseTypeAndVaildAttrs(rawType);

        while (true)
        {
            if (type.EndsWith('?'))
            {
                if (containerElementType)
                {
                    throw new Exception($"container element type can't be nullable type:'{module}.{type}'");
                }
                nullable = true;
                type = type[..^1];
                continue;
            }

            if (type.EndsWith("!"))
            {
                defaultAble = false;
                type = type[..^1];
                continue;
            }
            break;
        }

        if (!defaultAble)
        {
            tags.TryAdd("not-default", "1");
        }

        switch (type)
        {
            case "bool":
                return TBool.Create(nullable, tags);
            case "uint8":
            case "byte":
                return TByte.Create(nullable, tags);
            case "int16":
            case "short":
                return TShort.Create(nullable, tags);
            case "int32":
            case "int":
                return TInt.Create(nullable, tags);
            case "int64":
            case "long":
                return TLong.Create(nullable, tags, false);
            case "bigint":
                return TLong.Create(nullable, tags, true);
            case "float32":
            case "float":
                return TFloat.Create(nullable, tags);
            case "float64":
            case "double":
                return TDouble.Create(nullable, tags);
            case "string":
                return TString.Create(nullable, tags);
            case "text":
                tags.Add("text", "1");
                return TString.Create(nullable, tags);
            case "time":
            case "datetime":
                return TDateTime.Create(nullable, tags);
            default:
            {
                var dtype = GetDefTType(module, type, nullable, tags);
                if (dtype != null)
                {
                    return dtype;
                }
                else
                {
                    throw new ArgumentException($"invalid type. module:'{module}' type:'{type}'");
                }
            }
        }
    }

    TMap CreateMapType(string module, Dictionary<string, string> tags, string keyValueType, bool isTreeMap)
    {
        int typeSepIndex = DefUtil.IndexOfElementTypeSep(keyValueType);
        if (typeSepIndex <= 0 || typeSepIndex >= keyValueType.Length - 1)
        {
            throw new ArgumentException($"invalid map element type:'{keyValueType}'");
        }
        return TMap.Create(false, tags,
            CreateNotContainerType(module, keyValueType.Substring(0, typeSepIndex).Trim(), true),
            CreateType(module, keyValueType.Substring(typeSepIndex + 1).Trim(), true), isTreeMap);
    }

    TType CreateContainerType(string module, string containerType, Dictionary<string, string> containerTags, string elementType)
    {
        switch (containerType)
        {
            case "array":
            {
                return TArray.Create(false, containerTags, CreateType(module, elementType, true));
            }
            case "list":
                return TList.Create(false, containerTags, CreateType(module, elementType, true), true);
            case "set":
            {
                TType type = CreateType(module, elementType, true);
                if (type.IsCollection)
                {
                    throw new Exception("set的元素不支持容器类型");
                }
                return TSet.Create(false, containerTags, type, false);
            }
            case "map":
                return CreateMapType(module, containerTags, elementType, false);
            default:
            {
                throw new ArgumentException($"invalid container type. module:'{module}' container:'{containerType}' element:'{elementType}'");
            }
        }
    }
}
