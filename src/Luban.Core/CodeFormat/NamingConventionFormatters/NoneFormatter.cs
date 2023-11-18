namespace Luban.CodeFormat.NamingConventionFormatters;

[NamingConvention("none")]
public class NoneFormatter : INamingConventionFormatter
{
    public string FormatName(string name)
    {
        return name;
    }
}
