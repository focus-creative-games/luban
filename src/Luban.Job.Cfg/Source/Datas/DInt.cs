using Luban.Job.Cfg.DataVisitors;

namespace Luban.Job.Cfg.Datas
{
    public class DInt : DType<int>
    {
        private const int POOL_SIZE = 128;
        private static readonly DInt[] s_pool = new DInt[POOL_SIZE];

        static DInt()
        {
            for (int i = 0; i < POOL_SIZE; i++)
            {
                s_pool[i] = new DInt(i);
            }
        }

        public static DInt ValueOf(int x)
        {
            if (x >= 0 && x < POOL_SIZE)
            {
                return s_pool[x];
            }
            return new DInt(x);
        }

        public override string TypeName => "int";

        private DInt(int x) : base(x)
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

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case DInt dint: return this.Value == dint.Value;
                case DFint fint: return this.Value == fint.Value;
                case DEnum denum: return this.Value == denum.Value;
                default: return false;
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
