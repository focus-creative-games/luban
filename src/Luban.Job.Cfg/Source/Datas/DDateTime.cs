using Luban.Job.Cfg.DataVisitors;
using System;

namespace Luban.Job.Cfg.Datas
{
    public class DDateTime : DType
    {
        public DateTime Time { get; }

        //public int UnixTime { get; }
        private readonly int _localTime;

        public override string TypeName => "datetime";

        public DDateTime(DateTime time)
        {
            this.Time = time;
            // time.Kind == DateTimeKind.Unspecified
            // DateTimeOffset��������Local����
            this._localTime = (int)new DateTimeOffset(time).ToUnixTimeSeconds();
        }

        public int GetUnixTime(TimeZoneInfo asTimeZone)
        {
            if (asTimeZone == null || asTimeZone == TimeZoneInfo.Local)
            {
                return this._localTime;
            }
            else
            {
                var destDateTime = TimeZoneInfo.ConvertTime(Time, asTimeZone, TimeZoneInfo.Utc);
                return (int)new DateTimeOffset(destDateTime).ToUnixTimeSeconds();
            }
        }

        public override void Apply<T>(IDataActionVisitor<T> visitor, T x)
        {
            visitor.Accept(this, x);
        }

        public override void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y)
        {
            visitor.Accept(this, x, y);
        }

        public override TR Apply<TR>(IDataFuncVisitor<TR> visitor)
        {
            return visitor.Accept(this);
        }

        public override TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x)
        {
            return visitor.Accept(this, x);
        }
    }
}
