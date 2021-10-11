
using Bright.Time;
using Luban.Common.Protos;
using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luban.Server.Common
{
    public class LocalAgent : IAgent
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public LocalAgent()
        {
        }

        public Task<byte[]> ReadAllBytesAsync(string file)
        {
            return FileUtil.ReadAllBytesAsync(file);
        }

        public async Task<byte[]> GetFromCacheOrReadAllBytesAsync(string file, string md5)
        {
            var content = await ReadAllBytesAsync(file).ConfigureAwait(false);
            return content;
        }


        public async Task<GetImportFileOrDirectoryRes> GetFileOrDirectoryAsync(string file, params string[] searchPatterns)
        {
            long t1 = TimeUtil.NowMillis;
            var re = new GetImportFileOrDirectoryRes()
            {
                SubFiles = new List<Luban.Common.Protos.FileInfo>(),
            };
            var suffixes = new List<string>(searchPatterns.Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)));

            try
            {
                if (Directory.Exists(file))
                {
                    re.Err = 0;
                    re.IsFile = false;
                    foreach (var subFile in Directory.GetFiles(file, "*", SearchOption.AllDirectories))
                    {
                        if (FileUtil.IsValidInputFile(subFile) && (suffixes.Count == 0 || suffixes.Any(s => subFile.EndsWith(s))))
                        {

                            var md5 = FileUtil.CalcMD5(await FileUtil.ReadAllBytesAsync(subFile));
                            re.SubFiles.Add(new Luban.Common.Protos.FileInfo() { FilePath = FileUtil.Standardize(subFile), MD5 = md5 });
                        }
                    }

                }
                else if (File.Exists(file))
                {
                    re.IsFile = true;
                    re.Md5 = FileUtil.CalcMD5(await FileUtil.ReadAllBytesAsync(file));
                }
                else
                {
                    re.Err = Luban.Common.EErrorCode.FILE_OR_DIR_NOT_EXISTS;
                }
            }
            catch (Exception e)
            {
                re.Err = Luban.Common.EErrorCode.READ_FILE_FAIL;
                s_logger.Error(e);
            }

            s_logger.Trace(" GetImportFileOrDirectory file:{file} err:{err} cost:{time}", file, re.Err, TimeUtil.NowMillis - t1);

            return re;
        }

        public Task<QueryFilesExistsRes> QueryFileExistsAsync(QueryFilesExistsArg arg)
        {
            var re = new QueryFilesExistsRes() { Exists = new List<bool>(arg.Files.Count) };
            foreach (var f in arg.Files)
            {
                re.Exists.Add(File.Exists(Path.Combine(arg.Root, f)));
            }
            return Task.FromResult(re);
        }

        public async Task<XElement> OpenXmlAsync(string xmlFile)
        {
            try
            {
                s_logger.Trace("open {xml}", xmlFile);
                return XElement.Load(new MemoryStream(await ReadAllBytesAsync(xmlFile)));
            }
            catch (Exception e)
            {
                throw new Exception($"打开定义文件:{xmlFile} 失败 --> {e.Message}");
            }
        }

        #region log

        public void Error(string fmt, params object[] objs)
        {
            Log("error", string.Format(fmt, objs));
        }

        public void Info(string fmt, params object[] objs)
        {
            Log("info", string.Format(fmt, objs));
        }

        private void Log(string level, string content)
        {
            //Session.Send(new PushLog() { Level = level, LogContent = content });
        }
        #endregion
    }
}
