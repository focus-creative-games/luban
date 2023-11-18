namespace Luban.Utils;

public static class StringUtil
{
    public static string CollectionToString<T>(IEnumerable<T> collection)
    {
        return string.Join(",", collection);
    }
}
