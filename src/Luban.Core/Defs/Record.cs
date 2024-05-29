using Luban.Datas;

namespace Luban.Defs;

public class Record
{
    public int AutoIndex { get; set; }

    public DBean Data { get; set; }

    public string Source { get; }

    public List<string> Tags { get; }

    public bool IsNotFiltered(List<string> excludeTags)
    {
        if (Tags == null)
        {
            return true;
        }
        return Tags.TrueForAll(t => !excludeTags.Contains(t));
    }

    public bool IsFiltered(List<string> includeTags)
    {
        if (Tags == null)
        {
            return true;
        }
        return Tags.Any(includeTags.Contains);
    }

    public Record(DBean data, string source, List<string> tags)
    {
        Data = data;
        Source = source;
        Tags = tags;
    }
}