using Luban.Job.Cfg.DataVisitors;

namespace Luban.Job.Cfg.Datas
{
    public class DString : DType<string>
    {
        private static readonly DString s_empty = new DString("");

        public static DString ValueOf(string s)
        {
            if (s.Length == 0)
            {
                return s_empty;
            }
            return new DString(s);
        }

        public override string TypeName => "string";

        private DString(string x) : base(x)
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
            return obj is DString o && o.Value == this.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override int CompareTo(DType other)
        {
            if (other is DString d)
            {
                return this.Value.CompareTo(d.Value);
            }
            throw new System.NotSupportedException();
        }
    }
}
