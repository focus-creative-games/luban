using System;
using System.Threading;

namespace Bright.Time
{
    public static class TimeUtil
    {

        public static readonly long TIMEZONE_OFFSET_MILLS = (long)TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.FromUnixTimeSeconds(0)).TotalMilliseconds;
        public static readonly int TIMEZONE_OFFSET = (int)TimeZoneInfo.Local.GetUtcOffset(DateTimeOffset.FromUnixTimeSeconds(0)).TotalSeconds;

        public const long DAY_MILLISECONDS = 86400000;
        public const int DAY_SECONDS = 86400;
        public const int HOUR_SECONDS = 3600;
        public const int MINUTE_SECONDS = 60;

        public const int WEEKDAY_OF_19700101 = 3;

        public const long HOUR_MILLISECONDS = 3600000;
        public const long MINUTE_MILLISECONDS = 60000;
        public const long WEEK_MILLISECONDS = DAY_MILLISECONDS * 7;

        public static bool IsSameDay(int time1, int time2)
        {
            return (time1 + TIMEZONE_OFFSET) / DAY_SECONDS == (time2 + TIMEZONE_OFFSET) / DAY_SECONDS;
        }

#if DEBUG
        private static long s_millisTimeOffsetForTest;

        /// <summary>
        /// 用于调整服务器内时间,只对测试版本生效
        /// </summary>
        public static long MillisTimeOffsetForTest
        {
            get
            {
                return Interlocked.Read(ref s_millisTimeOffsetForTest);
            }
            set
            {
                Interlocked.Exchange(ref s_millisTimeOffsetForTest, value);
            }
        }


        public static long NowMillis => DateTimeOffset.Now.ToUnixTimeMilliseconds() + s_millisTimeOffsetForTest;

        public static int Now => (int)(DateTimeOffset.Now.ToUnixTimeSeconds() + s_millisTimeOffsetForTest / 1000);

#else
        public static long NowMillis => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public static int Now => (int)DateTimeOffset.Now.ToUnixTimeSeconds();
#endif

        public static bool IsSameWeek(int time1, int time2)
        {
            return GetMondayZeroTimeOfThisWeek(time1) == GetMondayZeroTimeOfThisWeek(time2);
        }

        public static int TodayZeroTime()
        {
            return TodayZeroTime(Now);
        }

        public static int TodayZeroTime(int time)
        {
            return (time + TIMEZONE_OFFSET) / DAY_SECONDS * DAY_SECONDS - TIMEZONE_OFFSET;
        }


        public static int TomorrowZeroTime()
        {
            return TomorrowZeroTime(Now);
        }

        public static int TomorrowZeroTime(int time)
        {
            return TodayZeroTime(time) + DAY_SECONDS;
        }

        public static int AnotherDayZeroTime(int time, int offsetDayNum)
        {
            return TodayZeroTime(time) + DAY_SECONDS * offsetDayNum;
        }

        public static bool IsContinuesDay(int time1, int time2)
        {
            return (time1 + TIMEZONE_OFFSET) / DAY_SECONDS + 1 == (time2 + TIMEZONE_OFFSET) / DAY_SECONDS;
        }

        public static int DayOffset(int time1, int time2)
        {
            int a = (time1 + TIMEZONE_OFFSET) / DAY_SECONDS;
            int b = (time2 + TIMEZONE_OFFSET) / DAY_SECONDS;
            return Math.Abs(a - b);
        }

        public static int GetSecondsFromTodayZeroTime(int time)
        {
            return time - TodayZeroTime(time);
        }

        public static int GetHourOfToday(int time)
        {
            return (time + TIMEZONE_OFFSET) % DAY_SECONDS / HOUR_SECONDS;
        }

        public static int GetMinuteOfToday(int time)
        {
            long interval = time - TodayZeroTime(time);
            long left = interval % HOUR_SECONDS;
            return (int)(left / MINUTE_SECONDS);
        }

        public static long GetSecondsOfDay(int hour, int minute)
        {
            return hour * HOUR_SECONDS + minute * MINUTE_SECONDS;
        }

        public static int GetWeekDay(int time)
        {
            return ((time + TIMEZONE_OFFSET) / DAY_SECONDS + WEEKDAY_OF_19700101) % 7;
        }


        public static int GetMondayZeroTimeOfNextWeek(int time)
        {
            return TodayZeroTime(time) + DAY_SECONDS * (7 - GetWeekDay(time));
        }

        public static int GetMondayZeroTimeOfNextWeek()
        {
            return GetMondayZeroTimeOfNextWeek(Now);
        }

        public static int GetMondayZeroTimeOfThisWeek(int time)
        {
            return TodayZeroTime(time) - DAY_SECONDS * GetWeekDay(time);
        }

        /**
         * weekday 0 - 6 对应 周一 到 周日
         */
        public static long GetSecondsOfWeek(int weekday, int hour, int minute, int second)
        {
            return (weekday * 86400 + hour * 3600 + minute * 60 + second) * 1000L;
        }

        public static long ToMills(float seconds)
        {
            return (long)((double)seconds * 1000);
        }
    }
}
