using Luban.DataVisitors;
using Luban.Types;

namespace Luban.Datas;

public class DMap : DType
{
    public TMap Type { get; }
    public Dictionary<DType, DType> Datas { get; }

    public override string TypeName => "map";

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

    public override bool Equals(object obj)
    {
        return obj is DMap d && Datas.Count == d.Datas.Count && Datas.All(e => d.Datas.TryGetValue(e.Key, out var v) && object.Equals(e.Value, v));
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
}
