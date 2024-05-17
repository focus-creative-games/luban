using Luban.Defs;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Types;

public class TEnum : TType
{
    public static TEnum Create(bool isNullable, DefEnum defEnum, Dictionary<string, string> tags)
    {
        return new TEnum(isNullable, DefUtil.MergeTags(defEnum.Tags, tags), defEnum);
    }

    public override string TypeName => "enum";

    public override bool IsEnum => true;

    public DefEnum DefEnum { get; }

    private TEnum(bool isNullable, Dictionary<string, string> tags, DefEnum defEnum) : base(isNullable, tags)
    {
        this.DefEnum = defEnum;
    }

    public override bool TryParseFrom(string s)
    {
        throw new NotSupportedException();
    }

    public override void Apply<T>(ITypeActionVisitor<T> visitor, T x)
    {
        visitor.Accept(this, x);
    }

    public override void Apply<T1, T2>(ITypeActionVisitor<T1, T2> visitor, T1 x, T2 y)
    {
        visitor.Accept(this, x, y);
    }

    public override TR Apply<TR>(ITypeFuncVisitor<TR> visitor)
    {
        return visitor.Accept(this);
    }

    public override TR Apply<T, TR>(ITypeFuncVisitor<T, TR> visitor, T x)
    {
        return visitor.Accept(this, x);
    }

    public override TR Apply<T1, T2, TR>(ITypeFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
    {
        return visitor.Accept(this, x, y);
    }

    public override TR Apply<T1, T2, T3, TR>(ITypeFuncVisitor<T1, T2, T3, TR> visitor, T1 x, T2 y, T3 z)
    {
        return visitor.Accept(this, x, y, z);
    }

    public override TR Apply<T1, T2, T3, T4, TR>(ITypeFuncVisitor<T1, T2, T3, T4, TR> visitor, T1 x, T2 y, T3 z, T4 w)
    {
        return visitor.Accept(this, x, y, z, w);
    }
}