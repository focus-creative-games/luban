using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Common.Types
{
    public class TVector2 : TType
    {
        public static TVector2 Ins { get; } = new TVector2(false);

        public static TVector2 NullableIns { get; } = new TVector2(true);

        public TVector2(bool isNullable) : base(isNullable)
        {
        }

        public override bool TryParseFrom(string s)
        {
            throw new NotSupportedException();
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
