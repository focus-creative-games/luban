using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class JavaJsonDeserialize : ITypeFuncVisitor<string, string, string>
    {
        public static JavaJsonDeserialize Ins { get; } = new();

        public string Accept(TBool type, string json, string x)
        {
            return $"{x} = {json}.getAsBoolean();";
        }

        public string Accept(TByte type, string json, string x)
        {
            return $"{x} = {json}.getAsByte();";
        }

        public string Accept(TShort type, string json, string x)
        {
            return $"{x} = {json}.getAsShort();";
        }

        public string Accept(TFshort type, string json, string x)
        {
            return $"{x} = {json}.getAsShort();";
        }

        public string Accept(TInt type, string json, string x)
        {
            return $"{x} = {json}.getAsInt();";
        }

        public string Accept(TFint type, string json, string x)
        {
            return $"{x} = {json}.getAsInt();";
        }

        public string Accept(TLong type, string json, string x)
        {
            return $"{x} = {json}.getAsLong();";
        }

        public string Accept(TFlong type, string json, string x)
        {
            return $"{x} = {json}.getAsLong();";
        }

        public string Accept(TFloat type, string json, string x)
        {
            return $"{x} = {json}.getAsFloat();";
        }

        public string Accept(TDouble type, string json, string x)
        {
            return $"{x} = {json}.getAsDouble();";
        }

        public string Accept(TEnum type, string json, string x)
        {
            return $"{x} = {type.DefineEnum.FullNameWithTopModule}.valueOf({json}.getAsInt());";
        }

        public string Accept(TString type, string json, string x)
        {
            return $"{x} = {json}.getAsString();";
        }

        public string Accept(TBytes type, string json, string x)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x)
        {
            return $"{json}.getAsJsonObject().get(\"{DText.KEY_NAME}\").getAsString(); {x} = {json}.getAsJsonObject().get(\"{DText.TEXT_NAME}\").getAsString();";
        }

        public string Accept(TBean type, string json, string x)
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

        public string Accept(TArray type, string json, string x)
        {
            return $"{{ com.google.gson.JsonArray _json0_ = {json}.getAsJsonArray(); int _n = _json0_.size(); {x} = new {type.ElementType.Apply(JavaDefineTypeName.Ins)}[_n]; int _index=0; for(JsonElement __e : _json0_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}[_index++] = __v; }}   }}";
        }

        public string Accept(TList type, string json, string x)
        {
            return $"{{ com.google.gson.JsonArray _json0_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDefineTypeName.Ins)}(_json0_.size()); for(JsonElement __e : _json0_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.add(__v); }}   }}";
        }

        public string Accept(TSet type, string json, string x)
        {
            return $"{{ com.google.gson.JsonArray _json0_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDefineTypeName.Ins)}(_json0_.size()); for(JsonElement __e : _json0_) {{ {type.ElementType.Apply(JavaDefineTypeName.Ins)} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.add(__v); }}   }}";
        }

        public string Accept(TMap type, string json, string x)
        {
            return @$"{{ com.google.gson.JsonArray _json0_ = {json}.getAsJsonArray(); {x} = new {type.Apply(JavaDefineTypeName.Ins)}(_json0_.size()); for(JsonElement __e : _json0_) {{ {type.KeyType.Apply(JavaDefineTypeName.Ins)} __k;  {type.KeyType.Apply(this, "__e.getAsJsonArray().get(0)", "__k")} {type.ValueType.Apply(JavaDefineTypeName.Ins)} __v;  {type.ValueType.Apply(this, "__e.getAsJsonArray().get(1)", "__v")}  {x}.put(__k, __v); }}   }}";
        }

        public string Accept(TVector2 type, string json, string x)
        {
            return $"{{ com.google.gson.JsonObject _json0_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, "_json0_.get(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0_.get(\"y\")", "__y") } {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x)
        {
            return $"{{ com.google.gson.JsonObject _json0_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, "_json0_.get(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0_.get(\"y\")", "__y") } float __z; {TFloat.Ins.Apply(this, "_json0_.get(\"z\")", "__z") }  {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x)
        {
            return $"{{ com.google.gson.JsonObject _json0_ = {json}.getAsJsonObject(); float __x; {TFloat.Ins.Apply(this, "_json0_.get(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0_.get(\"y\")", "__y") } float __z; {TFloat.Ins.Apply(this, "_json0_.get(\"z\")", "__z") }  float __w; {TFloat.Ins.Apply(this, "_json0_.get(\"w\")", "__w") } {x} = new {type.Apply(JavaDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x)
        {
            return $"{x} = {json}.getAsInt();";
        }
    }
}
