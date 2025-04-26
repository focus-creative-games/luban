
using Luban.DataVisitors;
using Luban.Types;

namespace Luban.Datas;

public abstract class DType : System.IComparable<DType>
{
    public abstract void Apply<T>(IDataActionVisitor<T> visitor, T x);

    public abstract void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y);

    public abstract void Apply<T>(IDataActionVisitor2<T> visitor, TType type, T x);

    public abstract void Apply<T1, T2>(IDataActionVisitor2<T1, T2> visitor, TType type, T1 x, T2 y);

    public abstract TR Apply<TR>(IDataFuncVisitor<TR> visitor);

    public abstract TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x);

    public abstract TR Apply<T1, T2, TR>(IDataFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y);

    public abstract TR Apply<TR>(IDataFuncVisitor2<TR> visitor, TType type);

    public abstract TR Apply<T, TR>(IDataFuncVisitor2<T, TR> visitor, TType type, T x);

    public abstract TR Apply<T1, T2, TR>(IDataFuncVisitor2<T1, T2, TR> visitor, TType type, T1 x, T2 y);

    public abstract string TypeName { get; }


    public List<DType> Datas { get; protected set; }

    public override string ToString()
    {
        return this.Apply(ToStringVisitor.Ins);
    }

    public virtual int CompareTo(DType other)
    {
        throw new System.NotSupportedException();
    }
}

public abstract class DType<T> : DType
{
    public T Value { get; }

    protected DType(T value)
    {
        Value = value;
    }
}
