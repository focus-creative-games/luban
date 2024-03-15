using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.CodeFormat.CodeStyles;

public class OverlayCodeStyle : CodeStyleBase
{
    private ICodeStyle _defaultStyle;
    private readonly INamingConventionFormatter _namespaceFormatter;
    private readonly INamingConventionFormatter _typeFormatter;
    private readonly INamingConventionFormatter _methodFormatter;
    private readonly INamingConventionFormatter _propertyFormatter;
    private readonly INamingConventionFormatter _fieldFormatter;
    private readonly INamingConventionFormatter _enumItemFormatter;

    public OverlayCodeStyle(ICodeStyle defaultStyle, string namespaceFormatterName, string typeFormatterName, string methodFormatterName,
        string propertyFormatterName, string fieldFormatterName, string enumItemFormatterName)
    {
        _defaultStyle = defaultStyle;
        _namespaceFormatter = CreateFormatter(namespaceFormatterName);
        _typeFormatter = CreateFormatter(typeFormatterName);
        _methodFormatter = CreateFormatter(methodFormatterName);
        _propertyFormatter = CreateFormatter(propertyFormatterName);
        _fieldFormatter = CreateFormatter(fieldFormatterName);
        _enumItemFormatter = CreateFormatter(enumItemFormatterName);
    }

    private static INamingConventionFormatter CreateFormatter(string formatterName)
    {
        return string.IsNullOrEmpty(formatterName) ? null : CodeFormatManager.Ins.CreateFormatter(formatterName);
    }

    public override string FormatNamespace(string ns)
    {
        return _namespaceFormatter?.FormatName(ns) ?? _defaultStyle.FormatNamespace(ns);
    }

    public override string FormatType(string typeName)
    {
        return _typeFormatter?.FormatName(typeName) ?? _defaultStyle.FormatType(typeName);
    }

    public override string FormatMethod(string methodName)
    {
        return _methodFormatter?.FormatName(methodName) ?? _defaultStyle.FormatMethod(methodName);
    }

    public override string FormatProperty(string propertyName)
    {
        return _propertyFormatter?.FormatName(propertyName) ?? _defaultStyle.FormatProperty(propertyName);
    }

    public override string FormatField(string fieldName)
    {
        return _fieldFormatter?.FormatName(fieldName) ?? _defaultStyle.FormatField(fieldName);
    }

    public override string FormatEnumItemName(string enumItemName)
    {
        return _enumItemFormatter?.FormatName(enumItemName) ?? _defaultStyle.FormatEnumItemName(enumItemName);
    }
}
