using Luban.DataLoader;
using Luban.DataLoader.Builtin.Excel;
using Luban.Datas;
using Luban.Defs;
using Luban.Encryption;
using Luban.RawDefs;
using Luban.Serialization;
using Luban.Types;
using Luban.Utils;
using NLog;

namespace Luban.L10N;

public static class TextBinaryGeneration
{
    private static ILogger s_logger;

    private const string TextDictFileOptionName = "textDictionaryFile";
    private const string OutputDictDir = "outputDictDir";
    private const string ExportExtensionOptionName = "textBinaryExtension";

    public static void Generate()
    {
        s_logger = LogManager.GetCurrentClassLogger();

        if (!EnvManager.Current.TryGetOption(BuiltinOptionNames.L10NFamily, TextDictFileOptionName, true, out string textDictionaryFile))
        {
            s_logger.Warn("not found option: '-x {0}.{1}=<textDictionaryFile>', text binary generation is disabled", BuiltinOptionNames.L10NFamily, TextDictFileOptionName);
            return;
        }
        var keyFieldName = EnvManager.Current.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, BuiltinOptionNames.TextKeyFieldName, false, "key");
        var binaryExtension = EnvManager.Current.GetOptionOrDefault(BuiltinOptionNames.L10NFamily, ExportExtensionOptionName, false, "bytes");
        var outputDictDir = EnvManager.Current.GetOption(BuiltinOptionNames.L10NFamily, OutputDictDir, false);

        s_logger.Info("process text binary:{} begin", textDictionaryFile);

        (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(textDictionaryFile));
        using var inputStream = new FileStream(actualFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var tableDefInfo = SheetLoadUtil.LoadSheetTableDefInfo(actualFile, sheetName, inputStream);

        var rawBean = new RawBean()
        {
            Namespace = "__intern__",
            Name = "__TextInfo__",
            Parent = "",
            Alias = "",
            IsValueType = false,
            Sep = "",
            Fields = new List<RawField>
            {
                new() { Name = keyFieldName, Type = "string" },
            }
        };

        var locales = new string[tableDefInfo.FieldInfos.Count - 1];
        int idx = 0;
        foreach (var (name, f) in tableDefInfo.FieldInfos)
        {
            if (name == keyFieldName)
            {
                continue;
            }

            if (f.Type != "string")
            {
                s_logger.Warn("textProviderFile:{} invalid field type: {}", textDictionaryFile, f.Name);
                continue;
            }

            locales[idx++] = name;
            rawBean.Fields.Add(new RawField() { Name = name, Type = "string" });
        }

        var ass = new DefAssembly(new RawAssembly()
        {
            Targets = new List<RawTarget> { new() { Name = "default", Manager = "Tables" } },
        }, "default", new List<string>());

        var defTableRecordType = new DefBean(rawBean)
        {
            Assembly = ass,
        };
        ass.AddType(defTableRecordType);
        defTableRecordType.PreCompile();
        defTableRecordType.Compile();
        defTableRecordType.PostCompile();
        var tableRecordType = TBean.Create(false, defTableRecordType, null);

        var records =
            DataLoaderManager.Ins.LoadTableFile(tableRecordType, actualFile, sheetName,
                new Dictionary<string, string>());

        var dict = new Dictionary<string, string[]>(records.Count);
        foreach (var r in records)
        {
            DBean data = r.Data;

            string key = ((DString)data.GetField(keyFieldName)).Value;
            string[] values = new string[locales.Length];
            for (int i = 0; i < locales.Length; i++)
            {
                values[i] = ((DString)data.GetField(locales[i])).Value;
            }

            if (dict.TryGetValue(key, out _))
            {
                s_logger.Warn("textProviderFile:{} key:{} duplicated", textDictionaryFile, key);
            }
            else
            {
                dict[key] = values;
            }
        }

        List<OutputFile> outputFiles = new();
        for (int i = 0; i < locales.Length; i++)
        {
            var locale = locales[i];
            var byteBuf = new ByteBuf();
            byteBuf.WriteSize(dict.Count);
            foreach (var kv in dict)
            {
                var source = kv.Key;
                var target = kv.Value[i];
                byteBuf.WriteString(source);
                byteBuf.WriteString(target);
            }

            outputFiles.Add(new OutputFile()
            {
                File = $"{locale}/dictionary.{binaryExtension}",
                Content = EncryptionUtil.Encrypt($"{locale}_dictionary", byteBuf.CopyData())
            });
        }

        FileCleaner.Clean(outputDictDir, new List<string>());
        var tasks = new List<Task>();
        foreach (var outputFile in outputFiles)
        {
            tasks.Add(Task.Run(() =>
            {
                string fullOutputPath = $"{outputDictDir}/{outputFile.File}";
                Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
                if (FileUtil.WriteAllBytes(fullOutputPath, outputFile.GetContentBytes()))
                {
                    s_logger.Info("save file:{} ", fullOutputPath);
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
        s_logger.Info("process text binary:{} end", textDictionaryFile);
    }
}