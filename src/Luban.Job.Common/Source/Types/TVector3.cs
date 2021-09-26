using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Types
{
    public class TVector3 : TType
    {
        private static TVector3 Ins { get; } = new TVector3(false, null);

        private static TVector3 NullableIns { get; } = new TVector3(true, null);

        public static TVector3 Create(bool isNullable, Dictionary<string, string> tags)
        {
            if (tags == null)
            {
                return isNullable ? NullableIns : Ins;
            }
            else
            {
                return new TVector3(isNullable, tags);
            }
        }

        private TVector3(bool isNullable, Dictionary<string, string> tags) : base(isNullable, tags)
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

        public override void Apply<T1, T3>(ITypeActionVisitor<T1, T3> visitor, T1 x, T3 y)
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

        public override TR Apply<T1, T3, TR>(ITypeFuncVisitor<T1, T3, TR> visitor, T1 x, T3 y)
        {
            return visitor.Accept(this, x, y);
        }

        public override TR Apply<T1, T2, T3, TR>(ITypeFuncVisitor<T1, T2, T3, TR> visitor, T1 x, T2 y, T3 z)
        {
            return visitor.Accept(this, x, y, z);
        }
    }
}
