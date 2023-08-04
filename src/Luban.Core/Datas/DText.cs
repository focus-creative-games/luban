using Luban.Core.DataVisitors;
using Luban.Core.Defs;

namespace Luban.Core.Datas;

public class DText : DType
{
    public string Key { get; }

    public override string TypeName => "text";

    public static DText ValueOf(string key)
    {
        return new DText(key);
    }

    private DText(string key)
    {
        Key = key;
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

    public override bool Equals(object obj)
    {
        return obj is DText o && o.Key == this.Key;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }
}