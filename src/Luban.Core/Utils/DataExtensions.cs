using Luban.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Utils;

public static class DataExtensions
{
    public static long UnixTimeOfCurrentContext(this DDateTime dateTime)
    {
        return dateTime.GetUnixTime(GenerationContext.Current.TimeZone);
    }
}
