using Bright.Time;
using Luban.Common.Utils;
using Luban.Job.Cfg.Cache;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static async Task<List<InputFileInfo>> CollectInputFilesAsync(RemoteAgent agent, IEnumerable<string> files, string dataDir)
        {
            var collectTasks = new List<Task<List<InputFileInfo>>>();
            foreach (var file in files)
            {
                (var actualFile, var sheetName) = RenderFileUtil.SplitFileAndSheetName(FileUtil.Standardize(file));
                var actualFullPath = FileUtil.Combine(dataDir, actualFile);
                var originFullPath = FileUtil.Combine(dataDir, file);
                //s_logger.Info("== get input file:{file} actualFile:{actual}", file, actualFile);

                collectTasks.Add(Task.Run(async () =>
                {
                    var fileOrDirContent = await agent.GetFileOrDirectoryAsync(actualFullPath);
                    if (fileOrDirContent.IsFile)
                    {
                        return new List<InputFileInfo> { new InputFileInfo() { OriginFile = file, ActualFile = actualFullPath, SheetName = sheetName, MD5 = fileOrDirContent.Md5 } };
                    }
                    else
                    {
                        return fileOrDirContent.SubFiles.Select(f => new InputFileInfo() { OriginFile = f.FilePath, ActualFile = f.FilePath, MD5 = f.MD5 }).ToList();
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

        //private async Task<List<InputFileInfo>> CollectInputFilesAsync(RemoteAgent agent, DefTable table, string dataDir)
        //{
        //    var collectTasks = new List<Task<List<InputFileInfo>>>();
        //    foreach (var file in table.InputFiles)
        //   return CollectInputFilesAsync(agent, table.InputFiles, dataDir)
        //}

        public static async Task GenerateLoadRecordFromFileTasksAsync(RemoteAgent agent, DefTable table, string dataDir, List<string> inputFiles2, bool exportTestData, List<Task<List<Record>>> tasks)
        {
            var inputFileInfos = await CollectInputFilesAsync(agent, inputFiles2, dataDir);

            // check cache (table, exporttestdata) -> (list<InputFileInfo>, List<DType>)
            // (md5, sheetName,exportTestData) -> (value_type, List<DType>)

            foreach (var file in inputFileInfos)
            {
                var actualFile = file.ActualFile;
                //s_logger.Info("== get input file:{file} actualFile:{actual}", file, actualFile);

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
                        RenderFileUtil.IsExcelFile(file.ActualFile),
                        exportTestData);

                    FileRecordCacheManager.Ins.AddCacheLoadedRecords(table, file.MD5, file.SheetName, res);

                    return res;
                }));
            }
        }

        public static async Task LoadTableAsync(RemoteAgent agent, DefTable table, string dataDir, string branchName, string branchDataDir, bool exportTestData)
        {
            var mainLoadTasks = new List<Task<List<Record>>>();
            var mainGenerateTask = GenerateLoadRecordFromFileTasksAsync(agent, table, dataDir, table.InputFiles, exportTestData, mainLoadTasks);

            var branchLoadTasks = new List<Task<List<Record>>>();

            Task branchGenerateTask = null;
            if (!string.IsNullOrWhiteSpace(branchName))
            {
                var branchInputFiles = table.GetBranchInputFiles(branchName);
                if (branchInputFiles != null)
                {
                    branchGenerateTask = GenerateLoadRecordFromFileTasksAsync(agent, table, branchDataDir, branchInputFiles, exportTestData, branchLoadTasks);
                }
            }

            await mainGenerateTask;

            var mainRecords = new List<Record>(256);
            foreach (var task in mainLoadTasks)
            {
                mainRecords.AddRange(await task);
            }
            s_logger.Trace("== load main records. count:{count}", mainRecords.Count);

            List<Record> branchRecords = null;
            if (branchGenerateTask != null)
            {
                branchRecords = new List<Record>(64);
                await branchGenerateTask;
                foreach (var task in branchLoadTasks)
                {
                    branchRecords.AddRange(await task);
                }
                s_logger.Trace("== load branch records. count:{count}", branchRecords.Count);
            }

            table.Assembly.AddDataTable(table, mainRecords, branchRecords);

            s_logger.Trace("table:{name} record num:{num}", table.FullName, mainRecords.Count);
        }

        public static async Task LoadCfgDataAsync(RemoteAgent agent, DefAssembly ass, string dataDir, string branchName, string branchDataDir, bool exportTestData)
        {
            var ctx = agent;
            List<DefTable> exportTables = ass.Types.Values.Where(t => t is DefTable ct && ct.NeedExport).Select(t => (DefTable)t).ToList();
            var genDataTasks = new List<Task>();
            var outputDataFiles = new ConcurrentBag<FileInfo>();
            long genDataStartTime = TimeUtil.NowMillis;

            foreach (DefTable c in exportTables)
            {
                genDataTasks.Add(Task.Run(async () =>
                {
                    long beginTime = TimeUtil.NowMillis;
                    await LoadTableAsync(agent, c, dataDir, branchName, branchDataDir, exportTestData);
                    long endTime = TimeUtil.NowMillis;
                    if (endTime - beginTime > 100)
                    {
                        ctx.Info("====== load {0} cost {1} ms ======", c.FullName, (endTime - beginTime));
                    }
                }));
            }
            await Task.WhenAll(genDataTasks.ToArray());
        }

        public static List<Record> LoadCfgRecords(TBean recordType, string originFile, string sheetName, byte[] content, bool multiRecord, bool exportTestData)
        {
            // (md5,sheet,multiRecord,exportTestData) -> (valuetype, List<(datas)>)
            var dataSource = DataSourceFactory.Create(originFile, sheetName, new MemoryStream(content), exportTestData);
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
                throw new Exception($"配置文件:{originFile} 生成失败. ==> {e.Message}", e);
            }
        }

        public static async Task LoadTextTablesAsync(RemoteAgent agent, DefAssembly ass, string baseDir, string textTableFiles)
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
                    throw new Exception($"load text table file:{files[i]} fail. ==> {e.Message} ");
                }
            }
        }

    }
}
