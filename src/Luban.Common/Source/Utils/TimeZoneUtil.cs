using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Common.Utils
{
    public static class TimeZoneUtil
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public static void InitDefaultTimeZone(string timeZoneName)
        {
            if (timeZoneName?.ToLower() == "local")
            {
                DefaultTimeZone = TimeZoneInfo.Local;
                return;
            }
            if (timeZoneName?.ToLower() == "utc")
            {
                DefaultTimeZone = TimeZoneInfo.Utc;
                return;
            }
            if (string.IsNullOrEmpty(timeZoneName))
            {
                try
                {
                    DefaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai");
                }
                catch (Exception)
                {
                    try
                    {
                        DefaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                    }
                    catch (Exception ex)
                    {
                        s_logger.Error(ex);
                        throw new ArgumentException("The default timezone ID 'Asia/Shanghai' and 'China Standard Time' was not found on local computer. please set valid timezone manually.");
                    }
                }
            }
            else
            {
                DefaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
            }
        }

        public static TimeZoneInfo DefaultTimeZone { get; set; } = TimeZoneInfo.Utc;
    }
}
