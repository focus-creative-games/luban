using Luban.Job.Common.TypeVisitors;
using System.Collections.Generic;

namespace Luban.Job.Common.Types
{
    public class TLong : TType
    {
        private static TLong Ins { get; } = new TLong(false, null, false);

        private static TLong NullableIns { get; } = new TLong(true, null, false);

        private static TLong BigIns { get; } = new TLong(false, null, true);

        private static TLong NullableBigIns { get; } = new TLong(true, null, true);

        public static TLong Create(bool isNullable, Dictionary<string, string> tags, bool isBigInt)
        {
            if (tags == null)
            {
                return isNullable ? NullableIns : Ins;
            }
            else
            {
                return new TLong(isNullable, tags, isBigInt);
            }
        }

        public bool IsBigInt { get; }

        private TLong(bool isNullable, Dictionary<string, string> tags, bool isBigInt) : base(isNullable, tags)
        {
            IsBigInt = isBigInt;
        }

        public override bool TryParseFrom(string s)
        {
            return long.TryParse(s, out _);
        }

        public override void Apply<T>(ITypeActionVisitor<T> visitor, T x)
        {
            visitor.Accept(this, x);
        }

        public override void Apply<T1, T2>(ITypeActionVisitor<T1, T2> visitor, T1 x, T2 y)
        {
            visitor.Accept(this, x, y);
        }

        public override TR Apply<TR>(ITypeFuncVisitor<TR> visitor)
        {
            return visitor.Accept(this);
        }

        public override TR Apply<T, TR>(ITypeFuncVisitor<T, TR> visitor, T x)
        {
            return visitor.Accept(this, x);
        }

        public override TR Apply<T1, T2, TR>(ITypeFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
        {
            return visitor.Accept(this, x, y);
        }

        public override TR Apply<T1, T2, T3, TR>(ITypeFuncVisitor<T1, T2, T3, TR> visitor, T1 x, T2 y, T3 z)
        {
            return visitor.Accept(this, x, y, z);
        }
    }
}
