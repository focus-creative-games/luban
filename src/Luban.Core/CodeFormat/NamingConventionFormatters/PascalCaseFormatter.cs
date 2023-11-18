using Luban.Utils;

namespace Luban.CodeFormat.NamingConventionFormatters;

[NamingConvention("pascal")]
public class PascalCaseFormatter : INamingConventionFormatter
{
    public string FormatName(string name)
    {
        return TypeUtil.ToPascalCase(name);
    }
}
