using Luban.DataVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Datas;

public class DDateTime : DType
{
    public DateTime Time { get; }

    public override string TypeName => "datetime";

    public DDateTime(DateTime time)
    {

        this.Time = time;
    }

    public override bool Equals(object obj)
    {
        return obj is DDateTime d && Time == d.Time;
    }

    public override int GetHashCode()
    {
        return Time.GetHashCode();
    }

    public override int CompareTo(DType other)
    {
        if (other is DDateTime d)
        {
            return this.Time.CompareTo(d.Time);
        }
        throw new System.NotSupportedException();
    }

    public string ToFormatString()
    {
        return DataUtil.FormatDateTime(Time);
    }

    public long GetUnixTime(TimeZoneInfo asTimeZone)
    {
        var destDateTime = TimeZoneInfo.ConvertTime(Time, asTimeZone, TimeZoneInfo.Utc);
        return new DateTimeOffset(destDateTime).ToUnixTimeSeconds();
    }

    public override void Apply<T>(IDataActionVisitor<T> visitor, T x)
    {
        visitor.Accept(this, x);
    }

    public override void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y)
    {
        visitor.Accept(this, x, y);
    }

    public override void Apply<T>(IDataActionVisitor2<T> visitor, TType type, T x)
    {
        visitor.Accept(this, type, x);
    }

    public override void Apply<T1, T2>(IDataActionVisitor2<T1, T2> visitor, TType type, T1 x, T2 y)
    {
        visitor.Accept(this, type, x, y);
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

    public override TR Apply<TR>(IDataFuncVisitor2<TR> visitor, TType type)
    {
        return visitor.Accept(this, type);
    }

    public override TR Apply<T, TR>(IDataFuncVisitor2<T, TR> visitor, TType type, T x)
    {
        return visitor.Accept(this, type, x);
    }

    public override TR Apply<T1, T2, TR>(IDataFuncVisitor2<T1, T2, TR> visitor, TType type, T1 x, T2 y)
    {
        return visitor.Accept(this, type, x, y);
    }
}
