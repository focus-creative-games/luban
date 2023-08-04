using System.Reflection;
using Luban.CodeFormat.CodeStyles;
using NLog;

namespace Luban.CodeFormat;

public class CodeFormatManager
{
    private static readonly ILogger s_logger = LogManager.GetCurrentClassLogger();
    
    public static CodeFormatManager Ins { get; } = new ();

    private readonly Dictionary<string, INamingConventionFormatter> _formatters = new();

    private readonly Dictionary<string, ICodeStyle> _codeStyles = new();

    public INamingConventionFormatter NoneFormatter { get; private set; }
    
    public INamingConventionFormatter CamelCaseFormatter { get; private set; }
    
    public INamingConventionFormatter PascalCaseFormatter { get; private set; }
    
    public INamingConventionFormatter SnakeCaseFormatter { get; private set; }
    
    public ICodeStyle NoneCodeStyle { get; private set; }
    
    public ICodeStyle CsharpDefaultCodeStyle { get; private set; }

    public ICodeStyle JavaDefaultCodeStyle { get; private set; }
    
    public ICodeStyle GoDefaultCodeStyle { get; private set; }
    
    public ICodeStyle LuaDefaultCodeStyle { get; private set; }
    
    public ICodeStyle TypescriptDefaultCodeStyle { get; private set; }
    
    public ICodeStyle CppDefaultCodeStyle { get; private set; }
    
    public ICodeStyle PythonDefaultCodeStyle { get; private set; }

    public void Init()
    {
        ScanRegisterFormatters(GetType().Assembly);
        NoneFormatter = GetFormatter("none");
        CamelCaseFormatter = GetFormatter("camel");
        PascalCaseFormatter = GetFormatter("pascal");
        SnakeCaseFormatter = GetFormatter("snake");
        
        ScanRegisterCodeStyle(GetType().Assembly);
        NoneCodeStyle = RegisterCodeStyle("none", "none", "none", "none", "none", "none", "none");
        CsharpDefaultCodeStyle = RegisterCodeStyle("csharp-default", "pascal", "pascal", "pascal", "pascal", "camel", "pascal");
        JavaDefaultCodeStyle = RegisterCodeStyle("java-default", "pascal", "pascal", "camel", "camel", "camel", "upper");
        GoDefaultCodeStyle = RegisterCodeStyle("go-default", "snake", "pascal", "camel", "camel", "pascal", "upper");
        LuaDefaultCodeStyle = RegisterCodeStyle("lua-default", "snake", "pascal", "camel", "camel", "snake", "upper");
        TypescriptDefaultCodeStyle = RegisterCodeStyle("typescript-default", "pascal", "pascal", "camel", "camel", "camel", "pascal");
        CppDefaultCodeStyle = RegisterCodeStyle("cpp-default", "snake", "pascal", "pascal", "pascal", "camel", "upper");
        PythonDefaultCodeStyle = RegisterCodeStyle("python-default", "snake", "pascal", "snake", "snake", "snake", "upper");
    }
    
    public void RegisterFormatter(string name, INamingConventionFormatter formatter)
    {
        if (!_formatters.TryAdd(name, formatter))
        {
            s_logger.Error("formatter:{} already exists", name);
        }
    }

    public INamingConventionFormatter GetFormatter(string formatterName)
    {
        return _formatters.TryGetValue(formatterName, out var formatter)
            ? formatter
            : throw new Exception($"formatter:{formatterName} not exists");
    }

    public void ScanRegisterFormatters(Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.GetCustomAttribute<NamingConventionAttribute>() is { } attr)
            {
                if (!typeof(INamingConventionFormatter).IsAssignableFrom(type))
                {
                    throw new Exception($"type:{type.FullName} not implement interface:{typeof(INamingConventionFormatter).FullName}");
                }
                var formatter = (INamingConventionFormatter)Activator.CreateInstance(type);
                RegisterFormatter(attr.Name, formatter);
            }
        }
    }
    
    public ICodeStyle GetCodeStyle(string codeStyleName)
    {
        return _codeStyles.TryGetValue(codeStyleName, out var codeStyle)
            ? codeStyle
            : throw new Exception($"code style:{codeStyleName} not exists");
    }

    public void RegisterCodeStyle(string name, ICodeStyle codeStyle)
    {
        if (!_codeStyles.TryAdd(name, codeStyle))
        {
            s_logger.Error("code style:{} exists", name);
        }
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