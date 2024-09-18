using Luban.Datas;

namespace Luban.Defs;

public class Record
{
    public int AutoIndex { get; set; }

    public DBean Data { get; set; }

    public string Source { get; }

    public List<string> Tags { get; }

    public bool IsNotFiltered(List<string> includeTags, List<string> excludeTags)
    {
        if (Tags == null || Tags.Count == 0)
        {
            return true;
        }
        if (includeTags != null && includeTags.Count > 0)
        {
            return Tags.Any(includeTags.Contains);
        }
        return !Tags.Any(excludeTags.Contains);
    }

    public Record(DBean data, string source, List<string> tags)
    {
        Data = data;
        Source = source;
        Tags = tags;
    }
}
