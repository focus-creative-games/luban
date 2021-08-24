using Luban.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Client.Common.Utils
{
    public class CacheMetaManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static CacheMetaManager Ins { get; } = new CacheMetaManager();


        class MetaInfo
        {
            public string FullPath { get; set; }

            public string Md5 { get; set; }

            public long FileLength { get; set; }

            public long FileModifiedTime { get; set; }

            public bool Visited { get; set; }
        }

        private readonly object _lock = new object();

        private string _metaFile;
        private volatile bool _dirty;
        private readonly SortedDictionary<string, MetaInfo> _cacheFileMetas = new SortedDictionary<string, MetaInfo>();

        public void Load(string metaFile)
        {
            _metaFile = metaFile;
            _dirty = false;

            try
            {
                if (File.Exists(metaFile))
                {
                    foreach (string line in File.ReadAllLines(metaFile, Encoding.UTF8))
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }
                        string[] args = line.Split(',');
                        if (args.Length != 4)
                        {
                            _dirty = true;
                            s_logger.Error("corrupt line:{line}", line);
                            continue;
                        }
                        var fullPath = args[0];
                        string md5 = args[1];
                        long fileLength = long.Parse(args[2]);
                        long fileModifiedTime = long.Parse(args[3]);
                        if (!File.Exists(fullPath))
                        {
                            _dirty = true;
                            s_logger.Debug("[drop] cache file:{file} not exist.", fullPath);
                            continue;
                        }

                        var fileInfo = new FileInfo(fullPath);

                        long actualFileLength = fileInfo.Length;
                        if (actualFileLength != fileLength)
                        {
                            _dirty = true;
                            s_logger.Debug("[drop] cache file:{file} length not match. cache length:{cl} actual length:{al}", fullPath, fileLength, actualFileLength);
                            continue;
                        }
                        long actualLastModifiedTime = ((DateTimeOffset)fileInfo.LastWriteTime).ToUnixTimeMilliseconds();
                        if (actualLastModifiedTime != fileModifiedTime)
                        {
                            _dirty = true;
                            s_logger.Debug("[drop] cache file:{file} last modified time not match. cache:{cl} actual:{al}", fullPath, fileModifiedTime, actualLastModifiedTime);
                            continue;
                        }
                        _cacheFileMetas[fullPath] = new MetaInfo
                        {
                            FullPath = fullPath,
                            Md5 = md5,
                            FileLength = fileLength,
                            FileModifiedTime = fileModifiedTime,
                        };
                        s_logger.Debug("load cache. file:{file} md5:{md5} length:{length} last modified time:{time}", fullPath, md5, fileLength, fileModifiedTime);
                    }
                }
                else
                {
                    s_logger.Info("meta file:{meta} not exist. ignore load", metaFile);
                }
            }
            catch (Exception e)
            {
                s_logger.Error(e, "load meta file fail");
            }

        }

        public void Save()
        {
            if (!_dirty)
            {
                return;
            }

            lock (_lock)
            {
                _dirty = false;
                var content = _cacheFileMetas.Values.Select(meta => $"{meta.FullPath},{meta.Md5},{meta.FileLength},{meta.FileModifiedTime}");
                File.WriteAllLines(_metaFile, content, Encoding.UTF8);
            }
            s_logger.Info("[Save] meta file:{metaFile} updated!", _metaFile);
        }

        public void Reset()
        {
            lock (_lock)
            {
                _dirty = false;
                _cacheFileMetas.Clear();
            }
        }

        private MetaInfo GetMetaInfo(string file)
        {
            lock (_lock)
            {
                var fullPath = Path.GetFullPath(file).Replace('\\', '/');
                return _cacheFileMetas.TryGetValue(fullPath, out var meta) ? meta : null;
            }
        }

        private static async Task<MetaInfo> BuildMetaInfo(string file, string md5 = null)
        {
            var fullPath = Path.GetFullPath(file).Replace('\\', '/');
            if (md5 == null)
            {
                s_logger.Info("comput md5. file:{file}", file);
                md5 = FileUtil.CalcMD5(await FileUtil.ReadAllBytesAsync(file));
            }
            var fileInfo = new FileInfo(fullPath);
            long actualFileLength = fileInfo.Length;
            long actualLastModifiedTime = ((DateTimeOffset)fileInfo.LastWriteTime).ToUnixTimeMilliseconds();
            return new MetaInfo()
            {
                FullPath = fullPath,
                Md5 = md5,
                FileLength = actualFileLength,
                FileModifiedTime = actualLastModifiedTime,
            };
        }

        public async Task<string> GetOrUpdateFileMd5Async(string file)
        {
            var meta = GetMetaInfo(file);

            if (meta == null)
            {
                meta = await BuildMetaInfo(file);
                lock (_lock)
                {
                    _dirty = true;
                    _cacheFileMetas[meta.FullPath] = meta;
                }
                s_logger.Debug("[add] meta not find, build it. file:{file} path:{path} md5:{md5} length:{length}", file, meta.FullPath, meta.Md5, meta.FileLength);
            }
            else
            {
                s_logger.Debug("[cache hit] file:{file} path:{path} md5:{md5} length:{length}", file, meta.FullPath, meta.Md5, meta.FileLength);
            }
            return meta.Md5;
        }

        public async Task<bool> CheckFileChangeAsync(string relateDir, string filePath, string md5)
        {
            var outputPath = relateDir != null ? FileUtil.Combine(relateDir, filePath) : filePath;

            var meta = GetMetaInfo(outputPath);
            if (meta == null)
            {
                if (!File.Exists(outputPath))
                {
                    return true;
                }
                meta = await BuildMetaInfo(outputPath);
                lock (_lock)
                {
                    _dirty = true;
                    _cacheFileMetas[meta.FullPath] = meta;
                }
                s_logger.Debug("[add] meta not find, create it. file:{file} path:{path} md5:{md5} length:{length}", outputPath, meta.FullPath, meta.Md5, meta.FileLength);
            }
            if (meta.Md5 != md5)
            {
                s_logger.Debug("[add] meta md5 not match,  file:{file} path:{path} md5:{md5} length:{length}", outputPath, meta.FullPath, meta.Md5, meta.FileLength);
                return true;
            }
            return false;
        }

        public async Task UpdateFileAsync(string relateDir, string filePath, string md5)
        {
            var file = relateDir != null ? FileUtil.Combine(relateDir, filePath) : filePath;
            var meta = await BuildMetaInfo(file, md5);
            lock (_lock)
            {
                _dirty = true;
                _cacheFileMetas[meta.FullPath] = meta;
            }
            s_logger.Debug("[update] file:{file} path:{path} md5:{md5} length:{length}", file, meta.FullPath, meta.Md5, meta.FileLength);
        }
    }
}
