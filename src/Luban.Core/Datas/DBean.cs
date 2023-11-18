using Luban.DataVisitors;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.Datas;

public class DBean : DType
{
    public TBean TType { get; }

    public DefBean Type => (DefBean)TType.DefBean;

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

    public override void Apply<T>(IDataActionVisitor2<T> visitor, TType type, T x)
    {
        visitor.Accept(this, type, x);
    }

    public override void Apply<T1, T2>(IDataActionVisitor2<T1, T2> visitor, TType type, T1 x, T2 y)
    {
        visitor.Accept(this, type, x, y);
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
