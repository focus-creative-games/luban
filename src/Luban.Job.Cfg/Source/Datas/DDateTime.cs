using Luban.Common.Utils;
using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using System;

namespace Luban.Job.Cfg.Datas
{
    public class DDateTime : DType
    {
        public DateTime Time { get; }

        //public int UnixTime { get; }
        private readonly long _localTime;

        public override string TypeName => "datetime";

        public DDateTime(DateTime time)
        {

            this.Time = time;
            // time.Kind == DateTimeKind.Unspecified
            // DateTimeOffset把它当作Local处理
            this._localTime = (long)new DateTimeOffset(TimeZoneInfo.ConvertTime(time, TimeZoneUtil.DefaultTimeZone, TimeZoneInfo.Utc)).ToUnixTimeSeconds();
        }

        public override bool Equals(object obj)
        {
            return obj is DDateTime d && Time == d.Time;
        }

        public override int GetHashCode()
        {
            return _localTime.GetHashCode();
        }

        public override int CompareTo(DType other)
        {
            if (other is DDateTime d)
            {
                return this._localTime.CompareTo(d._localTime);
            }
            throw new System.NotSupportedException();
        }

        public string ToFormatString()
        {
            return DataUtil.FormatDateTime(Time);
        }

        public long GetUnixTime(TimeZoneInfo asTimeZone)
        {
            if (asTimeZone == null || asTimeZone == TimeZoneInfo.Local)
            {
                return this._localTime;
            }
            else
            {
                var destDateTime = TimeZoneInfo.ConvertTime(Time, asTimeZone, TimeZoneInfo.Utc);
                return (long)new DateTimeOffset(destDateTime).ToUnixTimeSeconds();
            }
        }

        public long UnixTimeOfCurrentAssembly => GetUnixTime(DefAssembly.LocalAssebmly.TimeZone);

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

        public override TR Apply<T1, T2, TR>(IDataFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
        {
            return visitor.Accept(this, x, y);
        }
    }
}
