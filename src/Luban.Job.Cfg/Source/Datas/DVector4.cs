using Luban.Job.Cfg.DataVisitors;
using System.Numerics;

namespace Luban.Job.Cfg.Datas
{
    public class DVector4 : DType<Vector4>
    {
        public static DVector4 Default { get; } = new DVector4(default);

        public override string TypeName => "vector4";

        public DVector4(Vector4 x) : base(x)
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
            return obj is DVector4 o && o.Value == this.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
