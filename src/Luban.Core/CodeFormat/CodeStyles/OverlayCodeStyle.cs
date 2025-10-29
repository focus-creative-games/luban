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

﻿using System;
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
