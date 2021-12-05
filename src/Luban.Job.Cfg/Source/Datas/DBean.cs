using Luban.Job.Cfg.DataVisitors;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.Utils;
using Luban.Job.Common.Types;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Datas
{
    public class DBean : DType
    {
        public TBean TType { get; }

        public DefBean Type => (DefBean)TType.Bean;

        public DefBean ImplType { get; }

        public List<DType> Fields { get; }

        public override string TypeName => "bean";

        public DBean(TBean defType, DefBean implType, List<DType> fields)
        {
            this.TType = defType;
            this.ImplType = implType;
            this.Fields = fields;
        }

        public override bool Equals(object obj)
        {
            return obj is DBean d && string.Equals(ImplType?.FullName, d.ImplType?.FullName) && DataUtil.IsCollectionEqual(Fields, d.Fields);
        }

        public override int GetHashCode()
        {
            throw new System.NotSupportedException();
        }

        public override int CompareTo(DType other)
        {
            throw new System.NotSupportedException();
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

        public override TR Apply<T1, T2, TR>(IDataFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
        {
            return visitor.Accept(this, x, y);
        }
    }
}
