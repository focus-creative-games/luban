using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.DataExporter.Builtin.Json;

public class SimpleJsonTypeVisitor : AllTrueVisitor
{
    public static SimpleJsonTypeVisitor Ins { get; } = new();

    public override bool Accept(TEnum type)
    {
        return false;
    }

    public override bool Accept(TBean type)
    {
        //return type.Bean.IsNotAbstractType && type.Bean.HierarchyFields.All(f => f.CType.Apply(this));
        return false;
    }

    public override bool Accept(TArray type)
    {
        return type.ElementType.Apply(this);
    }

    public override bool Accept(TList type)
    {
        return type.ElementType.Apply(this);
    }

    public override bool Accept(TSet type)
    {
        return type.ElementType.Apply(this);
    }

    public override bool Accept(TMap type)
    {
        return false;
    }
}
