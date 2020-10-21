using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Common.Types;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class DArray : DType
    {
        public TArray Type { get; }
        public List<DType> Datas { get; }

        public DArray(TArray type, List<DType> datas)
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
    }
}
