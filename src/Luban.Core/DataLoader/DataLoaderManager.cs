using System.Reflection;
using Luban.Core.Defs;
using Luban.Core.Utils;

namespace Luban.Core.DataLoader;

public class DataLoaderManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    
    public static DataLoaderManager Ins { get; } = new();

    private readonly Dictionary<string, Func<IDataLoader>> _dataLoaderCreator = new();

    public void Init()
    {
        
    }

    public void LoadDatas(GenerationContext ctx)
    {
        var tasks = ctx.Tables.Select(t => Task.Run(() => LoadTable(ctx, t))).ToArray();
        Task.WaitAll(tasks);
    }

    private void LoadTable(GenerationContext ctx, DefTable table)
    {
        string inputDataDir = ctx.GetInputDataPath();
        var tasks = new List<Task<List<Record>>>();
        foreach (var inputFile in table.InputFiles)
        {
            (var actualFile, var subAssetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(inputFile));
            var options = new Dictionary<string, string>();
            foreach (var atomFile in FileUtil.GetFileOrDirectory(Path.Combine(inputDataDir, actualFile)))
            {
                tasks.Add(Task.Run(() => LoadTableFile(ctx, table, atomFile, subAssetName, options)));
            }
        }

        var records = new List<Record>();
        foreach (var task in tasks)
        {
            records.AddRange(task.Result);
        }
        ctx.AddDataTable(table, records, null);
    }
    
    private List<Record> LoadTableFile(GenerationContext ctx, DefTable table, string file, string subAssetName, Dictionary<string, string> options)
    {
        string loaderName = options.TryGetValue("loader", out var name) ? name : FileUtil.GetExtensionWithDot(file);
        var loader = CreateDataLoader(loaderName);
        using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        loader.Load(table, file, subAssetName, stream);
        if (IsMultiRecordFile(file, subAssetName))
        {
            return loader.ReadMulti(table, table.ValueTType);
        }
        return new List<Record> { loader.ReadOne(table, table.ValueTType) };
    }
    
    
    private static bool IsMultiRecordField(string sheet)
    {
        return !string.IsNullOrEmpty(sheet) && sheet.StartsWith("*");
    }
    
    private static bool IsMultiRecordFile(string file, string sheetOrFieldName)
    {
        return FileUtil.IsExcelFile(file) || IsMultiRecordField(sheetOrFieldName);
    }

    public IDataLoader CreateDataLoader(string loaderName)
    {
        if (_dataLoaderCreator.TryGetValue(loaderName, out var loaderCreator))
        {
            return loaderCreator();
        }
        throw new Exception($"data loader:{loaderName} not exists");
    }
    
    public void RegisterDataLoader(string loaderName, Func<IDataLoader> creator)
    {
        if (!_dataLoaderCreator.TryAdd(loaderName, creator))
        {
            s_logger.Error("duplicate data source loader:{loaderName}", loaderName);
            return;
        }
    }

    public void ScanRegisterDataLoader(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }
            if (typeof(IDataLoader).IsAssignableFrom(type))
            {
                var attr = type.GetCustomAttribute<DataLoaderAttribute>();
                if (attr == null)
                {
                    continue;
                }

                foreach (var loaderName in attr.LoaderNames)
                {
                    if (!_dataLoaderCreator.TryAdd(loaderName, () => (IDataLoader)Activator.CreateInstance(type)))
                    {
                        s_logger.Error("duplicate data source loader:{loaderName}", loaderName);
                    }
                }
            }
        }
    }
}