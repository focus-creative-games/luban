using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DeclaringCollectionRefNameFromRefGroupVisitor : ITypeFuncVisitor<int, string>
{
    public static DeclaringCollectionRefNameFromRefGroupVisitor Ins { get; } = new();

    public string Accept(TBool type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type, int index)
    {
        throw new NotImplementedException();
    }

    public string Accept(TArray type, int index)
    {
        var refTable = GetRefTable(type.ElementType, index);

        if (refTable != null)
        {
            return refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins) + "[]";
        }
        throw new Exception($"解析'{type.ElementType}[]' 的ref失败");
    }

    public string Accept(TList type, int index)
    {
        var refTable = GetRefTable(type.ElementType, index);

        if (refTable != null)
        {
            return $"{ConstStrings.ListTypeName}<{refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.ListTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TSet type, int index)
    {
        var refTable = GetRefTable(type.ElementType, index);

        if (refTable != null)
        {
            return $"{ConstStrings.HashSetTypeName}<{refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.HashSetTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TMap type, int index)
    {
        var refTable = GetRefTable(type, index);
        if (refTable != null)
        {
            return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}, {refTable.ValueTType.Apply(DeclaringTypeNameVisitor.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.HashMapTypeName}<{type.KeyType}, {type.ValueType}>' 的ref失败");
    }

    private static DefTable GetRefTable(TType type, int index)
    {
        var refTag = type.GetTag("ref");
        if (refTag == null)
        {
            refTag = type.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return null;
        }
        var refTables = TypeTemplateExtension.DoGetRefTablesFromRefGroup(refTag);
        if (index < 0 || index >= refTables.Count)
        {
            throw new Exception($"index:{index} out of range");
        }
        return refTables[index];
    }
}
