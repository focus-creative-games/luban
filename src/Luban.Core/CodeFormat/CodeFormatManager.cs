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

using System.Reflection;
using Luban.CodeFormat.CodeStyles;
using Luban.CustomBehaviour;
using NLog;

namespace Luban.CodeFormat;

public class CodeFormatManager
{
    private static readonly ILogger s_logger = LogManager.GetCurrentClassLogger();

    public static CodeFormatManager Ins { get; } = new();


    public ICodeStyle NoneCodeStyle { get; private set; }

    public ICodeStyle CsharpDefaultCodeStyle { get; private set; }

    public ICodeStyle JavaDefaultCodeStyle { get; private set; }

    public ICodeStyle GoDefaultCodeStyle { get; private set; }

    public ICodeStyle LuaDefaultCodeStyle { get; private set; }

    public ICodeStyle TypescriptDefaultCodeStyle { get; private set; }

    public ICodeStyle CppDefaultCodeStyle { get; private set; }

    public ICodeStyle PythonDefaultCodeStyle { get; private set; }

    public ICodeStyle DartDefaultCodeStyle { get; private set; }

    public void Init()
    {
    }

    public void PostInit()
    {
        NoneCodeStyle = RegisterCodeStyle("none", "none", "none", "none", "none", "none", "none");
        CsharpDefaultCodeStyle = RegisterCodeStyle("csharp-default", "pascal", "pascal", "pascal", "pascal", "camel", "none");
        JavaDefaultCodeStyle = RegisterCodeStyle("java-default", "pascal", "pascal", "camel", "camel", "camel", "none");
        GoDefaultCodeStyle = RegisterCodeStyle("go-default", "snake", "pascal", "camel", "camel", "pascal", "none");
        LuaDefaultCodeStyle = RegisterCodeStyle("lua-default", "snake", "pascal", "camel", "camel", "snake", "none");
        TypescriptDefaultCodeStyle = RegisterCodeStyle("typescript-default", "pascal", "pascal", "camel", "camel", "camel", "none");
        CppDefaultCodeStyle = RegisterCodeStyle("cpp-default", "snake", "pascal", "pascal", "pascal", "camel", "none");
        PythonDefaultCodeStyle = RegisterCodeStyle("python-default", "snake", "pascal", "snake", "snake", "snake", "none");
        DartDefaultCodeStyle = RegisterCodeStyle("dart-default", "pascal", "pascal", "camel", "camel", "camel", "none");
    }

    public INamingConventionFormatter CreateFormatter(string formatterName)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<INamingConventionFormatter, NamingConventionAttribute>(formatterName);
    }

    public ICodeStyle GetCodeStyle(string codeStyleName)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<ICodeStyle, CodeStyleAttribute>(codeStyleName);
    }

    public void RegisterCodeStyle(string name, ICodeStyle codeStyle)
    {
        CustomBehaviourManager.Ins.RegisterBehaviour(typeof(CodeStyleAttribute), name, 0, () => codeStyle);
    }

    public ICodeStyle RegisterCodeStyle(string name, string namespaceNamingConvention, string typeNamingConvention,
        string methodNamingConvention, string propertyNamingConvention, string fieldNamingConvention, string enumNamingConvention)
    {
        var codeStyle = new ConfigurableCodeStyle(namespaceNamingConvention, typeNamingConvention,
            methodNamingConvention, propertyNamingConvention, fieldNamingConvention, enumNamingConvention);
        RegisterCodeStyle(name, codeStyle);
        return codeStyle;
    }

    public void ScanRegisterCodeStyle(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<CodeStyleAttribute>() is { } attr)
            {
                if (!typeof(ICodeStyle).IsAssignableFrom(type))
                {
                    throw new Exception($"type:{type.FullName} not implement interface:{typeof(ICodeStyle).FullName}");
                }
                var codeStyle = (ICodeStyle)Activator.CreateInstance(type);
                RegisterCodeStyle(attr.Name, codeStyle);
            }
        }
    }
}
