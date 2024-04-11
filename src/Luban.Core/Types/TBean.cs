using Luban.Defs;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Types;

public class TBean : TType
{
    public static TBean Create(bool isNullable, DefBean defBean, Dictionary<string, string> tags)
    {
        // TODO
        return new TBean(isNullable, DefUtil.MergeTags(defBean.Tags, tags), defBean);
    }

    public DefBean DefBean { get; set; }

    public override string TypeName => "bean";

    private TBean(bool isNullable, Dictionary<string, string> attrs, DefBean defBean) : base(isNullable, attrs)
    {
        this.DefBean = defBean;
    }

    public override bool TryParseFrom(string s)
    {
        throw new NotSupportedException();
    }

    public bool IsDynamic => DefBean.IsAbstractType;

    public override bool IsBean => true;

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
