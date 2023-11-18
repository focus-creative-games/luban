using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.TypeVisitors;

class DeepCompareTypeDefine : ITypeFuncVisitor<TType, Dictionary<DefTypeBase, bool>, HashSet<DefTypeBase>, bool>
{
    public static DeepCompareTypeDefine Ins { get; } = new();

    private bool IsBaseDefineEqual(DefBean a, DefBean b)
    {
        if (!a.Name.Equals(b.Name))
        {
            return false;
        }
        if (!a.Namespace.Equals(b.Namespace))
        {
            return false;
        }

        if (a.Parent != b.Parent)
        {
            return false;
        }

        if (a.Alias != b.Alias)
        {
            return false;
        }
        if (a.IsMultiRow != b.IsMultiRow)
        {
            return false;
        }
        if (a.Sep != b.Sep)
        {
            return false;
        }
        return true;
    }

    public bool Compare(DefBean a, DefBean b, Dictionary<DefTypeBase, bool> ctx, HashSet<DefTypeBase> inWalk)
    {
        bool setupNotEqual()
        {
            ctx.Add(a, false);
            return false;
        }

        if (ctx.TryGetValue(a, out var e))
        {
            return e;
        }
        if (inWalk.Contains(a))
        {
            return true;
        }

        if (!IsBaseDefineEqual(a, b))
        {
            return setupNotEqual();
        }

        inWalk.Add(a);

        try
        {
            if (a.Fields.Count != b.Fields.Count)
            {
                return setupNotEqual();
            }

            for (int i = 0; i < a.Fields.Count; i++)
            {
                var f1 = a.Fields[i];
                var f2 = b.Fields[i];
                if (f1.Name != f2.Name
                    || f1.NeedExport() != f2.NeedExport()
                    || f1.CType.IsNullable != f2.CType.IsNullable
                    || f1.CType.GetType() != f2.CType.GetType()
                   )
                {
                    return setupNotEqual();
                }

                if (!f1.CType.Apply(this, f2.CType, ctx, inWalk))
                {
                    return setupNotEqual();
                }
            }


            var parentType = (DefBean)a.ParentDefType;
            if (parentType != null && !Compare(parentType, (DefBean)b.ParentDefType, ctx, inWalk))
            {
                return setupNotEqual();
            }
            if (a.Children == null)
            {
                if (b.Children != null)
                {
                    return setupNotEqual();
                }
            }
            else
            {
                if (b.Children == null || a.Children.Count != b.Children.Count)
                {
                    return setupNotEqual();
                }
                else
                {
                    int index = 0;
                    foreach (var c in a.Children)
                    {
                        if (!Compare((DefBean)c, (DefBean)b.Children[index++], ctx, inWalk))
                        {
                            return setupNotEqual();
                        }
                    }
                }
            }

            ctx.Add(a, true);
            return true;
        }
        finally
        {
            //inWalk.Remove(a);
        }
    }

    public bool Accept(TBool type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TByte type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TShort type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TInt type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TLong type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TFloat type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TDouble type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TEnum type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        var a = type.DefEnum;
        var b = ((TEnum)x).DefEnum;
        if (y.TryGetValue(a, out var v))
        {
            return v;
        }


        var same = a.FullName == b.FullName
                   && a.IsFlags == b.IsFlags
                   && a.IsUniqueItemId == b.IsUniqueItemId
                   && IsEnumItemEquals(a.Items, b.Items);
        y.Add(a, same);
        return same;
    }

    private bool IsEnumItemEquals(List<DefEnum.Item> a, List<DefEnum.Item> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }
        for (int i = 0; i < a.Count; i++)
        {
            var ia = a[i];
            var ib = b[i];
            if (ia.Name != ib.Name || ia.Value != ib.Value || ia.Alias != ib.Alias)
            {
                return false;
            }
        }
        return true;
    }

    public bool Accept(TString type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TDateTime type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return true;
    }

    public bool Accept(TBean type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return Compare(type.DefBean, ((TBean)x).DefBean, y, z);
    }

    public bool Accept(TArray type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return type.ElementType.Apply(this, ((TArray)x).ElementType, y, z);
    }

    public bool Accept(TList type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return type.ElementType.Apply(this, ((TList)x).ElementType, y, z);
    }

    public bool Accept(TSet type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        return type.ElementType.Apply(this, ((TSet)x).ElementType, y, z);
    }

    public bool Accept(TMap type, TType x, Dictionary<DefTypeBase, bool> y, HashSet<DefTypeBase> z)
    {
        TMap m = (TMap)x;
        return type.KeyType.Apply(this, m.KeyType, y, z) && type.ValueType.Apply(this, m.ValueType, y, z);
    }
}
