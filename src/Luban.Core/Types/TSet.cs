using Luban.Defs;
using Luban.TypeVisitors;

namespace Luban.Types;

public class TSet : TType
{
    public static TSet Create(bool isNullable, Dictionary<string, string> tags, TType elementType, bool isOrdered)
    {
        return new TSet(isNullable, tags, elementType, isOrdered);
    }

    public override string TypeName => "set";

    public override TType ElementType { get; }

    public bool IsOrderSet { get; }

    private TSet(bool isNullable, Dictionary<string, string> tags, TType elementType, bool isOrderSet) : base(isNullable, tags)
    {
        ElementType = elementType;
        IsOrderSet = isOrderSet;
    }

    public override bool TryParseFrom(string s)
    {
        throw new NotSupportedException();
    }

    public override bool IsCollection => true;

    public override void PostCompile(DefField field)
    {
        base.PostCompile(field);

        if (ElementType is TBean beanType)
        {
            throw new Exception($"bean:{field.HostType.FullName} field:{field.Name} element type can't be bean:{beanType.DefBean.FullName}");
        }
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
