using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Utils
{
    public class ListEqualityComparer<T> : IEqualityComparer<List<T>>
    {
        public static ListEqualityComparer<T> Default { get; } = new ListEqualityComparer<T>();

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
}
