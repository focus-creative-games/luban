using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Common.Utils
{
    public static class TimeZoneUtil
    {
        public static TimeZoneInfo DefaultTimeZone { get; set; } = TimeZoneInfo.Utc;
    }
}
