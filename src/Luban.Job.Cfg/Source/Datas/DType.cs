
using Luban.Job.Cfg.DataVisitors;

namespace Luban.Job.Cfg.Datas
{
    public abstract class DType
    {
        public abstract void Apply<T>(IDataActionVisitor<T> visitor, T x);

        public abstract void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y);

        public abstract TR Apply<TR>(IDataFuncVisitor<TR> visitor);

        public abstract TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x);

        public abstract string TypeName { get; }

        public override string ToString()
        {
            return this.Apply(ToStringVisitor.Ins);
        }
    }

    public abstract class DType<T> : DType
    {
        public T Value { get; }

        protected DType(T value)
        {
            Value = value;
        }
    }
}
