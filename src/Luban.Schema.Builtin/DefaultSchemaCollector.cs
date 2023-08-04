using Luban.RawDefs;
using Luban.Utils;

namespace Luban.Schema.Builtin;

[SchemaCollector("default")]
public class DefaultSchemaCollector : SchemaCollectorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public DefaultSchemaCollector()
    {
        
    }

    public override void Load(string schemaPath)
    {
        var rootLoader = (IRootSchemaLoader)SchemaLoaderManager.Ins.Create(FileUtil.GetExtensionWithDot(schemaPath), "root", this, null);
        rootLoader.Load(schemaPath);

        foreach (var importFile in rootLoader.ImportFiles)
        {
            s_logger.Debug("import schema file:{} type:{}", importFile.FileName, importFile.Type);
            var schemaLoader = SchemaLoaderManager.Ins.Create(FileUtil.GetExtensionWithDot(importFile.FileName), importFile.Type, this, null);
            schemaLoader.Load(importFile.FileName);
        }
        
        LoadTableValueTypeSchemasFromFile();
    }
    
    private void LoadTableValueTypeSchemasFromFile()
    {
        var tasks = new List<Task<RawBean>>();
        foreach (var table in GenerationContext.Current.Tables.Where(t => t.ReadSchemaFromFile))
        {
            string fileName = table.InputFiles[0];
            ISchemaLoader schemaLoader = SchemaLoaderManager.Ins.Create(FileUtil.GetExtensionWithDot(fileName), "table-valueType", this, table.ValueType);
            schemaLoader.Load(fileName);
        }
        Task.WaitAll(tasks.ToArray());
    }
}