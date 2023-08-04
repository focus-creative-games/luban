using System.Collections;

namespace Luban.Core.Utils;

public static class StringUtil
{
    public static string CollectionToString<T>(IEnumerable<T> collection)
    {
        return string.Join(",", collection);
    }
}