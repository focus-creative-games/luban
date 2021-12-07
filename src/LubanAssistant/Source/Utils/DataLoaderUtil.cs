using Bright.Time;
using Luban.Common.Utils;
using Luban.Job.Cfg.DataCreators;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
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
        //private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

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

        public static async Task GenerateLoadRecordFromFileTasksAsync(IAgent agent, DefTable table, string dataDir, List<string> inputFiles2, List<Task<List<Record>>> tasks)
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
                    var res = LoadCfgRecords(table.ValueTType,
                        file.OriginFile,
                        file.SheetName,
                        await agent.GetFromCacheOrReadAllBytesAsync(file.ActualFile, file.MD5),
                        FileUtil.IsExcelFile(file.ActualFile));

                    return res;
                }));
            }
        }

        public static async Task<TableDataInfo> LoadTableAsync(IAgent agent, DefTable table, string dataDir, string patchName, string patchDataDir)
        {
            var mainLoadTasks = new List<Task<List<Record>>>();
            var mainGenerateTask = GenerateLoadRecordFromFileTasksAsync(agent, table, dataDir, table.InputFiles, mainLoadTasks);

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
            //s_logger.Trace("== load main records. count:{count}", mainRecords.Count);

            List<Record> patchRecords = null;
            if (patchGenerateTask != null)
            {
                patchRecords = new List<Record>(64);
                await patchGenerateTask;
                foreach (var task in patchLoadTasks)
                {
                    patchRecords.AddRange(await task);
                }
                //s_logger.Trace("== load patch records. count:{count}", patchRecords.Count);
            }

            return new TableDataInfo(table, mainRecords, patchRecords);
        }

        public static async Task LoadCfgDataAsync(IAgent agent, DefAssembly ass, string dataDir, string patchName, string patchDataDir)
        {
            var ctx = agent;
            List<DefTable> exportTables = ass.Types.Values.Where(t => t is DefTable ct && ct.NeedExport).Select(t => (DefTable)t).ToList();
            var genDataTasks = new List<Task>();
            var outputDataFiles = new ConcurrentBag<FileInfo>();
            long genDataStartTime = TimeUtil.NowMillis;

            foreach (DefTable c in exportTables)
            {
                var table = c;
                genDataTasks.Add(Task.Run(async () =>
                {
                    long beginTime = TimeUtil.NowMillis;
                    await LoadTableAsync(agent, table, dataDir, patchName, patchDataDir);
                    long endTime = TimeUtil.NowMillis;
                    if (endTime - beginTime > 100)
                    {
                        ctx.Info("====== load {0} cost {1} ms ======", table.FullName, (endTime - beginTime));
                    }
                }));
            }
            await Task.WhenAll(genDataTasks.ToArray());
        }

        public static List<Record> LoadCfgRecords(TBean recordType, string originFile, string sheetName, byte[] content, bool multiRecord)
        {
            // (md5,sheet,multiRecord,exportTestData) -> (valuetype, List<(datas)>)
            var dataSource = DataSourceFactory.Create(originFile, sheetName, new MemoryStream(content));
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

        public static async Task<DefAssembly> LoadDefAssemblyAsync(string rootDefineFile, string inputDataDir)
        {
            IAgent agent = new LocalAgent();
            var loader = new CfgDefLoader(agent);
            await loader.LoadAsync(rootDefineFile);
            await loader.LoadDefinesFromFileAsync(inputDataDir);

            var rawDefines = loader.BuildDefines();

            TimeZoneInfo timeZoneInfo = null;

            var excludeTags = new List<string>();
            var ass = new DefAssembly("", timeZoneInfo, excludeTags, agent);

            ass.Load(rawDefines, agent, new GenArgs()
            {
                Service = "all",
            });

            DefAssemblyBase.LocalAssebmly = ass;
            return ass;
        }

        public static async Task<DefTable> LoadTableDefAsync(string rootDefineFile, string inputDataDir, string tableName)
        {
            DefAssembly ass = await LoadDefAssemblyAsync(rootDefineFile, inputDataDir);

            var table = ass.GetCfgTable(tableName);

            if (table == null)
            {
                throw new Exception($"table:{tableName}不存在");
            }
            return table;
        }

        public static async Task<TableDataInfo> LoadTableDataAsync(string rootDefineFile, string inputDataDir, string tableName)
        {
            var table = await LoadTableDefAsync(rootDefineFile, inputDataDir, tableName);
            var tableDataInfo = await LoadTableAsync(table.Assembly.Agent, table, inputDataDir, "", "");
            //MessageBox.Show($"table:{table.FullName} input:{StringUtil.CollectionToString(table.InputFiles)} record num:{datas.Count}");
            return tableDataInfo;
        }

    }
}
