using Bright.Threading;
using Bright.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Server.Common
{
    public class FileCache
    {
        public string MD5 { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public int LastAccessTime { get; set; }

        public long Timestamp { get; set; }
    }

    public class CacheManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly AtomicLong s_timestampAlloc = new AtomicLong();

        public static CacheManager Ins { get; } = new CacheManager();


        private readonly Dictionary<string, FileCache> _caches = new Dictionary<string, FileCache>();

        public long CacheHighWaterMarkSize { get; set; } = 4L * 1024L * 1024L * 1024L; // 1g 

        public long CacheLowWaterMarkSize { get; set; } = 3L * 1024 * 1024L * 1024L; // 1g 

        public long TotalBytes { get; private set; }


        public void Reset()
        {
            lock (this)
            {
                _caches.Clear();
                TotalBytes = 0;
            }
        }

        public FileCache FindCache(string MD5, bool updateTimestamp = true)
        {
            lock (this)
            {
                if (_caches.TryGetValue(MD5, out var cache))
                {
                    if (updateTimestamp)
                    {
                        UpdateAccess(cache);
                    }
                    return cache;
                }
                else
                {
                    return null;
                }
            }
        }

        public FileCache TestGetCache(string MD5)
        {
            lock (this)
            {
                return _caches.TryGetValue(MD5, out var cache) ? cache : null;
            }
        }

        private static void UpdateAccess(FileCache cache)
        {
            cache.Timestamp = s_timestampAlloc.IncrementAndGet();
        }


        public void AddCache(string fileName, string MD5, byte[] content)
        {
            lock (this)
            {
                if (_caches.ContainsKey(MD5))
                {
                    _caches[MD5].Timestamp = s_timestampAlloc.IncrementAndGet();
                }
                else
                {
                    var cache = new FileCache()
                    {
                        FileName = fileName,
                        MD5 = MD5,
                        Content = content,
                        LastAccessTime = TimeUtil.Now,
                        Timestamp = s_timestampAlloc.IncrementAndGet(),
                    };
                    _caches[MD5] = cache;
                    TotalBytes += content.Length;

                    if (TotalBytes > CacheHighWaterMarkSize)
                    {
                        RemoveLRU();
                    }
                }
            }

        }

        private void RemoveLRU()
        {
            lock (this)
            {
                s_logger.Info("before remove expires. cache nums:{num} total bytes:{bytes}", _caches.Count, TotalBytes);

                var sortCacheByTimestamp = _caches.Values.ToList();
                sortCacheByTimestamp.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

                long lowWaterMarkSize = Math.Min(CacheLowWaterMarkSize, (long)(CacheHighWaterMarkSize * 0.9f));
                foreach (var c in sortCacheByTimestamp)
                {
                    if (TotalBytes <= lowWaterMarkSize)
                    {
                        break;
                    }

                    _caches.Remove(c.MD5);
                    TotalBytes -= c.Content.Length;
                    s_logger.Info("remove cache. file:{file} md5:{md5} size:{size}, total bytes:{bytes} after remove.", c.FileName, c.MD5, c.Content.Length, TotalBytes);
                }
                s_logger.Info("after remove expires. cache nums:{num} total bytes:{bytes}", _caches.Count, TotalBytes);
            }
        }
    }
}
