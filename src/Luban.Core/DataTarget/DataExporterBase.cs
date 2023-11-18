using Luban.Defs;

namespace Luban.DataTarget;

public abstract class DataExporterBase : IDataExporter
{
    public const string FamilyPrefix = "dataExporter";


    public virtual void Handle(GenerationContext ctx, IDataTarget dataTarget, OutputFileManifest manifest)
    {
        List<DefTable> tables = dataTarget.ExportAllRecords ? ctx.Tables : ctx.ExportTables;
        switch (dataTarget.AggregationType)
        {
            case AggregationType.Table:
            {
                var tasks = tables.Select(table => Task.Run(() =>
                {
                    manifest.AddFile(dataTarget.ExportTable(table, ctx.GetTableExportDataList(table)));
                })).ToArray();
                Task.WaitAll(tasks);
                break;
            }
            case AggregationType.Tables:
            {
                manifest.AddFile(dataTarget.ExportTables(ctx.ExportTables));
                break;
            }
            case AggregationType.Record:
            {
                var tasks = new List<Task>();
                foreach (var table in tables)
                {
                    foreach (var record in ctx.GetTableExportDataList(table))
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            manifest.AddFile(dataTarget.ExportRecord(table, record));
                        }));
                    }
                }
                Task.WaitAll(tasks.ToArray());
                break;
            }
            case AggregationType.Other:
            {
                ExportCustom(tables, manifest, dataTarget);
                break;
            }
        }
    }

    protected virtual void ExportCustom(List<DefTable> tables, OutputFileManifest manifest, IDataTarget dataTarget)
    {

    }
}
