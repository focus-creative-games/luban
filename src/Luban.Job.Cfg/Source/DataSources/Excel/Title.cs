using Bright.Collections;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.DataSources.Excel
{
    public class Title
    {
        public bool Root { get; set; }

        public int FromIndex { get; set; }

        public int ToIndex { get; set; }

        public string Name { get; set; }

        public string Sep { get; set; } = "|";

        public Dictionary<string, Title> SubTitles { get; set; } = new Dictionary<string, Title>();

        public List<Title> SubTitleList { get; set; } = new List<Title>();

        public void AddSubTitle(Title title)
        {
            if (!SubTitles.TryAdd(title.Name, title))
            {
                throw new Exception($"标题:{title.Name} 重复");
            }
            SubTitleList.Add(title);
        }

        // 由于先处理merge再处理只占一列的标题头.
        // sub titles 未必是有序的。对于大多数数据并无影响
        // 但对于 list类型的多级标题头，有可能导致element 数据次序乱了
        public void SortSubTitles()
        {
            SubTitleList.Sort((t1, t2) => t1.FromIndex - t2.FromIndex);
            foreach (var t in SubTitleList)
            {
                t.SortSubTitles();
            }
        }

        public override string ToString()
        {
            return $"name:{Name} [{FromIndex}, {ToIndex}] sub titles:[{string.Join(",\\n", SubTitleList)}]";
        }
    }
}
