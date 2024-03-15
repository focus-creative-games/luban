using Luban.DataVisitors;
using Luban.Types;
using Luban.Utils;

namespace Luban.Datas;

public class DList : DType
{
    public TList Type { get; }
    public List<DType> Datas { get; }

    public override string TypeName => "list";

    public DList(TList type, List<DType> datas)
    {
        this.Type = type;
        this.Datas = datas;
    }


    public override bool Equals(object obj)
    {
        return obj is DList d && DataUtil.IsCollectionEqual(Datas, d.Datas);
    }

    public override int GetHashCode()
    {
        throw new System.NotSupportedException();
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

    public override TR Apply<TR>(IDataFuncVisitor2<TR> visitor, TType type)
    {
        return visitor.Accept(this, type);
    }

    public override TR Apply<T, TR>(IDataFuncVisitor2<T, TR> visitor, TType type, T x)
    {
        return visitor.Accept(this, type, x);
    }

    public override TR Apply<T1, T2, TR>(IDataFuncVisitor2<T1, T2, TR> visitor, TType type, T1 x, T2 y)
    {
        return visitor.Accept(this, type, x, y);
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
