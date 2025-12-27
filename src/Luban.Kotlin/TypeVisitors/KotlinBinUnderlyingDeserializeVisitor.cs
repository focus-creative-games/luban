using Luban.Kotlin.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.Kotlin.TypeVisitors;

class KotlinBinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static KotlinBinUnderlyingDeserializeVisitor Ins { get; } = new();

    public string Accept(TBool type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readBool()";
    }

    public string Accept(TByte type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readByte()";
    }

    public string Accept(TShort type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readShort()";
    }

    public string Accept(TInt type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readInt()";
    }

    public string Accept(TLong type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readLong()";
    }

    public string Accept(TFloat type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readFloat()";
    }

    public string Accept(TDouble type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readDouble()";
    }

    public string Accept(TEnum type, string bufName, string fieldName, int depth)
    {
        string src = $"{bufName}.readInt()";
        string constructor = type.DefEnum.TypeConstructorWithTypeMapper();
        string enumValue = $"{type.DefEnum.FullNameWithTopModule}.fromValue({src})";
        string enumTypeName = type.DefEnum.FullNameWithTopModule;
        // 修改：使用Elvis操作符简化空值检查，并处理构造函数情况
        if (string.IsNullOrEmpty(constructor))
        {
            return $"{fieldName} = {enumValue} ?: throw IllegalArgumentException(\"Invalid enum value for {enumTypeName}: ${{{src}}}\")";
        }
        else
        {
            return $"{fieldName} = {constructor}({enumValue} ?: throw IllegalArgumentException(\"Invalid enum value for {enumTypeName}: ${{{src}}}\"))";
        }
    }

    public string Accept(TString type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readString()";
    }

    public string Accept(TBean type, string bufName, string fieldName, int depth)
    {
        string src = $"{type.DefBean.FullNameWithTopModule}.deserialize({bufName})";
        string constructor = type.DefBean.TypeConstructorWithTypeMapper();
        return $"{fieldName} = {(string.IsNullOrEmpty(constructor) ? src : $"{constructor}({src})")}";
    }

    public string Accept(TArray type, string bufName, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        var typeStr = $"{type.ElementType.Apply(KotlinDeclaringTypeNameVisitor.Ins)}";
        var boxTypeStr = $"{type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}";
        
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
        
        return $"run {{ val {__n} = {bufName}.readSize(); {fieldName} = {arrayInit}; for ({__i} in 0 until {__n}) {{ var {__e}: {typeStr}; {type.ElementType.Apply(this, bufName, __e, depth + 1)}; {fieldName}[{__i}] = {__e} }} }}";
    }

    public string Accept(TList type, string bufName, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        return $"run {{ val {__n} = {bufName}.readSize(); {fieldName} = mutableListOf(); for ({__i} in 0 until {__n}) {{ var {__e}: {type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufName, __e, depth + 1)}; {fieldName}.add({__e}) }} }}";
    }

    public string Accept(TSet type, string bufName, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __e = $"__e{depth}";
        var __i = $"__i{depth}";
        return $"run {{ val {__n} = {bufName}.readSize(); {fieldName} = mutableSetOf(); for ({__i} in 0 until {__n}) {{ var {__e}: {type.ElementType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufName, __e, depth + 1)}; {fieldName}.add({__e}) }} }}";
    }

    public string Accept(TMap type, string bufName, string fieldName, int depth)
    {
        var __n = $"__n{depth}";
        var __k = $"__k{depth}";
        var __v = $"__v{depth}";
        var __i = $"__i{depth}";
        return $"run {{ val {__n} = {bufName}.readSize(); {fieldName} = mutableMapOf(); for ({__i} in 0 until {__n}) {{ var {__k}: {type.KeyType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, bufName, __k, depth + 1)}; var {__v}: {type.ValueType.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, bufName, __v, depth + 1)}; {fieldName}[{__k}] = {__v} }} }}";
    }

    public string Accept(TDateTime type, string bufName, string fieldName, int depth)
    {
        return $"{fieldName} = {bufName}.readLong()";
    }
}
