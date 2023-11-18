using System.Diagnostics.CodeAnalysis;

namespace Luban.Utils;

public class ListEqualityComparer<T> : IEqualityComparer<List<T>>
{
    public static ListEqualityComparer<T> Default { get; } = new();

    public bool Equals(List<T> x, List<T> y)
    {
        return x.Count == y.Count && System.Linq.Enumerable.SequenceEqual(x, y);
    }

    public int GetHashCode([DisallowNull] List<T> obj)
    {
        int hash = 17;
        foreach (T x in obj)
        {
            hash = hash * 23 + x.GetHashCode();
        }
        return hash;
    }
}
