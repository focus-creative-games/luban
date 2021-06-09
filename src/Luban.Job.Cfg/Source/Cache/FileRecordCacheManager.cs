using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Cache
{
    /// <summary>
    /// 配置加载记录缓存。
    /// 如果某个表对应的数据文件未修改，定义没变化，那加载后的数据应该是一样的。
    /// </summary>
    class FileRecordCacheManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private const int CACHE_FILE_LOW_WATER_MARK = 5000;
        private const int CACHE_FILE_HIGH_WATER_MARK = 8000;

        public static FileRecordCacheManager Ins { get; } = new FileRecordCacheManager();

        class FileRecordCache
        {
            public DefTable Table { get; }

            public List<DType> Records { get; }

            public volatile int LastAccessTime;

            public FileRecordCache(DefTable table, List<DType> records)
            {
                Table = table;
                Records = records;
                LastAccessTime = Bright.Time.TimeUtil.Now;
            }
        }

        private readonly ConcurrentDictionary<(string, string, string, bool), FileRecordCache> _caches = new ConcurrentDictionary<(string, string, string, bool), FileRecordCache>();

        private readonly object _shrinkLocker = new object();

        public bool TryGetCacheLoadedRecords(DefTable table, string md5, string originFile, string sheetName, bool exportTestData, out List<DType> cacheRecords)
        {
            // TODO text localization check
            cacheRecords = null;
            if (!_caches.TryGetValue((table.Assembly.TimeZone.Id, md5, sheetName, exportTestData), out var r))
            {
                return false;
            }
            r.LastAccessTime = Bright.Time.TimeUtil.Now;
            if (r.Table.ValueTType.GetBeanAs<DefBean>().IsDefineEquals(table.ValueTType.GetBeanAs<DefBean>()))
            {
                cacheRecords = r.Records;
                s_logger.Trace("hit cache. table:{table} file:{file} md5:{md5}", table.FullName, originFile, md5);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddCacheLoadedRecords(DefTable table, string md5, string sheetName, bool exportTestData, List<DType> cacheRecords)
        {
            lock (_shrinkLocker)
            {
                _caches[(table.Assembly.TimeZone.Id, md5, sheetName, exportTestData)] = new FileRecordCache(table, cacheRecords);
                if (_caches.Count > CACHE_FILE_HIGH_WATER_MARK)
                {
                    s_logger.Info("ShrinkCaches. cache count > high CACHE_FILE_HIGH_WATER_MARK:{}", CACHE_FILE_HIGH_WATER_MARK);
                    ShrinkCaches();
                }
            }
        }

        private void ShrinkCaches()
        {
            if (_caches.Count < CACHE_FILE_HIGH_WATER_MARK)
            {
                return;
            }
            var cacheList = _caches.ToList();
            cacheList.Sort((a, b) => a.Value.LastAccessTime - b.Value.LastAccessTime);
            for (int i = 0; i < CACHE_FILE_HIGH_WATER_MARK - CACHE_FILE_LOW_WATER_MARK; i++)
            {
                _caches.Remove(cacheList[i].Key, out _);
            }
            s_logger.Info("ShrinkCaches. after shrink, cache file num:{}", _caches.Count);
        }
    }
}
