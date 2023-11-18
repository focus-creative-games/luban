using Luban.Utils;

namespace Luban.DataLoader.Builtin.Excel;

public class Title
{
    public bool Root { get; set; }

    public int FromIndex { get; set; }

    public int ToIndex { get; set; }

    public string Name { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public Dictionary<string, Title> SubTitles { get; set; } = new();

    public List<Title> SubTitleList { get; set; } = new();

    public bool HasSubTitle => SubTitleList.Count > 0;

    public string Sep { get; private set; }

    public string SepOr(string sep)
    {
        return string.IsNullOrEmpty(sep) ? Sep : sep;
    }

    public bool NonEmpty { get; private set; }

    public string Default { get; private set; }

    public bool SelfMultiRows { get; private set; }

    public bool HierarchyMultiRows { get; private set; }

    public bool SubHierarchyMultiRows { get; private set; }

    public void AddSubTitle(Title title)
    {
        if (!SubTitles.TryAdd(title.Name, title))
        {
            throw new Exception($"列:{title.Name} 重复");
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

    private static HashSet<string> s_validTags = new()
    {
        "sep",
        "non_empty",
        "multi_rows",
        "default",
    };

    public void Init()
    {
        SortSubTitles();
        Sep = Tags.TryGetValue("sep", out var sep) ? sep : "";
        //if (Tags.ContainsKey("sep"))
        //{
        //    throw new Exception($"字段名现在不支持sep，请移到##type行，例如'int&sep=;'");
        //}
        NonEmpty = Tags.TryGetValue("non_empty", out var ne) && ne == "1";
        SelfMultiRows = Tags.TryGetValue("multi_rows", out var v2) && (v2 == "1" || v2 == "true");
        Default = Tags.TryGetValue("default", out var v3) ? v3 : null;

        foreach (var (key, value) in Tags)
        {
            if (!s_validTags.Contains(key))
            {
                throw new Exception($"excel标题列:'{Name}' 不支持tag:'{key}',请移到##type行");
            }
        }

        if (SubTitleList.Count > 0)
        {
            if (Root)
            {
                var firstField = SubTitleList.FirstOrDefault(f => DefUtil.IsNormalFieldName(f.Name));
                if (firstField != null)
                {
                    // 第一个字段一般为key，为了避免失误将空单元格当作key=0的数据，默认非空
                    firstField.Tags.TryAdd("non_empty", "1");
                }
            }
            foreach (var sub in SubTitleList)
            {
                sub.Init();
            }
        }
        SubHierarchyMultiRows = SubTitleList.Any(t => t.HierarchyMultiRows);
        ;
        HierarchyMultiRows = SelfMultiRows || SubHierarchyMultiRows;
    }

    public override string ToString()
    {
        return $"name:{Name} [{FromIndex}, {ToIndex}] sub titles:[{string.Join(",\\n", SubTitleList)}]";
    }
}
