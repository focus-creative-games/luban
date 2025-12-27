using Luban.CodeFormat;
using Luban.Defs;
using Luban.Kotlin.TypeVisitors;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Kotlin.TemplateExtensions;

public class KotlinCommonTemplateExtension : ScriptObject
{
    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(KotlinDeclaringTypeNameVisitor.Ins);
    }

    public static string DeclaringBoxTypeName(TType type)
    {
        return type.Apply(KotlinDeclaringBoxTypeNameVisitor.Ins);
    }

    public static string ClassModifier(DefBean type)
    {
        // 抽象类型使用 abstract
        if (type.IsAbstractType)
        {
            return "abstract";
        }

        // 如果有子类（作为父类被继承），必须使用 open 关键字
        // 因为 Kotlin 中类默认是 final 的
        if (type.Children != null && type.Children.Count > 0)
        {
            return "open";
        }

        // 普通类不需要修饰符（默认是 final）
        return "";
    }

    public static string GetterName(string name)
    {
        return name; // Kotlin uses property syntax, no need for getter prefix
    }

    public static string GetterName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatMethod(name);
    }

    public static string PropertyModifier(DefField field)
    {
        // 使用统一的逻辑处理字段声明
        return PropertyDeclaration(field);
    }

    /// <summary>
    /// 用于构造方法参数的字段声明（不使用 lateinit）
    /// </summary>
    public static string PropertyModifierInConstructor(DefField field)
    {
        return "var";
    }

    public static bool IsPrimitiveType(TType type)
    {
        // 判断是否为基本数据类型（值类型）
        // 注意：String 和枚举在 Kotlin 中是引用类型，不是基本类型
        return type switch
        {
            TBool => true,
            TByte => true,
            TShort => true,
            TInt => true,
            TLong => true,
            TFloat => true,
            TDouble => true,
            TDateTime => true, // DateTime 在 Kotlin 中映射为 Long
            _ => false
        };
    }

    public static string PropertyDeclaration(DefField field)
    {
        // 检查是否需要添加 lateinit
        // lateinit 只能用于非基本类型的 var 属性
        // 注意：只有在类体内声明的字段才需要 lateinit
        if (NeedLateinit(field))
        {
            return "lateinit var";
        }

        return "var";
    }

    /// <summary>
    /// 用于构造方法参数的字段声明（不使用 lateinit）
    /// </summary>
    public static string PropertyDeclarationInConstructor(DefField field)
    {
        return "var";
    }

    /// <summary>
    /// 判断字段是否需要 lateinit 修饰符
    /// lateinit 适用于：
    /// 1. 引用类型（非基本类型、非可空类型）
    /// 2. 不是 _buf 字段
    /// </summary>
    public static bool NeedLateinit(DefField field)
    {
        // _buf 字段不需要 lateinit（在构造函数中初始化）
        if (field.Name == "_buf")
        {
            return false;
        }

        // 基本类型不能使用 lateinit
        if (IsPrimitiveType(field.CType))
        {
            return false;
        }

        // 可空类型不能使用 lateinit
        if (field.CType.IsNullable)
        {
            return false;
        }

        // 引用类型（String、集合、Bean、Enum 等）需要 lateinit
        return field.CType is TString || field.CType.IsCollection || field.CType.IsBean || field.CType.IsEnum;
    }

    /// <summary>
    /// 获取Kotlin数组初始化的默认值
    /// 基本类型、String、DateTime使用默认值
    /// 对象类型使用null（需要nullable）
    /// </summary>
    public static string GetArrayDefaultValue(TType elementType)
    {
        return elementType switch
        {
            TBool => "false",
            TByte => "0",
            TShort => "0",
            TInt => "0",
            TLong => "0L",
            TFloat => "0f",
            TDouble => "0.0",
            TString => "\"\"",
            TDateTime => "0L",
            _ => "null"
        };
    }

    /// <summary>
    /// 获取字段的默认值（用于字段声明时的初始化）
    /// 基本类型返回默认值，引用类型不返回（使用 lateinit）
    /// </summary>
    public static string GetFieldDefaultValue(DefField field)
    {
        // 可空类型不需要默认值（可以是 null）
        if (field.CType.IsNullable)
        {
            return "";
        }

        // 基本类型需要默认值
        if (IsPrimitiveType(field.CType))
        {
            return field.CType switch
            {
                TBool => " = false",
                TByte => " = 0",
                TShort => " = 0",
                TInt => " = 0",
                TLong => " = 0L",
                TFloat => " = 0f",
                TDouble => " = 0.0",
                TDateTime => " = 0L",
                _ => ""
            };
        }

        // 引用类型使用 lateinit，不需要默认值
        return "";
    }
}
