namespace Luban.CodeFormat.CodeStyles;

public class ConfigurableCodeStyle : CodeStyleBase
{
    private readonly INamingConventionFormatter _namespaceFormatter;
    private readonly INamingConventionFormatter _typeFormatter;
    private readonly INamingConventionFormatter _methodFormatter;
    private readonly INamingConventionFormatter _propertyFormatter;
    private readonly INamingConventionFormatter _fieldFormatter;
    private readonly INamingConventionFormatter _enumItemFormatter;

    public ConfigurableCodeStyle(string namespaceFormatterName, string typeFormatterName, string methodFormatterName,
        string propertyFormatterName, string fieldFormatterName, string enumItemFormatterName)
    {
        _namespaceFormatter = CodeFormatManager.Ins.CreateFormatter(namespaceFormatterName);
        _typeFormatter = CodeFormatManager.Ins.CreateFormatter(typeFormatterName);
        _methodFormatter = CodeFormatManager.Ins.CreateFormatter(methodFormatterName);
        _propertyFormatter = CodeFormatManager.Ins.CreateFormatter(propertyFormatterName);
        _fieldFormatter = CodeFormatManager.Ins.CreateFormatter(fieldFormatterName);
        _enumItemFormatter = CodeFormatManager.Ins.CreateFormatter(enumItemFormatterName);
    }

    public override string FormatNamespace(string ns)
    {
        return _namespaceFormatter.FormatName(ns);
    }

    public override string FormatType(string typeName)
    {
        return _typeFormatter.FormatName(typeName);
    }

    public override string FormatMethod(string methodName)
    {
        return _methodFormatter.FormatName(methodName);
    }

    public override string FormatProperty(string propertyName)
    {
        return _propertyFormatter.FormatName(propertyName);
    }

    public override string FormatField(string fieldName)
    {
        return _fieldFormatter.FormatName(fieldName);
    }

    public override string FormatEnumItemName(string enumItemName)
    {
        return _enumItemFormatter.FormatName(enumItemName);
    }
}
