using Luban.DataLoader;
using Luban.Datas;
using Luban.Defs;
using Luban.RawDefs;
using Luban.Types;
using Luban.Utils;
using System.Linq;

namespace Luban.L10N;

[TextProvider("default")]
public class DefaultTextProvider : ITextProvider
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _keyFieldName;
    private string _ValueFieldName;

    private bool _convertTextKeyToValue;

    private readonly Dictionary<string, string> _texts = new();

    private readonly HashSet<string> _unknownTextKeys = new();

    public void Load()
    {
        EnvManager env = EnvManager.Current;

        _keyFieldName = env.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextFileKeyFieldName, false, "");
        if (string.IsNullOrWhiteSpace(_keyFieldName))
        {
            throw new Exception($"'-x {BuiltinOptionNames.L10NFamily}.{BuiltinOptionNames.L10NTextFileKeyFieldName}=xxx' missing");
        }

        _convertTextKeyToValue = DataUtil.ParseBool(env.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NConvertTextKeyToValue, false, "false"));
        if (_convertTextKeyToValue)
        {
            _ValueFieldName = env.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextFileLanguageFieldName, false, "");
            if (string.IsNullOrWhiteSpace(_ValueFieldName))
            {
                throw new Exception($"'-x {BuiltinOptionNames.L10NFamily}.{BuiltinOptionNames.L10NTextFileLanguageFieldName}=xxx' missing");
            }
        }

        string textProviderFile = env.GetOption(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.L10NTextFilePath, false);
        LoadTextListFromFile(textProviderFile);
    }

    public bool ConvertTextKeyToValue => _convertTextKeyToValue;

    public bool IsValidKey(string key)
    {
        return _texts.ContainsKey(key);
    }

    public bool TryGetText(string key, out string text)
    {
        return _texts.TryGetValue(key, out text);
    }

    private void LoadTextListFromFile(string fileName)
    {
        var ass = new DefAssembly(new RawAssembly()
        {
            Targets = new List<RawTarget> { new() { Name = "default", Manager = "Tables" } },
        }, "default", new List<string>(), null, null);


        var rawFields = new List<RawField> { new() { Name = _keyFieldName, Type = "string" }, };
        if (_convertTextKeyToValue)
        {
            rawFields.Add(new() { Name = _ValueFieldName, Type = "string" });
        }
        var defTableRecordType = new DefBean(new RawBean()
        {
            Namespace = "__intern__",
            Name = "__TextInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = rawFields,
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
            string value = _convertTextKeyToValue ? ((DString)data.GetField(_ValueFieldName)).Value : key;
            if (string.IsNullOrEmpty(key))
            {
                s_logger.Error("textFile:{} key:{} is empty. ignore it!", fileName, key);
                continue;
            }
            if (!_texts.TryAdd(key, value))
            {
                s_logger.Error("textFile:{} key:{} is duplicated", fileName, key);
            }
        };
    }

    public void AddUnknownKey(string key)
    {
        _unknownTextKeys.Add(key);
    }

    public void ProcessDatas()
    {
        if (_convertTextKeyToValue)
        {
            var trans = new TextKeyToValueTransformer(this);
            foreach (var table in GenerationContext.Current.Tables)
            {
                foreach (var record in GenerationContext.Current.GetTableAllDataList(table))
                {
                    record.Data = (DBean)record.Data.Apply(trans,table.ValueTType);
                }
            }
        }
    }
}
