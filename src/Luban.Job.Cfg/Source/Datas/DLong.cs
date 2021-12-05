using Luban.Job.Cfg.DataVisitors;

namespace Luban.Job.Cfg.Datas
{
    public class DLong : DType<long>
    {
        public static DLong Default { get; } = new DLong(0);
        private const int POOL_SIZE = 128;
        private static readonly DLong[] s_pool = new DLong[POOL_SIZE];

        static DLong()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                s_pool[i] = new DLong(i);
            }
        }

        public static DLong ValueOf(long x)
        {
            if (x >= 0 && x < POOL_SIZE)
            {
                return s_pool[x];
            }
            return new DLong(x);
        }

        public override string TypeName => "long";

        private DLong(long x) : base(x)
        {
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

        public override TR Apply<T1, T2, TR>(IDataFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
        {
            return visitor.Accept(this, x, y);
        }

        public override bool Equals(object obj)
        {
            return obj is DLong o && o.Value == this.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override int CompareTo(DType other)
        {
            if (other is DLong d)
            {
                return this.Value.CompareTo(d.Value);
            }
            throw new System.NotSupportedException();
        }
    }
}
