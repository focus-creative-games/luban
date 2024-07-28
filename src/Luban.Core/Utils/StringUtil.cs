using System.Collections.Specialized;

namespace Luban.Utils;

public static class StringUtil
{
    public static string CollectionToString<T>(IEnumerable<T> collection)
    {
        return string.Join(",", collection);
    }

    public static string RepeatString(string str, int count)
    {
        return count == 0 ? "" : string.Concat(Enumerable.Repeat(str, count));
    }

    public static string RepeatSpaceAsTab(int count)
    {
        return count == 0 ? "" : string.Concat(Enumerable.Repeat("    ", count));
    }
}
