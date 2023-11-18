namespace Luban.CodeFormat.CodeStyles;

public abstract class CodeStyleBase : ICodeStyle
{
    public abstract string FormatNamespace(string ns);
    public abstract string FormatType(string typeName);
    public abstract string FormatMethod(string methodName);

    public virtual string FormatProperty(string propertyName)
    {
        return FormatMethod(propertyName);
    }

    public abstract string FormatField(string fieldName);

    public abstract string FormatEnumItemName(string enumItemName);
}
