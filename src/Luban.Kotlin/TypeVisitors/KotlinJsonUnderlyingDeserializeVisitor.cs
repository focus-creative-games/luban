using Luban.Datas;
using Luban.Kotlin.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Kotlin.TypeVisitors;

public class KotlinJsonUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static KotlinJsonUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string json, string x, int depth)
    {
        return $"{x} = {json}.asBoolean";
    }

    public string Accept(TByte type, string json, string x, int depth)
    {
        return $"{x} = {json}.asByte";
    }

    public string Accept(TShort type, string json, string x, int depth)
    {
        return $"{x} = {json}.asShort";
    }

    public string Accept(TInt type, string json, string x, int depth)
    {
        return $"{x} = {json}.asInt";
    }

    public string Accept(TLong type, string json, string x, int depth)
    {
        return $"{x} = {json}.asLong";
    }

    public string Accept(TFloat type, string json, string x, int depth)
    {
        return $"{x} = {json}.asFloat";
    }

    public string Accept(TDouble type, string json, string x, int depth)
    {
        return $"{x} = {json}.asDouble";
    }

    public string Accept(TEnum type, string json, string x, int depth)
    {
        // 支持直接使用枚举值，而不是转换为整数
        string src = $"{json}.asInt";
        string constructor = type.DefEnum.TypeConstructorWithTypeMapper();
        string enumValue = $"{type.DefEnum.FullNameWithTopModule}.fromValue({src})";
        string enumTypeName = type.DefEnum.FullNameWithTopModule;
        // 修改：使用Elvis操作符简化空值检查，并处理构造函数情况
        if (string.IsNullOrEmpty(constructor))
        {
            return $"{x} = {enumValue} ?: throw IllegalArgumentException(\"Invalid enum value for {enumTypeName}: ${{{src}}}\")";
        }
        else
        {
            return $"{x} = {constructor}({enumValue} ?: throw IllegalArgumentException(\"Invalid enum value for {enumTypeName}: ${{{src}}}\"))";
        }
    }

    public string Accept(TString type, string json, string x, int depth)
    {
        return $"{x} = {json}.asString";
    }

    public string Accept(TDateTime type, string json, string x, int depth)
    {
        return $"{x} = {json}.asLong";
    }

    public string Accept(TBean type, string json, string x, int depth)
    {
        return $"{x} = {type.DefBean.FullNameWithTopModule}.deserialize({json}.asJsonObject)";
    }

    public string Accept(TArray type, string json, string x, int depth)
    {
        string __n = $"__n{depth}";
        string __e = $"__e{depth}";
        string __v = $"__v{depth}";
        string __index = $"__index{depth}";
        string typeStr = type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins);
        string boxTypeStr = type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins);
        
        // 对于引用类型，使用arrayOfNulls创建，然后通过@Suppress抑制类型转换警告
        // 对于基本类型，使用默认值初始化
        string arrayInit;
        if (type.ElementType is TBean || (type.ElementType is TBean bean && bean.DefBean.IsAbstractType))
        {
            // 引用类型：使用arrayOfNulls，运行时会正确填充
            arrayInit = $"@Suppress(\"UNCHECKED_CAST\") (arrayOfNulls<{boxTypeStr}>({__n}) as Array<{boxTypeStr}>)";
        }
        else
        {
            // 基本类型：使用默认值初始化
            var defaultValue = KotlinCommonTemplateExtension.GetArrayDefaultValue(type.ElementType);
            arrayInit = $"Array({__n}) {{ {defaultValue} }}";
        }
        
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; val {__n} = _json{depth}_.size(); {x} = {arrayInit}; var {__index} = 0; for ({__e} in _json{depth}_) {{ var {__v}: {typeStr}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}[{__index}++] = {__v} }} }}";
    }

    public string Accept(TList type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableListOf(); for ({__e} in _json{depth}_) {{ var {__v}: {type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}.add({__v}) }} }}";
    }

    public string Accept(TSet type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableSetOf(); for ({__e} in _json{depth}_) {{ var {__v}: {type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, __e, __v, depth + 1)}; {x}.add({__v}) }} }}";
    }

    public string Accept(TMap type, string json, string x, int depth)
    {
        string __e = $"_e{depth}";
        string __k = $"_k{depth}";
        string __v = $"_v{depth}";
        return $"run {{ val _json{depth}_ = {json}.asJsonArray; {x} = mutableMapOf(); for ({__e} in _json{depth}_) {{ var {__k}: {type.KeyType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, $"{__e}.asJsonArray[0]", __k, depth + 1)}; var {__v}: {type.ValueType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, $"{__e}.asJsonArray[1]", __v, depth + 1)}; {x}[{__k}] = {__v} }} }}";
    }
}
