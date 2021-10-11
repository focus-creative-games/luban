using System;
using System.Collections.Generic;

namespace Bright.Collections
{
    public static class CollectionExtension
    {

        public static void AddRange<T>(this IList<T> dst, IEnumerable<T> src)
        {
            foreach (var x in src)
            {
                dst.Add(x);
            }
        }

        public static bool TryAdd<TK, TV>(this IDictionary<TK, TV> map, TK key, TV value)
        {
            if (map.ContainsKey(key))
            {
                return false;
            }
            map.Add(key, value);
            return true;
        }

        public static TV GetValueOrDefault<TK, TV>(this IDictionary<TK, TV> map, TK key, TV defaultValue = default)
        {
            return map.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> map, TK key, Func<TK, TV> creator)
        {
            if (map.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                TV newValue = creator(key);
                map.Add(key, newValue);
                return newValue;
            }
        }

        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> map, TK key) where TV : new()
        {
            if (map.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                TV newValue = new TV();
                map.Add(key, newValue);
                return newValue;
            }
        }

        public static TV Get<TK, TV>(this IDictionary<TK, TV> map, TK key) where TV : class
        {
            return map.TryGetValue(key, out var value) ? value : null;
        }

        public static bool Contains<TK, TV>(this IDictionary<TK, TV> map, TK key, TV value)
        {
            return map.Contains(new KeyValuePair<TK, TV>(key, value));
        }

        public static void PutAll<TK, TV>(this IDictionary<TK, TV> dst, IDictionary<TK, TV> src)
        {
            foreach (var e in src)
            {
                dst[e.Key] = e.Value;
            }
        }

        public static bool IsEmpty<TE>(this ICollection<TE> c)
        {
            return c.Count == 0;
        }

        public static void Merge(this IDictionary<int, float> dst, IDictionary<int, float> src)
        {
            foreach (var e in src)
            {
                if (dst.TryGetValue(e.Key, out var v))
                {
                    dst[e.Key] = v + e.Value;
                }
                else
                {
                    dst[e.Key] = e.Value;
                }
            }
        }

        public static void Replace(this IDictionary<int, float> dst, IDictionary<int, float> src)
        {
            dst.Clear();
            dst.PutAll(src);
        }

        public static int AddValue<T>(this IDictionary<T, int> dst, T key, int add)
        {
            if (dst.TryGetValue(key, out var value))
            {
                return dst[key] = value + add;
            }
            else
            {
                return dst[key] = add;
            }
        }

        public static long AddValue<T>(this IDictionary<T, long> dst, T key, long add)
        {
            if (dst.TryGetValue(key, out var value))
            {
                return dst[key] = value + add;
            }
            else
            {
                return dst[key] = add;
            }
        }

        public static int IncrementValue<T>(this IDictionary<T, int> dst, T key)
        {
            if (dst.TryGetValue(key, out var value))
            {
                return dst[key] = value + 1;
            }
            else
            {
                return dst[key] = 1;
            }
        }

        public static long IncrementValue<T>(this IDictionary<T, long> dst, T key)
        {
            if (dst.TryGetValue(key, out var value))
            {
                return dst[key] = value + 1;
            }
            else
            {
                return dst[key] = 1;
            }
        }

        public static bool ContainsAll<T>(this IList<T> a, IList<T> b)
        {
            foreach (var x in b)
            {
                if (!a.Contains(x))
                {
                    return false;
                }
            }
            return true;
        }


        public static void RemoveAll<T>(this IList<T> a, IList<T> b)
        {
            foreach (var x in b)
            {
                a.Remove(x);
            }
        }

        public static bool DicEquals<TK, TV>(this IDictionary<TK, TV> a, IDictionary<TK, TV> b)
        {
            if (a.Count != b.Count)
            {
                return false;
            }
            foreach (var e in a)
            {
                if (!(b.TryGetValue(e.Key, out var v) && v.Equals(e.Value)))
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddAll<TK, TV>(this Dictionary<TK, TV> data, Dictionary<TK, TV> b)
        {
            foreach (var e in b)
            {
                data[e.Key] = e.Value;
            }
        }

        public static Dictionary<TK, TV> Plus<TK, TV>(this Dictionary<TK, TV> data, TK key, TV value)
        {
            var newMap = new Dictionary<TK, TV>(data)
            {
                [key] = value
            };
            return newMap;
        }



        public static T[] CopySubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// 返回第一个满足条件的元素的index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="predic"></param>
        /// <returns>返回第一个满足条件的元素的index,如果没找到，返回 -1</returns>
        public static int IndexOfFirst<T>(IEnumerable<T> arr, Func<T, bool> predic)
        {
            int i = 0;
            foreach (var x in arr)
            {
                if (predic(x))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// 返回最后一个满足条件的元素的index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="predic"></param>
        /// <returns>返回最后一个满足条件的元素的index,如果没找到，返回 -1</returns>
        public static int IndexOfLast<T>(IEnumerable<T> arr, Func<T, bool> predic)
        {
            int i = -1;
            foreach (var x in arr)
            {
                if (predic(x))
                {
                    ++i;
                }
                else
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
