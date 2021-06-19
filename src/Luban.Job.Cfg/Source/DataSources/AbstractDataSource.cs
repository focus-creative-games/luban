using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using System.Collections.Generic;
using System.IO;

namespace Luban.Job.Cfg.DataSources
{
    abstract class AbstractDataSource
    {
        public const string TAG_KEY = "__tag__";

        public static bool IsIgnoreTag(string tagName)
        {
            return !string.IsNullOrWhiteSpace(tagName) &&
                (
                   tagName.Equals("false", System.StringComparison.OrdinalIgnoreCase)
                || tagName.Equals("no", System.StringComparison.OrdinalIgnoreCase)
                || tagName.Equals("##", System.StringComparison.Ordinal)
                || tagName.Equals("∑Ò", System.StringComparison.Ordinal)
                );
        }

        public static bool IsTestTag(string tagName)
        {
            return !string.IsNullOrWhiteSpace(tagName) &&
                (tagName.Equals("test", System.StringComparison.OrdinalIgnoreCase)
                || tagName.Equals("≤‚ ‘", System.StringComparison.Ordinal)
                );
        }

        public string RawUrl { get; protected set; }

        public abstract Record ReadOne(TBean type);

        public abstract List<Record> ReadMulti(TBean type);

        public abstract void Load(string rawUrl, string sheetName, Stream stream, bool exportDebugData);
    }
}
