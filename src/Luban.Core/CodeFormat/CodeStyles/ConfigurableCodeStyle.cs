// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
