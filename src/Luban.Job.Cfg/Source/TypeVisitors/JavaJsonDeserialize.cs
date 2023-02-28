using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class JavaJsonDeserialize : ITypeFuncVisitor<string, string, int, string>
    {
        public static JavaJsonDeserialize Ins { get; } = new();

        public string Accept(TBool type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsBoolean();";
        }

        public string Accept(TByte type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsByte();";
        }

        public string Accept(TShort type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsShort();";
        }

        public string Accept(TFshort type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsShort();";
        }

        public string Accept(TInt type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsInt();";
        }

        public string Accept(TFint type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsInt();";
        }

        public string Accept(TLong type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsLong();";
        }

        public string Accept(TFlong type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsLong();";
        }

        public string Accept(TFloat type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsFloat();";
        }

        public string Accept(TDouble type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsDouble();";
        }

        public string Accept(TEnum type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsInt();";
        }

        public string Accept(TString type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsString();";
        }

        public string Accept(TBytes type, string json, string x, int depth)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x, int depth)
        {
            return $"{json}.getAsJsonObject().get(\"{DText.KEY_NAME}\").getAsString(); {x} = {json}.getAsJsonObject().get(\"{DText.TEXT_NAME}\").getAsString();";
        }

        public string Accept(TBean type, string json, string x, int depth)
        {
            if (type.IsDynamic)
            {
                return $"{x} = {type.Bean.FullNameWithTopModule}.deserialize{type.Bean.Name}({json}.getAsJsonObject());";
            }
            else
            {
                return $"{x} = new {type.Bean.FullNameWithTopModule}({json}.getAsJsonObject());";
            }
        }

        public string Accept(TArray type, string json, string x, int depth)
        {
            string __n = $"__n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __index = $"__index{depth}";
            string typeStr = $"{type.ElementType.Apply(JavaDefineTypeName.Ins)}[{__n}]";
            if (type.Dimension > 1)
            {
                if (type.FinalElementType == null)
                {
                    throw new System.Exception("多维数组没有元素类型");
                }
                typeStr = $"{type.FinalElementType.Apply(JavaDefineTypeName.Ins)}[{__n}]";
                for (int i = 0; i < type.Dimension - 1; i++)
                {
                    typeStr += "[]";
                }
            }
            return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); int {__n} = _json{depth}_.size(); {x} = new {typeStr}; int {__index}=0; for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
        }

        public string Accept(TList type, string json, string x, int depth)
        {
            string __e = $"_e{depth}";
            string __v = $"_v{depth}";
            return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {type.Apply(JavaDefineTypeName.Ins)} _tmp_{x} = new java.util.ArrayList(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  _tmp_{x}.add({__v}); }} {x} = Collections.unmodifiableList(_tmp_{x});  }}";
        }

        public string Accept(TSet type, string json, string x, int depth)
        {
            string __e = $"_e{depth}";
            string __v = $"_v{depth}";
            return $"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {type.Apply(JavaDefineTypeName.Ins)} _tmp_{x} = new java.util.HashSet(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  _tmp_{x}.add({__v}); }} {x} = Collections.unmodifiableSet(_tmp_{x});  }}";
        }

        public string Accept(TMap type, string json, string x, int depth)
        {
            string __e = $"_e{depth}";
            string __k = $"_k{depth}";
            string __v = $"_v{depth}";
            return @$"{{ com.google.gson.JsonArray _json{depth}_ = {json}.getAsJsonArray(); {type.Apply(JavaDefineTypeName.Ins)} _tmp_{x} = new java.util.HashMap(_json{depth}_.size()); for(JsonElement {__e} : _json{depth}_) {{ {type.KeyType.Apply(JavaDefineTypeName.Ins)} {__k};  {type.KeyType.Apply(this, $"{__e}.getAsJsonArray().get(0)", __k, depth + 1)} {type.ValueType.Apply(JavaDefineTypeName.Ins)} {__v};  {type.ValueType.Apply(this, $"{__e}.getAsJsonArray().get(1)", __v, depth + 1)}  _tmp_{x}.put({__k}, {__v}); }} {x} = Collections.unmodifiableMap(_tmp_{x});  }}";
        }

        public string Accept(TVector2 type, string json, string x, int depth)
        {
            return $"{{ com.google.gson.JsonObject _json{depth}_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"x\")", "__x", depth)} float __y; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"y\")", "__y", depth)} {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x, int depth)
        {
            return $"{{ com.google.gson.JsonObject _json{depth}_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"x\")", "__x", depth)} float __y; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"y\")", "__y", depth)} float __z; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"z\")", "__z", depth)}  {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x, int depth)
        {
            return $"{{ com.google.gson.JsonObject _json{depth}_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"x\")", "__x", depth)} float __y; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"y\")", "__y", depth)} float __z; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"z\")", "__z", depth)}  float __w; {TFloat.Ins.Apply(this, $"_json{depth}_.get(\"w\")", "__w", depth)} {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x, int depth)
        {
            return $"{x} = {json}.getAsLong();";
        }
    }
}
