using Luban.Defs;
using Luban.Types;

namespace Luban.TypeVisitors;

class RefTypeVisitor : ITypeActionVisitor<Dictionary<string, DefTypeBase>>
{
    public static RefTypeVisitor Ins { get; } = new();

    public void Accept(TBool type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TByte type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TShort type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TInt type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TLong type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TFloat type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TDouble type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TEnum type, Dictionary<string, DefTypeBase> x)
    {
        x.TryAdd(type.DefEnum.FullName, type.DefEnum);
    }

    public void Accept(TString type, Dictionary<string, DefTypeBase> x)
    {

    }

    public void Accept(TDateTime type, Dictionary<string, DefTypeBase> x)
    {

    }

    void Walk(DefBean type, Dictionary<string, DefTypeBase> types)
    {
        if (types.TryAdd(type.FullName, type))
        {
            foreach (var f in type.Fields)
            {
                f.CType.Apply(this, types);
            }
            if (type.Children != null)
            {
                foreach (DefBean c in type.Children)
                {
                    Walk(c, types);
                }
            }
        }
    }

    public void Accept(TBean type, Dictionary<string, DefTypeBase> x)
    {
        var root = (DefBean)type.DefBean.RootDefType;
        Walk(root, x);
    }

    public void Accept(TArray type, Dictionary<string, DefTypeBase> x)
    {
        type.ElementType.Apply(this, x);
    }

    public void Accept(TList type, Dictionary<string, DefTypeBase> x)
    {
        type.ElementType.Apply(this, x);
    }

    public void Accept(TSet type, Dictionary<string, DefTypeBase> x)
    {
        type.ElementType.Apply(this, x);
    }

    public void Accept(TMap type, Dictionary<string, DefTypeBase> x)
    {
        type.KeyType.Apply(this, x);
        type.ValueType.Apply(this, x);
    }
}
