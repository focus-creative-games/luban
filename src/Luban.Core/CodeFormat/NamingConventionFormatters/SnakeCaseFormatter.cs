using Luban.Utils;

namespace Luban.CodeFormat.NamingConventionFormatters;

[NamingConvention("snake")]
public class SnakeCaseFormatter : INamingConventionFormatter
{
    public string FormatName(string name)
    {
        return TypeUtil.ToUnderScores(name);
    }
}
