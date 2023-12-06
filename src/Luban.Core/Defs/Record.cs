using Luban.Datas;

namespace Luban.Defs;

public class Record
{
    public int AutoIndex { get; set; }

    public DBean Data { get; }

    public string Source { get; }

    public List<string> Tags { get; }

    public string SheetName { get; }

    public bool IsNotFiltered(List<string> excludeTags)
    {
        if (Tags == null)
        {
            return true;
        }
        return Tags.TrueForAll(t => !excludeTags.Contains(t));
    }
    public Record(DBean data, string source, string sheetName, List<string> tags)
    {
        Data = data;
        Source = source;
        Tags = tags;
        SheetName = sheetName;
    }
    public Record(DBean data, string source, List<string> tags)
    {
        Data = data;
        Source = source;
        Tags = tags;
    }
}
