using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;

namespace Luban.Job.Cfg.TypeVisitors
{
    class GoDeserializeJsonUndering2Visitor : ITypeFuncVisitor<string, string, string>
    {
        public static GoDeserializeJsonUndering2Visitor Ins { get; } = new();

        public string Accept(TBool type, string varName, string bufName)
        {
            return $"{{ var _ok_ bool; if {varName}, _ok_ = {bufName}.(bool); !_ok_ {{ err = errors.New(\"{varName} error\"); return }} }}";
        }

        private string DeserializeNumber(TType type, string varName, string bufName)
        {
            return $"{{ var _ok_ bool; var _x_ float64; if _x_, _ok_ = {bufName}.(float64); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; {varName} = {type.Apply(GoTypeUnderingNameVisitor.Ins)}(_x_) }}";
        }

        public string Accept(TByte type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TShort type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TFshort type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TInt type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TFint type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TLong type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TFlong type, string varName, string bufName)
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

        public string Accept(TText type, string varName, string bufName)
        {
            return $"{{var _ok_ bool; var __json_text__ map[string]interface{{}}; if __json_text__, _ok_ = {bufName}.(map[string]interface{{}}) ; !_ok_ {{ err = errors.New(\"{varName} error\"); return }};   {DeserializeString(type, "_", $"__json_text__[\"{DText.KEY_NAME}\"]")}; {DeserializeString(type, varName, $"__json_text__[\"{DText.TEXT_NAME}\"]")} }}";
        }

        public string Accept(TBytes type, string varName, string bufName)
        {
            //return $"{{ if {varName}, err = {bufName}.ReadBytes(); err != nil {{ return }} }}";
            throw new System.NotSupportedException();
        }

        public string Accept(TVector2 type, string varName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{varName} error""); return }}
            var _x_, _y_ float32;
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_y_", "y", "_v_")}
            {varName} = serialization.NewVector2(_x_, _y_)
            }}
";
        }

        public string Accept(TVector3 type, string varName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{varName} error""); return }}
            var _x_, _y_, _z_ float32;
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_y_", "y", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_z_", "z", "_v_")}
            {varName} = serialization.NewVector3(_x_, _y_, _z_)
            }}
";
        }

        public string Accept(TVector4 type, string varName, string bufName)
        {
            return $@"{{ var _ok_ bool; var _v_ map[string]interface{{}}; if _v_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(""{varName} error""); return }}
            var _x_, _y_, _z_, _w_ float32;
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_x_", "x", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_y_", "y", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_z_", "z", "_v_")}
            {TFloat.Ins.Apply(GoDeserializeJsonUnderingVisitor.Ins, "_w_", "w", "_v_")}
            {varName} = serialization.NewVector4(_x_, _y_, _z_, _w_)
            }}
";
        }

        public string Accept(TDateTime type, string varName, string bufName)
        {
            return DeserializeNumber(type, varName, bufName);
        }

        public string Accept(TBean type, string varName, string bufName)
        {
            return $"{{ var _ok_ bool; var _x_ map[string]interface{{}}; if _x_, _ok_ = {bufName}.(map[string]interface{{}}); !_ok_ {{ err = errors.New(\"{varName} error\"); return }}; if {varName}, err = {($"Deserialize{ type.Bean.GoFullName}(_x_)")}; err != nil {{ return }} }}";
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
}
