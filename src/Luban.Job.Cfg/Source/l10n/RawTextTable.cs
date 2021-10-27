using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using System;
using System.Collections.Concurrent;

namespace Luban.Job.Cfg.l10n
{
    public class RawTextTable
    {
        private readonly ConcurrentDictionary<string, (Record Record, string Text)> _texts = new();

        public void AddText(Record record, string key, string text)
        {
            if (key == null || text == null)
            {
                throw new Exception("text的key或text属性不能为null");
            }
            if (key == "" && text != "")
            {
                throw new Exception($"text  key为空, 但text:{text}不为空");
            }
            if (!_texts.TryAdd(key, (record, text)) && _texts[key].Text != text)
            {
                throw new Exception($@"text key:{key} 出现多次，但值不相同. 
                当前:{text} 位置:{record.Source}
                之前:{_texts[key].Text} 位置:{_texts[key].Record.Source}");
            }
        }


        public bool TryGetText(string key, out string text)
        {
            if (_texts.TryGetValue(key, out var e))
            {
                text = e.Text;
                return true;
            }
            else
            {
                text = null;
                return false;
            }
        }
    }
}
