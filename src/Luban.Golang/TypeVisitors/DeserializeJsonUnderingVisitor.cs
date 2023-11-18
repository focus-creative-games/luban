using Luban.Datas;
using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

class DeserializeJsonUnderingVisitor : ITypeFuncVisitor<string, string, string, string>
{
    public static DeserializeJsonUnderingVisitor Ins { get; } = new();

    public string Accept(TBool type, string varName, string fieldName, string bufName)
    {
        return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}[\"{fieldName}\"].(bool); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }} }}";
    }

    private string DeserializeNumber(TType type, string varName, string fieldName, string bufName)
    {
        return $"{{ var _ok_ bool; var _tempNum_ float64; if _tempNum_, _ok_ = {bufName}[\"{fieldName}\"].(float64); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }}; {varName} = {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}(_tempNum_) }}";
    }

    public string Accept(TByte type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TShort type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TInt type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TLong type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TFloat type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TDouble type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TEnum type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }


    private string DeserializeString(TType type, string varName, string fieldName, string bufName)
    {
        return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}[\"{fieldName}\"].(string); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }} }}";
    }

    public string Accept(TString type, string varName, string fieldName, string bufName)
    {
        return DeserializeString(type, varName, fieldName, bufName);
    }

    public string Accept(TDateTime type, string varName, string fieldName, string bufName)
    {
        return DeserializeNumber(type, varName, fieldName, bufName);
    }

    public string Accept(TBean type, string varName, string fieldName, string bufName)
    {
        return $"{{ var _ok_ bool; var _x_ map[string]interface{{}}; if _x_, _ok_ = {bufName}[\"{fieldName}\"].(map[string]interface{{}}); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }}; if {varName}, err = {($"New{GoCommonTemplateExtension.FullName(type.DefBean)}(_x_)")}; err != nil {{ return }} }}";
    }


    private string GenList(TType elementType, string varName, string fieldName, string bufName)
    {
        return $@" {{
                var _arr_ []interface{{}}
                var _ok_ bool
                if _arr_, _ok_ = {bufName}[""{fieldName}""].([]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}

                {varName} = make([]{elementType.Apply(DeclaringTypeNameVisitor.Ins)}, 0, len(_arr_))
                
                for _, _e_ := range _arr_ {{
                    var _list_v_ {elementType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {elementType.Apply(DeserializeJson2Visitor.Ins, "_list_v_", "_e_")}
                    {varName} = append({varName}, _list_v_)
                }}
            }}
";
    }

    public string Accept(TArray type, string varName, string fieldName, string bufName)
    {
        return GenList(type.ElementType, varName, fieldName, bufName);
    }

    public string Accept(TList type, string varName, string fieldName, string bufName)
    {
        return GenList(type.ElementType, varName, fieldName, bufName);
    }

    public string Accept(TSet type, string varName, string fieldName, string bufName)
    {
        return GenList(type.ElementType, varName, fieldName, bufName);
    }

    public string Accept(TMap type, string varName, string fieldName, string bufName)
    {
        return $@"{{
                var _arr_ []interface{{}}
                var _ok_ bool
                if _arr_, _ok_ = {bufName}[""{fieldName}""].([]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}

                {varName} = make({type.Apply(DeclaringTypeNameVisitor.Ins)})
                
                for _, _e_ := range _arr_ {{
                    var _kv_ []interface{{}}
                    if _kv_, _ok_ = _e_.([]interface{{}}); !_ok_ || len(_kv_) != 2 {{ err = errors.New(""{fieldName} error""); return }}
                    var _key_ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {type.KeyType.Apply(DeserializeJson2Visitor.Ins, "_key_", "_kv_[0]")}
                    var _value_ {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {type.ValueType.Apply(DeserializeJson2Visitor.Ins, "_value_", "_kv_[1]")}
                    {varName}[_key_] = _value_
                }}
                }}";
    }
}
