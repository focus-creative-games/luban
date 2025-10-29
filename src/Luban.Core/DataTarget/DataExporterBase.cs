// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
