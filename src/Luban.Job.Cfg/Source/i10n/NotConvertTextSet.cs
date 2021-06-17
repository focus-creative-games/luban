using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.i10n
{
    public class NotConvertTextSet
    {
        private readonly ConcurrentDictionary<string, string> _notConvertTexts = new();
        public void Add(string key, string text)
        {
            _notConvertTexts.TryAdd(key, text);
        }
    }
}
