namespace Bright.Common
{
    public static class ValueUtil
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static void LockAndSwap<T>(object locker, ref T a, ref T b)
        {
            lock (locker)
            {
                T temp = a;
                a = b;
                b = temp;
            }
        }
    }
}
