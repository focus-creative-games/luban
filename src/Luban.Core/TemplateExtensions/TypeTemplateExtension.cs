using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class TypeTemplateExtension : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }
    
    public static string FormatFieldName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatField(name);
    }

    public static string FormatPropertyName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatProperty(name);
    }

    public static string FormatEnumItemName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatEnumItemName(name);
    }
    
    public static bool CanGenerateRef(DefField field)
    {
        if (field.CType.IsCollection)
        {
            return false;
        }
        if (field.CType.GetTag("ref") is { } value && GenerationContext.Current.Assembly.GetCfgTable(value) is { } cfgTable)
        {
            return true;
        }
        return false;
    }
}