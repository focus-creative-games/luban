namespace Luban.Utils;

public static class CollectionExtensions
{
    public static void AddAll<K, V>(this Dictionary<K, V> resultDic, Dictionary<K, V> addDic) where K : notnull
    {
        foreach (var e in addDic)
        {
            resultDic[e.Key] = e.Value;
        }
    }
}
