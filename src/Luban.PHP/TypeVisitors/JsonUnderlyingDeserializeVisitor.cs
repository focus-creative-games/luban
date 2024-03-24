using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.PHP.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.PHP.TypeVisitors;

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
        string fullName = PHPCommonTemplateExtension.FullName(type.DefBean);
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {fullName}::constructFrom({jsonVarName})";
        }
        else
        {
            return $"{fieldName} = new {fullName}({jsonVarName})";
        }
    }

    public string Accept(TArray type, string jsonVarName, string fieldName, int depth)
    {
        return $"{{ {fieldName} = []; foreach ({jsonVarName} as $_ele{depth}) {{ {type.ElementType.Apply(this, $"$_ele{depth}", $"$_e{depth}", depth + 1)}; array_push({fieldName}, $_e{depth});}} }}";
    }

    public string Accept(TList type, string jsonVarName, string fieldName, int depth)
    {
        return $"{{ {fieldName} = []; foreach ({jsonVarName} as $_ele{depth}) {{ {type.ElementType.Apply(this, $"$_ele{depth}", $"$_e{depth}", depth + 1)}; array_push({fieldName}, $_e{depth});}} }}";
    }

    public string Accept(TSet type, string jsonVarName, string fieldName, int depth)
    {
        if (type.ElementType.Apply(SimpleJsonTypeVisitor.Ins))
        {
            return $"{fieldName} = {jsonVarName}";
        }
        return $"{{ {fieldName} = []; foreach ({jsonVarName} as $_ele{depth}) {{ {type.ElementType.Apply(this, $"$_ele{depth}", $"$_e{depth}", depth + 1)}; array_push({fieldName}, $_e{depth});}} }}";
    }

    public string Accept(TMap type, string jsonVarName, string fieldName, int depth)
    {
        if (type.KeyType.Apply(SimpleJsonTypeVisitor.Ins) && type.ValueType.Apply(SimpleJsonTypeVisitor.Ins))
        {
            return $"{fieldName} = {jsonVarName}";
        }
        return $"{{{fieldName} = []; foreach ({jsonVarName} as $e{depth}) {{ {type.KeyType.Apply(this, $"$e{depth}[0]", $"$_k{depth}", depth + 1)}; {type.ValueType.Apply(this, $"$e{depth}[1]", $"$_v{depth}", depth + 1)}; {fieldName}[$_k{depth}] = $_v{depth}; }} }}";
    }
}
