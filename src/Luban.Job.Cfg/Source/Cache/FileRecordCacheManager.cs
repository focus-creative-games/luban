using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.Defs;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Cache
{
    /// <summary>
    /// 配置加载记录缓存。
    /// 如果某个表对应的数据文件未修改，定义没变化，那加载后的数据应该是一样的。
    /// </summary>
    class FileRecordCacheManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static FileRecordCacheManager Ins { get; } = new FileRecordCacheManager();

        private readonly ConcurrentDictionary<(string, string, string, bool), (DefTable, List<DType>)> _caches = new ConcurrentDictionary<(string, string, string, bool), (DefTable, List<DType>)>();

        public bool TryGetCacheLoadedRecords(DefTable table, string md5, string originFile, string sheetName, bool exportTestData, out List<DType> cacheRecords)
        {
            // TODO text localization check
            cacheRecords = null;
            if (!_caches.TryGetValue((table.Assembly.TimeZone.Id, md5, sheetName, exportTestData), out var r))
            {
                return false;
            }
            if (r.Item1.ValueTType.GetBeanAs<DefBean>().IsDefineEquals(table.ValueTType.GetBeanAs<DefBean>()))
            {
                cacheRecords = r.Item2;
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
            _caches[(table.Assembly.TimeZone.Id, md5, sheetName, exportTestData)] = (table, cacheRecords);
        }
    }
}
