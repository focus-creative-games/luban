using Bright.Time;
using Luban.Common.Utils;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.DataSources.Excel;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    public static class DataLoaderUtil
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public class InputFileInfo
        {
            public string MD5 { get; set; }

            public string OriginFile { get; set; }

            public string ActualFile { get; set; }

            public string SheetName { get; set; }
        }

        public static async Task<List<InputFileInfo>> CollectInputFilesAsync(IAgent agent, IEnumerable<string> files, string dataDir)
        {
            var collectTasks = new List<Task<List<InputFileInfo>>>();
            foreach (var file in files)
            {
                (var actualFile, var sheetName) = FileUtil.SplitFileAndSheetName(FileUtil.Standardize(file));
                var actualFullPath = FileUtil.Combine(dataDir, actualFile);
                var originFullPath = FileUtil.Combine(dataDir, file);
                //s_logger.Info("== get input file:{file} actualFile:{actual}", file, actualFile);

                collectTasks.Add(Task.Run(async () =>
                {
                    var fileOrDirContent = await agent.GetFileOrDirectoryAsync(actualFullPath, DataSourceFactory.validDataSourceSuffixes);
                    if (fileOrDirContent.IsFile)
                    {
                        return new List<InputFileInfo> { new InputFileInfo() { OriginFile = file, ActualFile = actualFullPath, SheetName = sheetName, MD5 = fileOrDirContent.Md5 } };
                    }
                    else
                    {
                        return fileOrDirContent.SubFiles.Select(f => new InputFileInfo() { OriginFile = f.FilePath, ActualFile = f.FilePath, SheetName = sheetName, MD5 = f.MD5 }).ToList();
                    }
                }));
            }

            var allFiles = new List<InputFileInfo>();
            foreach (var t in collectTasks)
            {
                allFiles.AddRange(await t);
            }
            return allFiles;
        }

        public static bool IsMultiRecordField(string sheet)
        {
            return !string.IsNullOrEmpty(sheet) && sheet.StartsWith("*");
        }

        private static bool IsMultiRecordFile(string file, string sheetOrFieldName)
        {
            return FileUtil.IsExcelFile(file) || IsMultiRecordField(sheetOrFieldName);
        }

        public static async Task GenerateLoadRecordFromFileTasksAsync(IAgent agent, DefTable table, string dataDir, List<string> inputFiles2, List<Task<List<Record>>> tasks)
        {
            var inputFileInfos = await CollectInputFilesAsync(agent, inputFiles2, dataDir);

            // check cache (table, exporttestdata) -> (list<InputFileInfo>, List<DType>)
            // (md5, sheetName,exportTestData) -> (value_type, List<DType>)

            foreach (var file in inputFileInfos)
            {
                var actualFile = file.ActualFile;

                tasks.Add(Task.Run(async () =>
                {
                    if (FileRecordCacheManager.Ins.TryGetCacheLoadedRecords(table, file.MD5, actualFile, file.SheetName, out var cacheRecords))
                    {
                        return cacheRecords;
                    }
                    var res = LoadCfgRecords(table.ValueTType,
                        file.OriginFile,
                        file.SheetName,
                        await agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5),
                        IsMultiRecordFile(file.ActualFile, file.SheetName), table.Options);

                    FileRecordCacheManager.Ins.AddCacheLoadedRecords(table, file.MD5, file.SheetName, res);

                    return res;
                }));
            }
        }

        public static async Task LoadTableAsync(IAgent agent, DefTable table, string dataDir, string patchName, string patchDataDir, string inputConvertDataDir)
        {
            var mainLoadTasks = new List<Task<List<Record>>>();

            // 如果指定了 inputConvertDataDir, 则覆盖dataDir为 inputConvertDataDir
            // 同时 修改所有表的input为 table.FullName
            string finalDataDir;
            List<string> finalInputFiles;
            if (string.IsNullOrWhiteSpace(inputConvertDataDir))
            {
                finalDataDir = dataDir;
                finalInputFiles = table.InputFiles;
            }
            else
            {
                finalDataDir = inputConvertDataDir;
                finalInputFiles = new List<string>() { table.FullName };
            }

            var mainGenerateTask = GenerateLoadRecordFromFileTasksAsync(agent, table, finalDataDir, finalInputFiles, mainLoadTasks);

            var patchLoadTasks = new List<Task<List<Record>>>();

            Task patchGenerateTask = null;
            if (!string.IsNullOrWhiteSpace(patchName))
            {
                var patchInputFiles = table.GetPatchInputFiles(patchName);
                if (patchInputFiles != null)
                {
                    patchGenerateTask = GenerateLoadRecordFromFileTasksAsync(agent, table, patchDataDir, patchInputFiles, patchLoadTasks);
                }
            }

            await mainGenerateTask;

            var mainRecords = new List<Record>(256);
            foreach (var task in mainLoadTasks)
            {
                mainRecords.AddRange(await task);
            }
            s_logger.Trace("== load main records. count:{count}", mainRecords.Count);

            List<Record> patchRecords = null;
            if (patchGenerateTask != null)
            {
                patchRecords = new List<Record>(64);
                await patchGenerateTask;
                foreach (var task in patchLoadTasks)
                {
                    patchRecords.AddRange(await task);
                }
                s_logger.Trace("== load patch records. count:{count}", patchRecords.Count);
            }

            table.Assembly.AddDataTable(table, mainRecords, patchRecords);

            s_logger.Trace("table:{name} record num:{num}", table.FullName, mainRecords.Count);
        }

        public static async Task LoadCfgDataAsync(IAgent agent, DefAssembly ass, string dataDir, string patchName, string patchDataDir, string inputConvertDataDir)
        {
            var ctx = agent;
            List<DefTable> exportTables = ass.Types.Values.Where(t => t is DefTable).Cast<DefTable>().ToList(); //&& ct.NeedExport.Select(t => (DefTable)t)
            var genDataTasks = new List<Task>();
            var outputDataFiles = new ConcurrentBag<FileInfo>();
            long genDataStartTime = Bright.Time.TimeUtil.NowMillis;

            foreach (DefTable c in exportTables)
            {
                var table = c;
                genDataTasks.Add(Task.Run(async () =>
                {
                    long beginTime = Bright.Time.TimeUtil.NowMillis;
                    await LoadTableAsync(agent, table, dataDir, patchName, patchDataDir, inputConvertDataDir);
                    long endTime = Bright.Time.TimeUtil.NowMillis;
                    if (endTime - beginTime > 100)
                    {
                        ctx.Info("====== load {0} cost {1} ms ======", table.FullName, (endTime - beginTime));
                    }
                }));
            }
            await Task.WhenAll(genDataTasks.ToArray());
        }

        public static List<Record> LoadCfgRecords(TBean recordType, string originFile, string sheetName, byte[] content, bool multiRecord, Dictionary<string, string> options)
        {
            // (md5,sheet,multiRecord,exportTestData) -> (valuetype, List<(datas)>)
            var dataSource = DataSourceFactory.Create(originFile, sheetName, options, new MemoryStream(content));
            try
            {
                if (multiRecord)
                {
                    return dataSource.ReadMulti(recordType);
                }
                else
                {
                    Record record = dataSource.ReadOne(recordType);
                    return record != null ? new List<Record> { record } : new List<Record>();
                }
            }
            catch (DataCreateException dce)
            {
                if (string.IsNullOrWhiteSpace(dce.OriginDataLocation))
                {
                    dce.OriginDataLocation = originFile;
                }
                throw;
            }
            catch (Exception e)
            {
                throw new Exception($"配置文件:{originFile} 生成失败.", e);
            }
        }

        public static async Task LoadTextTablesAsync(IAgent agent, DefAssembly ass, string baseDir, string textTableFiles)
        {
            var tasks = new List<Task<byte[]>>();
            var files = textTableFiles.Split(',');
            foreach (var file in await CollectInputFilesAsync(agent, files, baseDir))
            {
                tasks.Add(agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5));
            }

            var textTable = ass.ExportTextTable;
            for (int i = 0; i < tasks.Count; i++)
            {
                var bytes = await tasks[i];
                try
                {
                    textTable.LoadFromFile(files[i], bytes);
                }
                catch (Exception e)
                {
                    throw new Exception($"load text table file:{files[i]} fail", e);
                }
            }
        }

    }
}
