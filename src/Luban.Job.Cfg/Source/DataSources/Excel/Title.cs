using Bright.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.DataSources.Excel
{
    public class Title
    {
        public bool Root { get; set; }

        public int FromIndex { get; set; }

        public int ToIndex { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public Dictionary<string, Title> SubTitles { get; set; } = new Dictionary<string, Title>();

        public List<Title> SubTitleList { get; set; } = new List<Title>();

        public bool HasSubTitle => SubTitleList.Count > 0;

        public string Sep { get; private set; }

        public string Default { get; private set; }

        public bool SelfMultiRows { get; private set; }

        public bool HierarchyMultiRows { get; private set; }

        public bool SubHierarchyMultiRows { get; private set; }

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
        private void SortSubTitles()
        {
            SubTitleList.Sort((t1, t2) => t1.FromIndex - t2.FromIndex);
            foreach (var t in SubTitleList)
            {
                t.SortSubTitles();
            }
        }

        public void Init()
        {
            SortSubTitles();
            Sep = Tags.TryGetValue("sep", out var v) && !string.IsNullOrWhiteSpace(v) ? v : null;
            SelfMultiRows = Tags.TryGetValue("multi_rows", out var v2) && (v2 == "1" || v2 == "true");
            Default = Tags.TryGetValue("default", out var v3) ? v3 : null;
            if (SubTitleList.Count > 0)
            {
                foreach (var sub in SubTitleList)
                {
                    sub.Init();
                }
            }
            SubHierarchyMultiRows = SubTitleList.Any(t => t.HierarchyMultiRows); ;
            HierarchyMultiRows = SelfMultiRows || SubHierarchyMultiRows;
        }

        public override string ToString()
        {
            return $"name:{Name} [{FromIndex}, {ToIndex}] sub titles:[{string.Join(",\\n", SubTitleList)}]";
        }
    }
}
