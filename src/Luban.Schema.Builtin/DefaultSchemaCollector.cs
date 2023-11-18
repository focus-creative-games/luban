using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaCollector("default")]
public class DefaultSchemaCollector : SchemaCollectorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private LubanConfig _config;

    public override void Load(LubanConfig config)
    {
        _config = config;

        foreach (var importFile in _config.Imports)
        {
            s_logger.Debug("import schema file:{} type:{}", importFile.FileName, importFile.Type);
            var schemaLoader = SchemaManager.Ins.CreateSchemaLoader(FileUtil.GetExtensionWithDot(importFile.FileName), importFile.Type, this);
            schemaLoader.Load(importFile.FileName);
        }

        LoadTableValueTypeSchemasFromFile();
    }

    public override RawAssembly CreateRawAssembly()
    {
        return CreateRawAssembly(_config);
    }

    private void LoadTableValueTypeSchemasFromFile()
    {
        var tasks = new List<Task>();
        string beanSchemaLoaderName = EnvManager.Current.GetOptionOrDefault(BuiltinOptionNames.SchemaCollectorFamily, "beanSchemaLoader", true, "default");
        foreach (var table in Tables.Where(t => t.ReadSchemaFromFile))
        {
            tasks.Add(Task.Run(() =>
            {
                string fileName = table.InputFiles[0];
                IBeanSchemaLoader schemaLoader = SchemaManager.Ins.CreateBeanSchemaLoader(beanSchemaLoaderName);
                string fullPath = $"{GenerationContext.GetInputDataPath()}/{fileName}";
                RawBean bean = schemaLoader.Load(fullPath, table.ValueType);
                Add(bean);
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }
}
