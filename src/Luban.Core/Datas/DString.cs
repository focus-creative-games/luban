using Luban.DataVisitors;
using Luban.Types;

namespace Luban.Datas;

public class DString : DType<string>
{
    private static readonly DString s_empty = new("");

    public static DString ValueOf(TType type, string s)
    {
        if (s.Length == 0)
        {
            return s_empty;
        }

        string escapeMode = type.GetTagOrDefault("escape", "0")?.ToLowerInvariant();
        switch (escapeMode)
        {
            case "0":
            case "false":
                return new DString(s);
            case "1":
            case "true":
                return new DString(System.Text.RegularExpressions.Regex.Unescape(s));
            default:
                throw new Exception($"unknown escape mode:{escapeMode}");
        }
    }

    public override string TypeName => "string";

    private DString(string x) : base(x)
    {
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

    public override bool Equals(object obj)
    {
        return obj is DString o && o.Value == this.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override int CompareTo(DType other)
    {
        if (other is DString d)
        {
            return String.Compare(this.Value, d.Value, StringComparison.Ordinal);
        }
        throw new System.NotSupportedException();
    }
}
