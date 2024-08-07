using Luban.DataLoader;
using Luban.Datas;
using Luban.Golang.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Golang.TypeVisitors;

class DeserializeJsonUnderingVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static DeserializeJsonUnderingVisitor Ins { get; } = new();

    public string Accept(TBool type, string varName, string bufName, int depth)
    {
        return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}.(bool); !_ok_ {{ err = errors.New(\"{varName} error\"); return }} }}";
    }

    private string DeserializeNumber(TType type, string varName, string bufName, int depth)
    {
        return $"{{ var _ok_ bool; var _x_ float64; if _x_, _ok_ = {bufName}.(float64); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; {varName} = {type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}(_x_) }}";
    }

    public string Accept(TByte type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TShort type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TInt type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TLong type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TFloat type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TDouble type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TEnum type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }


    private string DeserializeString(TType type, string varName, string bufName, int depth)
    {
        return $"{{  if {varName}, _ok_ = {bufName}.(string); !_ok_ {{ err = errors.New(\"{varName} error\"); return }} }}";
    }

    public string Accept(TString type, string varName, string bufName, int depth)
    {
        return DeserializeString(type, varName, bufName, depth);
    }

    public string Accept(TDateTime type, string varName, string bufName, int depth)
    {
        return DeserializeNumber(type, varName, bufName, depth);
    }

    public string Accept(TBean type, string varName, string bufName, int depth)
    {
        return $"{{ var _ok_ bool; var _x_ map[string]interface{{}}; if _x_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; if {varName}, err = {($"New{GoCommonTemplateExtension.FullName(type.DefBean)}(_x_)")}; err != nil {{ return }} }}";
    }

    private string GenList(TType elementType, string varName, string bufName, int depth)
    {
        return $@"{{
                var _arr{depth}_ []interface{{}}
                var _ok{depth}_ bool
                if _arr{depth}_, _ok{depth}_ = ({bufName}).([]interface{{}}); !_ok{depth}_ {{ err = errors.New(""{varName} error""); return }}

                {varName} = make([]{elementType.Apply(DeclaringTypeNameVisitor.Ins)}, 0, len(_arr{depth}_))
                
                for _, _e{depth}_ := range _arr{depth}_ {{
                    var _list_v{depth}_ {elementType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {elementType.Apply(DeserializeJsonVisitor.Ins, $"_list_v{depth}_", $"_e{depth}_", depth + 1)}
                    {varName} = append({varName}, _list_v{depth}_)
                }}
            }}
";
    }

    public string Accept(TArray type, string varName, string bufName, int depth)
    {
        return GenList(type.ElementType, varName, bufName, depth);
    }

    public string Accept(TList type, string varName, string bufName, int depth)
    {
        return GenList(type.ElementType, varName, bufName, depth);
    }

    public string Accept(TSet type, string varName, string bufName, int depth)
    {
        return GenList(type.ElementType, varName, bufName, depth);
    }

    public string Accept(TMap type, string varName, string bufName, int depth)
    {
        return $@"{{
                var _arr{depth}_ []interface{{}}
                var _ok{depth}_ bool
                if _arr{depth}_, _ok_ = ({bufName}).([]interface{{}}); !_ok_ {{ err = errors.New(""{varName} error""); return }}

                {varName} = make({type.Apply(DeclaringTypeNameVisitor.Ins)})
                
                for _, _e{depth}_ := range _arr{depth}_ {{
                    var _kv{depth}_ []interface{{}}
                    if _kv{depth}_, _ok{depth}_ = _e{depth}_.([]interface{{}}); !_ok{depth}_ || len(_kv{depth}_) != 2 {{ err = errors.New(""{varName} error""); return }}
                    var _key{depth}_ {type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {type.KeyType.Apply(DeserializeJsonVisitor.Ins, $"_key{depth}_", $"_kv{depth}_[0]", depth + 1)}
                    var _value{depth}_ {type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}
                    {type.ValueType.Apply(DeserializeJsonVisitor.Ins, $"_value{depth}_", $"_kv{depth}_[1]", depth + 1)}
                    {varName}[_key{depth}_] = _value{depth}_
                }}
                }}";
    }
}
