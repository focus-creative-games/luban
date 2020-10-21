using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class DMap : DType
    {
        public TMap Type { get; }
        public Dictionary<DType, DType> Datas { get; }

        public DMap(TMap type, Dictionary<DType, DType> datas)
        {
            this.Type = type;
            this.Datas = datas;

            var set = new HashSet<DType>();
            foreach (var key in datas.Keys)
            {
                if (!set.Add(key))
                {
                    throw new Exception($"set 的 value:{key} 重复");
                }
            }
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
