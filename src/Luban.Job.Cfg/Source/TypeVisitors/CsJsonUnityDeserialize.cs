using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsUnityJsonDeserialize : ITypeFuncVisitor<string, string, string>
    {
        public static CsUnityJsonDeserialize Ins { get; } = new();

        public string Accept(TBool type, string json, string x)
        {
            return $"{{ if(!{json}.IsBoolean) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TByte type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TShort type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFshort type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TInt type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFint type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TLong type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFlong type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFloat type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TDouble type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TEnum type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = ({type.CsUnderingDefineType()}){json}.AsInt; }}";
        }

        public string Accept(TString type, string json, string x)
        {
            return $"{{ if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TBytes type, string json, string x)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x)
        {
            return $"{{ if(!{json}[\"{DText.KEY_NAME}\"].IsString) {{ throw new SerializationException(); }}  {x}{TText.L10N_FIELD_SUFFIX} = {json}[\"{DText.KEY_NAME}\"]; if(!{json}[\"{DText.TEXT_NAME}\"].IsString) {{ throw new SerializationException(); }}  {x} = {json}[\"{DText.TEXT_NAME}\"]; }}";
        }

        public string Accept(TBean type, string json, string x)
        {
            return $"{{ if(!{json}.IsObject) {{ throw new SerializationException(); }}  {x} = {type.CsUnderingDefineType()}.Deserialize{type.Bean.Name}({json}); }}";
        }

        public string Accept(TArray type, string json, string x)
        {
            string tempJsonName = $"_json1";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} int _n = {tempJsonName}.Count; {x} = new {type.ElementType.CsUnderingDefineType()}[_n]; int _index=0; foreach(JSONNode __e in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}[_index++] = __v; }}   }}";
        }

        public string Accept(TList type, string json, string x)
        {
            string tempJsonName = $"_json1";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}({tempJsonName}.Count); foreach(JSONNode __e in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}   }}";
        }

        public string Accept(TSet type, string json, string x)
        {
            string tempJsonName = $"_json1";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}(/*{tempJsonName}.Count*/); foreach(JSONNode __e in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}   }}";
        }

        public string Accept(TMap type, string json, string x)
        {
            string tempJsonName = $"_json1";
            return @$"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}({tempJsonName}.Count); foreach(JSONNode __e in {tempJsonName}.Children) {{ {type.KeyType.CsUnderingDefineType()} __k;  {type.KeyType.Apply(this, "__e[0]", "__k")} {type.ValueType.CsUnderingDefineType()} __v;  {type.ValueType.Apply(this, "__e[1]", "__v")}  {x}.Add(__k, __v); }}   }}";
        }

        public string Accept(TVector2 type, string json, string x)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }}  float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x") } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y") } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }} float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x") } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y") } float __z; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"z\"]", "__z") }  {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }} float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x") } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y") } float __z; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"z\"]", "__z") }  float __w; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"w\"]", "__w") } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }
    }
}
