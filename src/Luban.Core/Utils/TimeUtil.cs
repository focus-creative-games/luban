namespace Luban.Utils;

public static class TimeUtil
{
    public static int Now => (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

    public static long NowMillis => (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
}
