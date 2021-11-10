using Luban.Job.Cfg.DataSources.Excel;
using System.Collections.Concurrent;

namespace Luban.Job.Cfg.Cache
{
    class ExcelTableValueTypeDefInfoCacheManager
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static ExcelTableValueTypeDefInfoCacheManager Instance { get; } = new();

        private readonly ConcurrentDictionary<(string MD5, string Sheet), RawSheetTableDefInfo> _cacheDefs = new();

        public bool TryGetTableDefInfo(string md5, string sheet, out RawSheetTableDefInfo defInfo)
        {
            if (_cacheDefs.TryGetValue((md5, sheet), out defInfo))
            {
                s_logger.Debug("find RawSheetTableDefInfo in cache. md5:{} sheet:{} def:{@}", md5, sheet, defInfo);
                return true;
            }
            return false;
        }

        public void AddTableDefInfoToCache(string md5, string sheet, RawSheetTableDefInfo defInfo)
        {
            _cacheDefs.TryAdd((md5, sheet), defInfo);
            s_logger.Debug("add RawSheetTableDefInfo in cache. md5:{} sheet:{} def:{@}", md5, sheet, defInfo);
        }
    }
}
