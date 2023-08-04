using Luban.Core.Defs;
using Luban.Core.Types;
using Luban.Core.TypeVisitors;
using Luban.Core.Utils;

namespace Luban.CodeTarget.CSharp.TypeVisitors;

public class UnderlyingDeclaringTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static UnderlyingDeclaringTypeNameVisitor Ins { get; } = new();

    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "byte";
    }

    public string Accept(TShort type)
    {
        return "short";
    }

    public string Accept(TInt type)
    {
        return "int";
    }

    public string Accept(TLong type)
    {
        return "long";
    }

    public string Accept(TFloat type)
    {
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "double";
    }

    public virtual string Accept(TEnum type)
    {
        // return ExternalTypeUtil.CsMapperToExternalType(type.DefineEnum);
        return type.DefineEnum.FullName;
    }

    public string Accept(TString type)
    {
        return "string";
    }

    public virtual string Accept(TText type)
    {
        return "string";
    }

    public string Accept(TBean type)
    {
        // return ExternalTypeUtil.CsMapperToExternalType(type.Bean);
        return type.DefBean.FullName;
    }

    public string Accept(TArray type)
    {
        return $"{type.ElementType.Apply(this)}[]";
    }

    public string Accept(TList type)
    {
        return $"{ConstStrings.ListTypeName}<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"{ConstStrings.HashSetTypeName}<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }

    public virtual string Accept(TDateTime type)
    {
        // var mapper = ExternalTypeUtil.GetExternalTypeMappfer("datetime");
        // if (mapper != null)
        // {
        //     return mapper.TargetTypeName;
        // }
        return "long";
    }
}