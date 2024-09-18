using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.DataLoader;
using Luban.Datas;
using Luban.Defs;
using Luban.L10N;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using Luban.Validator;

namespace Luban;

public class GenerationContextBuilder
{
    public DefAssembly Assembly { get; set; }

    public List<string> IncludeTags { get; set; }

    public List<string> ExcludeTags { get; set; }

    public string TimeZone { get; set; }
}

public class GenerationContext
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static GenerationContext Current { get; private set; }

    public static ICodeTarget CurrentCodeTarget { get; set; }

    public static LubanConfig GlobalConf { get; set; }

    public DefAssembly Assembly { get; private set; }

    public RawTarget Target => Assembly.Target;

    public List<string> IncludeTags { get; private set; }

    public List<string> ExcludeTags { get; private set; }

    private readonly ConcurrentDictionary<string, TableDataInfo> _recordsByTables = new();

    public string TopModule => Target.TopModule;

    public List<DefTable> Tables => Assembly.GetAllTables();

    private List<DefTypeBase> ExportTypes { get; set; }

    public List<DefTable> ExportTables { get; private set; }

    public List<DefBean> ExportBeans { get; private set; }

    public List<DefEnum> ExportEnums { get; private set; }

    public TimeZoneInfo TimeZone { get; private set; }

    public ITextProvider TextProvider { get; private set; }

    private readonly Dictionary<string, object> _uniqueObjects = new();

    private readonly HashSet<Type> _failedValidatorTypes = new();

    private bool _exportEmptyGroupsTypes;

    public void LoadDatas()
    {
        s_logger.Info("load datas begin");
        TextProvider?.Load();
        DataLoaderManager.Ins.LoadDatas(this);
        s_logger.Info("load datas end");
    }

    public GenerationContext()
    {
        Current = this;
    }

    public void Init(GenerationContextBuilder builder)
    {
        Assembly = builder.Assembly;
        IncludeTags = builder.IncludeTags;
        ExcludeTags = builder.ExcludeTags;
        if (IncludeTags != null && IncludeTags.Count != 0 && ExcludeTags != null && ExcludeTags.Count > 0)
        {
            throw new Exception("option '--includeTag <tag>' and '--excludeTag <tag>' can not be set at the same time");
        }
        TimeZone = TimeZoneUtil.GetTimeZone(builder.TimeZone);
        _exportEmptyGroupsTypes = builder.Assembly.Target.Groups.Any(g => GlobalConf.Groups.First(gd => gd.Names.Contains(g))?.IsDefault == true);

        TextProvider = EnvManager.Current.TryGetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NProviderName, false, out string providerName) ?
            L10NManager.Ins.CreateTextProvider(providerName) : null;

        ExportTables = Assembly.ExportTables;
        ExportTypes = CalculateExportTypes();
        ExportBeans = SortBeanTypes(ExportTypes.OfType<DefBean>().ToList());
        ExportEnums = ExportTypes.OfType<DefEnum>().ToList();
    }

    private void AddChildrenByOrder(List<DefBean> list, DefBean bean)
    {
        list.Add(bean);
        if (bean.Children == null || bean.Children.Count == 0)
        {
            return;
        }
        var children = new List<DefBean>(bean.Children);
        children.Sort((a, b) => a.FullName.CompareTo(b.FullName));
        foreach (var child in children)
        {
            AddChildrenByOrder(list, child);
        }
    }

    /// <summary>
    /// some languages like c++ have dependencies on the order of type definitions, so we need to sort the types here
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    private List<DefBean> SortBeanTypes(List<DefBean> types)
    {
        var sortedBeans = new List<DefBean>();
        foreach (var bean in types)
        {
            if (bean.ParentDefType == null)
            {
                AddChildrenByOrder(sortedBeans, bean);
            }
        }
        Debug.Assert(types.Count == sortedBeans.Count);
        return sortedBeans;
    }

    private bool NeedExportNotDefault(List<string> groups)
    {
        if (groups.Count == 0)
        {
            return _exportEmptyGroupsTypes;
        }
        return groups.Any(Target.Groups.Contains);
    }

    private List<DefTypeBase> CalculateExportTypes()
    {
        var refTypes = new Dictionary<string, DefTypeBase>();
        var types = Assembly.TypeList;
        foreach (var t in types)
        {
            if (!refTypes.ContainsKey(t.FullName))
            {
                if (t is DefBean bean && NeedExportNotDefault(t.Groups))
                {
                    TBean.Create(false, bean, null).Apply(RefTypeVisitor.Ins, refTypes);
                }
                else if (t is DefEnum && NeedExportNotDefault(t.Groups))
                {
                    refTypes.Add(t.FullName, t);
                }
            }
        }

        foreach (var table in ExportTables)
        {
            refTypes[table.FullName] = table;
            table.ValueTType.Apply(RefTypeVisitor.Ins, refTypes);
        }

        return refTypes.OrderBy(p => p.Key).Select(p => p.Value).ToList();
    }

    public static string GetInputDataPath()
    {
        return GlobalConf.InputDataDir;
    }

    public void AddDataTable(DefTable table, List<Record> mainRecords, List<Record> patchRecords)
    {
        s_logger.Debug("AddDataTable name:{} record count:{}", table.FullName, mainRecords.Count);
        _recordsByTables[table.FullName] = new TableDataInfo(table,
            mainRecords.Where(r => r.IsNotFiltered(IncludeTags, ExcludeTags)).ToList(),
            patchRecords != null ? patchRecords.Where(r => r.IsNotFiltered(IncludeTags, ExcludeTags)).ToList() : null);
    }

    public List<Record> GetTableAllDataList(DefTable table)
    {
        return _recordsByTables[table.FullName].FinalRecords;
    }

    public List<Record> GetTableExportDataList(DefTable table)
    {
        return _recordsByTables[table.FullName].FinalRecords;
    }

    public static List<Record> ToSortByKeyDataList(DefTable table, List<Record> originRecords)
    {
        var sortedRecords = new List<Record>(originRecords);

        DefField keyField = table.IndexField;
        if (keyField != null && (keyField.CType is TInt || keyField.CType is TLong))
        {
            string keyFieldName = keyField.Name;
            sortedRecords.Sort((a, b) =>
            {
                DType keya = a.Data.GetField(keyFieldName);
                DType keyb = b.Data.GetField(keyFieldName);
                switch (keya)
                {
                    case DInt ai:
                        return ai.Value.CompareTo((keyb as DInt).Value);
                    case DLong al:
                        return al.Value.CompareTo((keyb as DLong).Value);
                    default:
                        throw new NotSupportedException();
                }
            });
        }
        return sortedRecords;
    }

    public TableDataInfo GetTableDataInfo(DefTable table)
    {
        return _recordsByTables[table.FullName];
    }

    public ICodeStyle GetCodeStyle(string family)
    {
        if (EnvManager.Current.TryGetOption(family, BuiltinOptionNames.CodeStyle, true, out var codeStyleName))
        {
            return CodeFormatManager.Ins.GetCodeStyle(codeStyleName);
        }
        return null;
    }

    public object GetUniqueObject(string key)
    {
        lock (this)
        {
            return _uniqueObjects[key];
        }
    }

    public object TryGetUniqueObject(string key)
    {
        lock (this)
        {
            _uniqueObjects.TryGetValue(key, out var obj);
            return obj;
        }
    }

    public object GetOrAddUniqueObject(string key, Func<object> factory)
    {
        lock (this)
        {
            if (_uniqueObjects.TryGetValue(key, out var obj))
            {
                return obj;
            }
            else
            {
                obj = factory();
                _uniqueObjects.Add(key, obj);
                return obj;
            }
        }
    }

    public void LogValidatorFail(IDataValidator validator)
    {
        lock (this)
        {
            _failedValidatorTypes.Add(validator.GetType());
        }
    }

    public bool AnyValidatorFail
    {
        get
        {
            lock (this)
            {
                return _failedValidatorTypes.Count > 0;
            }
        }
    }
}
