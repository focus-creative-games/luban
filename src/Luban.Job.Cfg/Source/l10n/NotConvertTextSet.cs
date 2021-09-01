using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.l10n
{
    public class NotConvertTextSet
    {
        private readonly ConcurrentDictionary<string, string> _notConvertTexts = new();
        public void Add(string key, string text)
        {
            if (key != "")
            {
                _notConvertTexts.TryAdd(key, text);
            }
        }

        public List<KeyValuePair<string, string>> SortedEntry
        {
            get
            {
                var list = _notConvertTexts.ToList();
                list.Sort((a, b) => a.Key.CompareTo(b.Key));
                return list;
            }
        }
    }
}
