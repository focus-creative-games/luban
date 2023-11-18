using Luban.Utils;

namespace Luban.CodeFormat.NamingConventionFormatters;

[NamingConvention("camel")]
public class CamelCaseFormatter : INamingConventionFormatter
{
    public string FormatName(string fieldName)
    {
        return TypeUtil.ToCamelCase(fieldName);
    }
}
