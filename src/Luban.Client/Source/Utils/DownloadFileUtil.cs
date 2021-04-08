using Luban.Client.Common.Net;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Client.Common.Utils
{
    public static class DownloadFileUtil
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        const int DOWNLOAD_TIMTOUT = 10;

        public static async Task DownloadGeneratedFiles(string outputDir, List<FileInfo> newFiles)
        {
            List<Task> tasks = new List<Task>();
            foreach (var file in newFiles)
            {
                if (!await CacheMetaManager.Ins.CheckFileChangeAsync(outputDir, file.FilePath, file.MD5))
                {
                    continue;
                }
                tasks.Add(Task.Run(async () =>
                {
                    s_logger.Trace("new code file:{@file}", file);
                    GetOutputFileRes res = await GenClient.Ins.Session.CallRpcAsync<GetOutputFile, GetOutputFileArg, GetOutputFileRes>(new GetOutputFileArg()
                    {
                        MD5 = file.MD5,
                    }, DOWNLOAD_TIMTOUT);

                    await FileUtil.SaveFileAsync(outputDir, file.FilePath, res.FileContent);
                    await CacheMetaManager.Ins.UpdateFileAsync(outputDir, file.FilePath, file.MD5);
                }));
            }
            await Task.WhenAll(tasks);

            FileCleaner.Clean(outputDir, newFiles);
        }

        public static async Task DownloadGeneratedFile(FileInfo file)
        {
            if (!await CacheMetaManager.Ins.CheckFileChangeAsync(null, file.FilePath, file.MD5))
            {
                return;
            }
            GetOutputFileRes res = await GenClient.Ins.Session.CallRpcAsync<GetOutputFile, GetOutputFileArg, GetOutputFileRes>(new GetOutputFileArg()
            {
                MD5 = file.MD5,
            }, DOWNLOAD_TIMTOUT).ConfigureAwait(false);

            await FileUtil.SaveFileAsync(null, file.FilePath, res.FileContent);
            await CacheMetaManager.Ins.UpdateFileAsync(null, file.FilePath, file.MD5);
        }
    }
}
