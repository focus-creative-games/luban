namespace Luban.CodeFormat.NamingConventionFormatters;

[NamingConvention("upper")]
public class UpperCaseFormatter : INamingConventionFormatter
{
    public string FormatName(string name)
    {
        return name.ToUpperInvariant();
    }
}
