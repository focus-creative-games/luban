using Luban.DataLoader;
using Luban.Datas;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;

namespace Luban.L10N;

[TextProvider("default")]
public class DefaultTextProvider : ITextProvider
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _keyFieldName;
    private readonly HashSet<string> _keys = new();
    public void Load()
    {
        if (!EnvManager.Current.TryGetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextProviderFile, true,
                out string textProviderFile))
        {
            s_logger.Error("not found option:{}.{}, text validation will not work", BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextProviderFile);
            return;
        }
        _keyFieldName = EnvManager.Current.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextKeyFieldName, false, "key");
        LoadTextListFromFile(textProviderFile);
    }

    public bool IsValidKey(string key)
    {
        return _keys.Contains(key);
    }

    public string GetText(string key, string language)
    {
        throw new NotSupportedException("default text provider not support get text");
    }
    
    private void LoadTextListFromFile(string fileName)
    {
        var ass = new DefAssembly(new RawAssembly()
        {
            Targets = new List<RawTarget>{new() { Name = "default", Manager = "Tables"}},
        }, "default", new List<string>());
        
        var defTableRecordType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__TextInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = _keyFieldName, Type = "string" },
            }
        })
        {
            Assembly = ass,
        };
        ass.AddType(defTableRecordType);
        defTableRecordType.PreCompile();
        defTableRecordType.Compile();
        defTableRecordType.PostCompile();
        var tableRecordType = TBean.Create(false, defTableRecordType, null);
        
        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(fileName));
        var records = DataLoaderManager.Ins.LoadTableFile(tableRecordType, actualFile, sheetName, new Dictionary<string, string>());

        foreach (var r in records)
        {
            DBean data = r.Data;

            string key = ((DString)data.GetField(_keyFieldName)).Value;
            if (!_keys.Add(key))
            {
                s_logger.Warn("textProviderFile:{} key:{} duplicated", fileName, key);
            }
        };
    }
}