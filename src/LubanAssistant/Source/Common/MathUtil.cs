namespace Bright.Common
{
    public static class MathUtil
    {
        /// <summary>
        /// 确保返回 x + y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int AddExactly(int x, int y)
        {
            return checked(x + y);
        }

        /// <summary>
        /// 确保返回 x + y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long AddExactly(long x, long y)
        {
            return checked(x + y);
        }

        /// <summary>
        /// 确保返回 x - y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int SubExcatly(int x, int y)
        {
            return checked(x - y);
        }

        /// <summary>
        /// 确保返回 x - y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long SubExactly(long x, long y)
        {
            return checked(x - y);
        }

        /// <summary>
        /// 确保返回 x * y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int MultifyExactly(int x, int y)
        {
            return checked(x * y);
        }

        /// <summary>
        /// 确保返回 x * y 的正确值, 如果溢出 抛出异常
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static long MultifyExactly(long x, long y)
        {
            return checked(x * y);
        }

        /// <summary>
        /// 取 >= x/y 的最小整数
        /// </summary>
        /// <param name="x">除数</param>
        /// <param name="y">被除数</param>
        /// <returns></returns>
        public static int Ceil(int x, int y)
        {
            return (x + y - 1) / y;
        }

        /// <summary>
        /// 取 >= x/y 的最小整数
        /// </summary>
        /// <param name="x">除数</param>
        /// <param name="y">被除数</param>
        /// <returns></returns>
        public static long Ceil(long x, long y)
        {
            return (x + y - 1) / y;
        }

        /// <summary>
        /// 返回 [min, max] 之间的数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

    }
}
