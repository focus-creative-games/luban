namespace Luban.Utils;

public static class TimeZoneUtil
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static TimeZoneInfo GetTimeZone(string timeZoneName)
    {

        if (string.IsNullOrEmpty(timeZoneName))
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai");
            }
            catch (Exception)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                }
                catch (Exception)
                {
                    throw new ArgumentException("The default timezone ID 'Asia/Shanghai' and 'China Standard Time' was not found on local computer. please set valid timezone manually.");
                }
            }
        }
        if (timeZoneName.ToLower() == "local")
        {
            return TimeZoneInfo.Local;
        }
        if (timeZoneName.ToLower() == "utc")
        {
            return TimeZoneInfo.Utc;
        }
        return TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
    }
}
