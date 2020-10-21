using Luban.Job.Cfg.DataVisitors;
using System;

namespace Luban.Job.Cfg.Datas
{
    public class DDateTime : DType
    {
        public DateTime Time { get; }

        public int UnixTime { get; }

        public DDateTime(DateTime time)
        {
            this.Time = time;
            this.UnixTime = (int)new DateTimeOffset(time).ToUnixTimeSeconds();
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
