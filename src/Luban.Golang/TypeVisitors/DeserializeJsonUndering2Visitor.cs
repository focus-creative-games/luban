using Luban.Datas;
using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

class DeserializeJsonUndering2Visitor : ITypeFuncVisitor<string, string, string>
{
    public static DeserializeJsonUndering2Visitor Ins { get; } = new();

    public string Accept(TBool type, string varName, string bufName)
    {
        return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}.(bool); !_ok_ {{ err = errors.New(\"{varName} error\"); return }} }}";
    }

    private string DeserializeNumber(TType type, string varName, string bufName)
    {
        return $"{{ var _ok_ bool; var _x_ float64; if _x_, _ok_ = {bufName}.(float64); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; {varName} = {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}(_x_) }}";
    }

    public string Accept(TByte type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TShort type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TInt type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TLong type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TFloat type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TDouble type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TEnum type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }


    private string DeserializeString(TType type, string varName, string bufName)
    {
        return $"{{  if {varName}, _ok_ = {bufName}.(string); !_ok_ {{ err = errors.New(\"{varName} error\"); return }} }}";
    }

    public string Accept(TString type, string varName, string bufName)
    {
        return DeserializeString(type, varName, bufName);
    }

    public string Accept(TDateTime type, string varName, string bufName)
    {
        return DeserializeNumber(type, varName, bufName);
    }

    public string Accept(TBean type, string varName, string bufName)
    {
        return $"{{ var _ok_ bool; var _x_ map[string]interface{{}}; if _x_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; if {varName}, err = {($"New{GoCommonTemplateExtension.FullName(type.DefBean)}(_x_)")}; err != nil {{ return }} }}";
    }

    public string Accept(TArray type, string varName, string bufName)
    {
        throw new System.NotSupportedException();
    }

    public string Accept(TList type, string varName, string bufName)
    {
        throw new System.NotSupportedException();
    }

    public string Accept(TSet type, string varName, string bufName)
    {
        throw new System.NotSupportedException();
    }

    public string Accept(TMap type, string varName, string bufName)
    {
        throw new System.NotSupportedException();
    }
}
