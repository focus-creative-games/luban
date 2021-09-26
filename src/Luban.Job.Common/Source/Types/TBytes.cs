using Luban.Job.Common.TypeVisitors;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Types
{
    public class TBytes : TType
    {
        private static TBytes Ins { get; } = new TBytes(false, null);

        public static TBytes Create(bool isNullable, Dictionary<string, string> tags)
        {
            if (tags == null)
            {
                return isNullable ? new TBytes(true, null) : Ins;
            }
            else
            {
                return new TBytes(isNullable, tags);
            }
        }

        private TBytes(bool isNullable, Dictionary<string, string> tags) : base(isNullable, tags)
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
