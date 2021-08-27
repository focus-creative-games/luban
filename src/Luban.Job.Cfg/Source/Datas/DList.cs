using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Common.Types;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class DList : DType
    {
        public TList Type { get; }
        public List<DType> Datas { get; }

        public bool IsList => true;

        public DList(TList type, List<DType> datas)
        {
            this.Type = type;
            this.Datas = datas;
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

        //public override bool Equals(object obj)
        //{
        //    if (obj is DList o)
        //    {
        //        return o.Datas.Count == this.Datas.Count && o.Datas.SequenceEqual(this.Datas);
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
