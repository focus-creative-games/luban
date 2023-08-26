using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class JsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static JsonUnderlyingDeserializeVisitor Ins { get; } = new JsonUnderlyingDeserializeVisitor();

    public string Accept(TBool type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TByte type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TShort type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TInt type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TLong type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TFloat type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TDouble type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TEnum type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TString type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TDateTime type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TBean type, string jsonVarName, string fieldName)
    {
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {type.DefBean.FullName}.constructorFrom({jsonVarName})";
        }
        else
        {
            return $"{fieldName} = new {type.DefBean.FullName}({jsonVarName})";
        }
    }

    public string Accept(TArray type, string jsonVarName, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let _ele of {jsonVarName}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
    }

    public string Accept(TList type, string jsonVarName, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let _ele of {jsonVarName}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
    }

    public string Accept(TSet type, string jsonVarName, string fieldName)
    {
        if (type.Apply(SimpleJsonTypeVisitor.Ins))
        {
            return $"{fieldName} = {jsonVarName}";
        }
        else
        {
            return $"{{ {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _ele of {jsonVarName}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.add(_e);}}}}";
        }
    }

    public string Accept(TMap type, string jsonVarName, string fieldName)
    {
        return $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _entry_ of {jsonVarName}) {{ let _k; {type.KeyType.Apply(this, "_entry_[0]", "_k")};  let _v;  {type.ValueType.Apply(this, "_entry_[1]", "_v")}; {fieldName}.set(_k, _v);  }}";
    }
}