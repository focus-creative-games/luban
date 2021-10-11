using Luban.Job.Cfg.Utils;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class Record
    {
        public DBean Data { get; }

        public string Source { get; }

        public List<string> Tags { get; }

        public int Index { get; set; }

        public bool IsTest => Tags != null && DataUtil.IsTestTag(Tags);

        public Record(DBean data, string source, List<string> tags)
        {
            Data = data;
            Source = source;
            Tags = tags;
        }
    }
}
