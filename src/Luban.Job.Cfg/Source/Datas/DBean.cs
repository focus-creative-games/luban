using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class DBean : DType
    {
        public DefBean Type { get; }

        public DefBean ImplType { get; }

        public List<DType> Fields { get; }

        public override string TypeName => "bean";

        public DBean(DefBean defType, DefBean implType, List<DType> fields)
        {
            this.Type = defType;
            this.ImplType = implType;
            this.Fields = fields;
        }

        public DType GetField(string fieldName)
        {
            if (ImplType.TryGetField(fieldName, out var _, out var findex))
            {
                return Fields[findex];
            }
            else
            {
                return null;
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
