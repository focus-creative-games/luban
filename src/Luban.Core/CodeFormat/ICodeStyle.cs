namespace Luban.CodeFormat;

public interface ICodeStyle
{
    string FormatNamespace(string ns);

    string FormatType(string typeName);

    string FormatMethod(string methodName);

    string FormatProperty(string propertyName);

    string FormatField(string fieldName);

    string FormatEnumItemName(string enumItemName);
}
