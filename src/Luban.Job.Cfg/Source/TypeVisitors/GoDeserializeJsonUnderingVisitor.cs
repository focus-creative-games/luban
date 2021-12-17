using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeJsonUnderingVisitor : ITypeFuncVisitor<string, string, string, string>
    {
        public static GoDeserializeJsonUnderingVisitor Ins { get; } = new();

        public string Accept(TBool type, string varName, string fieldName, string bufName)
        {
            return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}[\"{fieldName}\"].(bool); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }} }}";
        }

        private string DeserializeNumber(TType type, string varName, string fieldName, string bufName)
        {
            return $"{{ var _ok_ bool; var _tempNum_ float64; if _tempNum_, _ok_ = {bufName}[\"{fieldName}\"].(float64); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }}; {varName} = {type.Apply(GoTypeUnderingNameVisitor.Ins)}(_tempNum_) }}";
        }

        public string Accept(TByte type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TShort type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TFshort type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TInt type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TFint type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TLong type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TFlong type, string varName, string fieldName, string bufName)
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

        public string Accept(TText type, string varName, string fieldName, string bufName)
        {
            return $"{{var _ok_ bool; var __json_text__ map[string]interface{{}}; if __json_text__, _ok_ = {bufName}[\"{fieldName}\"].(map[string]interface{{}}) ; !_ok_ {{ err = errors.New(\"{varName} error\"); return }};  {DeserializeString(type, "_", DText.KEY_NAME, "__json_text__")}; {DeserializeString(type, varName, DText.TEXT_NAME, "__json_text__")} }}";
        }

        public string Accept(TBytes type, string varName, string fieldName, string bufName)
        {
            //return $"{{ if {varName}, err = {bufName}.ReadBytes(); err != nil {{ return }} }}";
            throw new System.NotSupportedException();
        }

        public string Accept(TVector2 type, string varName, string fieldName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}[""{fieldName}""].(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}
            var _x_, _y_ float32;
            {TFloat.Ins.Apply(this, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(this, "_y_", "y", "_v_")}
            {varName} = serialization.NewVector2(_x_, _y_)
            }}
";
        }

        public string Accept(TVector3 type, string varName, string fieldName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}[""{fieldName}""].(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}
            var _x_, _y_, _z_ float32;
            {TFloat.Ins.Apply(this, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(this, "_y_", "y", "_v_")}
            {TFloat.Ins.Apply(this, "_z_", "z", "_v_")}
            {varName} = serialization.NewVector3(_x_, _y_, _z_)
            }}
";
        }

        public string Accept(TVector4 type, string varName, string fieldName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}[""{fieldName}""].(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}
            var _x_, _y_, _z_, _w_ float32;
            {TFloat.Ins.Apply(this, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(this, "_y_", "y", "_v_")}
            {TFloat.Ins.Apply(this, "_z_", "z", "_v_")}
            {TFloat.Ins.Apply(this, "_w_", "w", "_v_")}
            {varName} = serialization.NewVector4(_x_, _y_, _z_, _w_)
            }}
";
        }

        public string Accept(TDateTime type, string varName, string fieldName, string bufName)
        {
            return DeserializeNumber(type, varName, fieldName, bufName);
        }

        public string Accept(TBean type, string varName, string fieldName, string bufName)
        {
            return $"{{ var _ok_ bool; var _x_ map[string]interface{{}}; if _x_, _ok_ = {bufName}[\"{fieldName}\"].(map[string]interface{{}}); !_ok_ {{ err = errors.New(\"{fieldName} error\"); return }}; if {varName}, err = {($"Deserialize{ type.Bean.GoFullName}(_x_)")}; err != nil {{ return }} }}";
        }


        private string GenList(TType elementType, string varName, string fieldName, string bufName)
        {
            return $@" {{
                var _arr_ []interface{{}}
                var _ok_ bool
                if _arr_, _ok_ = {bufName}[""{ fieldName}""].([]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}

                { varName} = make([]{elementType.Apply(GoTypeNameVisitor.Ins)}, 0, len(_arr_))
                
                for _, _e_ := range _arr_ {{
                    var _list_v_ {elementType.Apply(GoTypeNameVisitor.Ins)}
                    {elementType.Apply(GoDeserializeJson2Visitor.Ins, "_list_v_", "_e_")}
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
                if _arr_, _ok_ = {bufName}[""{ fieldName}""].([]interface{{}}); !_ok_ {{ err = errors.New(""{fieldName} error""); return }}

                {varName} = make({type.Apply(GoTypeNameVisitor.Ins)})
                
                for _, _e_ := range _arr_ {{
                    var _kv_ []interface{{}}
                    if _kv_, _ok_ = _e_.([]interface{{}}); !_ok_ || len(_kv_) != 2 {{ err = errors.New(""{fieldName} error""); return }}
                    var _key_ {type.KeyType.Apply(GoTypeNameVisitor.Ins)}
                    {type.KeyType.Apply(GoDeserializeJson2Visitor.Ins, "_key_", "_kv_[0]")}
                    var _value_ {type.ValueType.Apply(GoTypeNameVisitor.Ins)}
                    {type.ValueType.Apply(GoDeserializeJson2Visitor.Ins, "_value_", "_kv_[1]")}
                    {varName}[_key_] = _value_
                }}
                }}";
        }
    }
}
