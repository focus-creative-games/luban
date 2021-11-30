using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
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
    public class FileRecordCacheManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private const int CACHE_FILE_LOW_WATER_MARK = 5000;
        private const int CACHE_FILE_HIGH_WATER_MARK = 8000;

        public static FileRecordCacheManager Ins { get; } = new FileRecordCacheManager();

        class FileRecordCache
        {
            public DefTable Table { get; }

            public List<Record> Records { get; }

            public volatile int LastAccessTime;

            public FileRecordCache(DefTable table, List<Record> records)
            {
                Table = table;
                Records = records;
                LastAccessTime = Bright.Time.TimeUtil.Now;
            }
        }

        public void Init(bool enableCache)
        {
            _enableCache = enableCache;
        }

        private readonly ConcurrentDictionary<(string TableName, string MD5, string SheetName), FileRecordCache> _caches = new();

        private readonly object _shrinkLocker = new object();

        private bool _enableCache = true;

        public bool TryGetCacheLoadedRecords(DefTable table, string md5, string originFile, string sheetName, out List<Record> cacheRecords)
        {
            cacheRecords = null;
            if (!_enableCache || !_caches.TryGetValue((table.FullName, md5, sheetName), out var r))
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

        public void AddCacheLoadedRecords(DefTable table, string md5, string sheetName, List<Record> cacheRecords)
        {
            lock (_shrinkLocker)
            {
                _caches[(table.FullName, md5, sheetName)] = new FileRecordCache(table, cacheRecords);
                if (_caches.Count > CACHE_FILE_HIGH_WATER_MARK)
                {
                    s_logger.Info("ShrinkCaches. cache count > high CACHE_FILE_HIGH_WATER_MARK:{}", CACHE_FILE_HIGH_WATER_MARK);
                    ShrinkCaches();
                }
            }
        }

        private readonly ConcurrentDictionary<(string TableFullName, string DataType), (DefTable Table, List<Record> Records, string Md5)> _tableCaches = new();

        public bool TryGetRecordOutputData(DefTable table, List<Record> records, string dataType, out string md5)
        {
            if (_enableCache && _tableCaches.TryGetValue((table.FullName, dataType), out var cacheInfo))
            {
                var cacheAss = cacheInfo.Table.Assembly;
                var curAss = table.Assembly;
                if (cacheAss.TimeZone == curAss.TimeZone
                    && cacheAss.TargetPatch == null && curAss.TargetPatch == null
                    && !cacheAss.NeedL10nTextTranslate && !curAss.NeedL10nTextTranslate
                    && cacheAss.OutputCompactJson == curAss.OutputCompactJson
                    && records.Count == cacheInfo.Records.Count && records.SequenceEqual(cacheInfo.Records))
                {
                    md5 = cacheInfo.Md5;
                    s_logger.Debug("find output data cache. table:{} dataType:{} md5:{}", table.FullName, dataType, md5);
                    return true;
                }
            }
            md5 = null;
            return false;
        }

        public void AddCachedRecordOutputData(DefTable table, List<Record> records, string dataType, string md5)
        {
            var curAss = table.Assembly;
            if (curAss.TargetPatch == null && !curAss.NeedL10nTextTranslate)
            {
                _tableCaches[(table.FullName, dataType)] = (table, records, md5);
                s_logger.Debug("add output data cache. table:{} dataType:{} md5:{}", table.FullName, dataType, md5);
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
                _caches.TryRemove(cacheList[i].Key, out _);
            }
            s_logger.Info("ShrinkCaches. after shrink, cache file num:{}", _caches.Count);
        }
    }
}
