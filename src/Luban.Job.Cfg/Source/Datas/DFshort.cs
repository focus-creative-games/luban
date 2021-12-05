using Luban.Job.Cfg.DataVisitors;

namespace Luban.Job.Cfg.Datas
{
    public class DFshort : DType<short>
    {
        public static DFshort Default { get; } = new DFshort(0);

        public static DFshort ValueOf(short x)
        {
            return x == 0 ? Default : new DFshort(x);
        }

        public override string TypeName => "fshort";

        private DFshort(short x) : base(x)
        {
        }

        public override bool Equals(object obj)
        {
            return obj is DFshort d && Value == d.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override int CompareTo(DType other)
        {
            if (other is DFshort d)
            {
                return this.Value.CompareTo(d.Value);
            }
            throw new System.NotSupportedException();
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
    }
}
