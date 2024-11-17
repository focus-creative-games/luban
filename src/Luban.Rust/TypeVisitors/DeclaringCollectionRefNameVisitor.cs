using Luban.Defs;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Rust.TypeVisitors;

public class DeclaringCollectionRefNameVisitor : ITypeFuncVisitor<string>
{
    public static DeclaringCollectionRefNameVisitor Ins { get; } = new();
    public string Accept(TBool type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TArray type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstantStrings.ArrayType}<Option<std::sync::Arc<{refTable.ValueTType.Apply(RustDeclaringTypeNameVisitor.Ins)}>>>";
        }
        throw new Exception($"解析'{type.ElementType}[]' 的ref失败");
    }

    public string Accept(TList type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstantStrings.ListType}<Option<std::sync::Arc<{refTable.ValueTType.Apply(RustDeclaringTypeNameVisitor.Ins)}>>>";
        }
        throw new Exception($"解析'{ConstantStrings.ListType}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TSet type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstantStrings.ListType}<Option<std::sync::Arc<{refTable.ValueTType.Apply(RustDeclaringTypeNameVisitor.Ins)}>>>";
        }
        throw new Exception($"解析'{ConstantStrings.SetType}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TMap type)
    {
        var refTable = GetCollectionRefTable(type);
        if (refTable != null)
        {
            return $"{ConstantStrings.MapType}<{type.KeyType.Apply(RustDeclaringTypeNameVisitor.Ins)}, Option<std::sync::Arc<{refTable.ValueTType.Apply(RustDeclaringTypeNameVisitor.Ins)}>>>";
        }
        throw new Exception($"解析'{ConstantStrings.MapType}<{type.KeyType}, {type.ValueType}>' 的ref失败");
    }
    private static DefTable GetCollectionRefTable(TType type)
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
        if (GenerationContext.Current.Assembly.GetCfgTable(refTag.Replace("?", "")) is { } cfgTable)
        {
            return cfgTable;
        }
        return null;
    }
}