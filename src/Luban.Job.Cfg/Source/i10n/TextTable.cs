using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.i10n
{
    public class TextTable
    {
        private readonly Dictionary<string, string> _key2Texts = new();

        public TextTable()
        {
            _key2Texts.Add("test/a", "这是本地化数据 test/a");
            _key2Texts.Add("name", "这是本地化数据 name");
        }

        public void AddText(string key, string text)
        {
            if (!_key2Texts.TryAdd(key, text))
            {
                throw new Exception($"text key:{key} 重复");
            }
        }

        public bool TryGetText(string key, out string text)
        {
            return _key2Texts.TryGetValue(key, out text);
        }
    }
}
