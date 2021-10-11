using System.Collections.Generic;
using System.Text;

namespace Bright.Collections
{

    public static class EmptyDictionary<TK, TV>
    {
        public static Dictionary<TK, TV> Empty { get; } = new Dictionary<TK, TV>();
    }

    public static class CollectionUtil
    {

        public static Dictionary<TK, TV> SingletonMap<TK, TV>(TK key, TV value)
        {
            var newMap = new Dictionary<TK, TV>
            {
                { key, value }
            };
            return newMap;
        }

        public static string ToString<TK, TV>(IDictionary<TK, TV> m)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            foreach (var e in m)
            {
                sb.Append(e.Key).Append(':').Append(e.Value).Append(',');
            }
            sb.Append('}');
            return sb.ToString();
        }

        public static void Replace<TK, TV>(IDictionary<TK, TV> dest, IDictionary<TK, TV> src)
        {
            dest.Clear();
            foreach (var entry in src)
            {
                dest.Add(entry.Key, entry.Value);
            }

        }

        public static void MergeIntValueDic<TK>(IDictionary<TK, int> dest, IDictionary<TK, int> src)
        {
            foreach (var entry in src)
            {
                if (dest.ContainsKey(entry.Key))
                {
                    dest[entry.Key] = dest[entry.Key] + entry.Value;
                }
                else
                {
                    dest.Add(entry.Key, entry.Value);
                }
            }
        }

        public static void MergeFloatValueDic<TK>(IDictionary<TK, float> dest, IDictionary<TK, float> src)
        {
            foreach (var entry in src)
            {
                if (dest.ContainsKey(entry.Key))
                {
                    dest[entry.Key] = dest[entry.Key] + entry.Value;
                }
                else
                {
                    dest.Add(entry.Key, entry.Value);
                }
            }
        }
    }
}
