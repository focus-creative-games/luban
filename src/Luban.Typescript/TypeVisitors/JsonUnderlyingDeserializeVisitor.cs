using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class JsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static JsonUnderlyingDeserializeVisitor Ins { get; } = new JsonUnderlyingDeserializeVisitor();

    public string Accept(TBool type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TByte type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TShort type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TInt type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TLong type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TFloat type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TDouble type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TEnum type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TString type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TDateTime type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = {jsonVarName}";
    }

    public string Accept(TBean type, string jsonVarName, string fieldName, int depth)
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

    public string Accept(TArray type, string jsonVarName, string fieldName, int depth)
    {
        return $"{{ {fieldName} = []; for(let _ele{depth} of {jsonVarName}) {{ let _e{depth}; {type.ElementType.Apply(this, $"_ele{depth}", $"_e{depth}", depth + 1)}; {fieldName}.push(_e{depth});}}}}";
    }

    public string Accept(TList type, string jsonVarName, string fieldName, int depth)
    {
        return $"{{ {fieldName} = []; for(let _ele{depth} of {jsonVarName}) {{ let _e{depth}; {type.ElementType.Apply(this, $"_ele{depth}", $"_e{depth}", depth + 1)}; {fieldName}.push(_e{depth});}}}}";
    }

    public string Accept(TSet type, string jsonVarName, string fieldName, int depth)
    {
        if (type.Apply(SimpleJsonTypeVisitor.Ins))
        {
            return $"{fieldName} = {jsonVarName}";
        }
        else
        {
            return $"{{ {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _ele{depth} of {jsonVarName}) {{ let _e{depth}; {type.ElementType.Apply(this, $"_ele{depth}", $"_e{depth}", depth + 1)}; {fieldName}.add(_e{depth});}}}}";
        }
    }

    public string Accept(TMap type, string jsonVarName, string fieldName, int depth)
    {
        return $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _entry{depth}_ of {jsonVarName}) {{ let _k{depth}; {type.KeyType.Apply(this, $"_entry{depth}_[0]", $"_k{depth}", depth + 1)};  let _v{depth};  {type.ValueType.Apply(this, $"_entry{depth}_[1]", $"_v{depth}", depth + 1)}; {fieldName}.set(_k{depth}, _v{depth});  }}";
    }
}
