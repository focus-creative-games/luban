using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Common.Types;

namespace Luban.Job.Cfg.Datas
{
    public class DEnum : DType
    {
        public int Value { get; }

        public string StrValue { get; }

        public TEnum Type { get; }

        public DEnum(TEnum type, string value)
        {
            Type = type;
            StrValue = value;

            Value = type.DefineEnum.GetValueByNameOrAlias(value);
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
            return obj is DEnum o && o.StrValue == this.StrValue;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
