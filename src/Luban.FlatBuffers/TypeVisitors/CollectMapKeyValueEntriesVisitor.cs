using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.FlatBuffers.TypeVisitors;

public class MapKeyValueEntryCollection
{
    public Dictionary<string, TMap> KeyValueEntries { get; } = new();

    public HashSet<string> VisitedTypes { get; } = new();
}

public class CollectMapKeyValueEntriesVisitor : DecoratorActionVisitor<MapKeyValueEntryCollection>
{
    public static CollectMapKeyValueEntriesVisitor Ins { get; } = new();


    public override void DoAccept(TType type, MapKeyValueEntryCollection x)
    {

    }

    public void Accept(DefBean bean, MapKeyValueEntryCollection x)
    {
        if (!x.VisitedTypes.Add(bean.FullName))
        {
            return;
        }
        if (bean.IsAbstractType)
        {
            foreach (var c in bean.HierarchyNotAbstractChildren)
            {
                Accept(c, x);
            }
        }
        else
        {
            foreach (var field in bean.HierarchyFields)
            {
                field.CType.Apply(this, x);
            }
        }
    }

    private static string MakeKeyValueType(TMap type)
    {
        return $"{type.KeyType.Apply(FlatBuffersTypeNameVisitor.Ins)}_{type.ValueType.Apply(FlatBuffersTypeNameVisitor.Ins)}";
    }

    public override void Accept(TBean type, MapKeyValueEntryCollection x)
    {
        Accept(type.DefBean, x);
    }

    public override void Accept(TArray type, MapKeyValueEntryCollection x)
    {
        if (type.ElementType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }

    public override void Accept(TList type, MapKeyValueEntryCollection x)
    {
        if (type.ElementType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }

    public override void Accept(TMap type, MapKeyValueEntryCollection x)
    {
        x.KeyValueEntries[MakeKeyValueType(type)] = type;
        if (type.ValueType is TBean tbean)
        {
            tbean.Apply(this, x);
        }
    }
}
