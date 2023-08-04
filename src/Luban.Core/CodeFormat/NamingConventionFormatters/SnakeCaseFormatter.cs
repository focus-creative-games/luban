using Luban.Core.Utils;

namespace Luban.Core.CodeFormat.NamingConventionFormatters;

[NamingConvention("snake")]
public class SnakeCaseFormatter : INamingConventionFormatter
{
    public string FormatName(string name)
    {
        return TypeUtil.ToUnderScores(name);
    }
}