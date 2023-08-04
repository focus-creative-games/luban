using Luban.Core;
using Luban.Core.Schema;
using Luban.Core.Utils;

namespace Luban.Schema.Default;

[SchemaCollector("default")]
public class DefaultSchemaCollector : SchemaCollectorBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public DefaultSchemaCollector()
    {
        
    }

    public override void Load(string schemaPath)
    {
        var rootLoader = (IRootSchemaLoader)SchemaLoaderManager.Ins.Create(FileUtil.GetExtensionWithDot(schemaPath), "root");
        rootLoader.Load(schemaPath, this);

        foreach (var importFile in rootLoader.ImportFiles)
        {
            s_logger.Debug("import schema file:{} type:{}", importFile.FileName, importFile.Type);
            var schemaLoader = SchemaLoaderManager.Ins.Create(FileUtil.GetExtensionWithDot(importFile.FileName), importFile.Type);
            schemaLoader.Load(importFile.FileName, this);
        }
    }
}