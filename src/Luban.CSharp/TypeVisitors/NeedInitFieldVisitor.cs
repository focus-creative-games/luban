using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class NeedInitFieldVisitor : AllFalseVisitor
{
    public static NeedInitFieldVisitor Ins { get; } = new();

    public override bool Accept(TString type)
    {
        return true;
    }

    public override bool Accept(TArray type)
    {
        return true;
    }

    public override bool Accept(TList type)
    {
        return true;
    }

    public override bool Accept(TSet type)
    {
        return true;
    }

    public override bool Accept(TMap type)
    {
        return true;
    }

    public override bool Accept(TBean type)
    {
        return !type.IsDynamic;
    }
}
