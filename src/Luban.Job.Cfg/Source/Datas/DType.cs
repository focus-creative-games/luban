
using Luban.Job.Cfg.DataVisitors;
using System.Text;

namespace Luban.Job.Cfg.Datas
{
    public abstract class DType
    {
        public abstract void Apply<T>(IDataActionVisitor<T> visitor, T x);

        public abstract void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y);

        public abstract TR Apply<TR>(IDataFuncVisitor<TR> visitor);

        public abstract TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x);

        public override string ToString()
        {
            var s = new StringBuilder();
            this.Apply(ToStringVisitor.Ins, s);
            return s.ToString();
        }

        public string JsonValue => this.Apply(ToJsonStringVisitor.Ins);

        public string LuaValue => this.Apply(ToLuaStringVisitor.Ins);
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
