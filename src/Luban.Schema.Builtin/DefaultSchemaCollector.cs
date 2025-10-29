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
            string ext = FileUtil.GetExtensionWithoutDot(importFile.FileName);
            if (string.IsNullOrEmpty(ext))
            {
                throw new Exception($"schema file:'{importFile.FileName}' has no extension. luban doesn't know how to load file without extension.");
            }
            var schemaLoader = SchemaManager.Ins.CreateSchemaLoader(ext, importFile.Type, this);
            schemaLoader.Load(importFile.FileName);
        }

        LoadTablesFromTableImporter();
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
                RawBean bean = schemaLoader.Load(fullPath, table.ValueType, table);
                bean.Groups = new List<string>(table.Groups);
                Add(bean);
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }

    private void LoadTablesFromTableImporter()
    {
        string tableImporterName = EnvManager.Current.GetOptionOrDefault("tableImporter", "name", false, "default");
        if (string.IsNullOrWhiteSpace(tableImporterName) || tableImporterName == "none")
        {
            return;
        }
        ITableImporter tableImporter = SchemaManager.Ins.CreateTableImporter(tableImporterName);
        foreach (var table in tableImporter.LoadImportTables())
        {
            Add(table);
        }
    }
}
