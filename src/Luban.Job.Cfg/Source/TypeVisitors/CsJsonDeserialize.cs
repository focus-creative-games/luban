using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsJsonDeserialize : ITypeFuncVisitor<string, string, string>
    {
        public static CsJsonDeserialize Ins { get; } = new();

        public string Accept(TBool type, string json, string x)
        {
            return $"{x} = {json}.GetBoolean();";
        }

        public string Accept(TByte type, string json, string x)
        {
            return $"{x} = {json}.GetByte();";
        }

        public string Accept(TShort type, string json, string x)
        {
            return $"{x} = {json}.GetInt16();";
        }

        public string Accept(TFshort type, string json, string x)
        {
            return $"{x} = {json}.GetInt16();";
        }

        public string Accept(TInt type, string json, string x)
        {
            return $"{x} = {json}.GetInt32();";
        }

        public string Accept(TFint type, string json, string x)
        {
            return $"{x} = {json}.GetInt32();";
        }

        public string Accept(TLong type, string json, string x)
        {
            return $"{x} = {json}.GetInt64();";
        }

        public string Accept(TFlong type, string json, string x)
        {
            return $"{x} = {json}.GetInt64();";
        }

        public string Accept(TFloat type, string json, string x)
        {
            return $"{x} = {json}.GetSingle();";
        }

        public string Accept(TDouble type, string json, string x)
        {
            return $"{x} = {json}.GetDouble();";
        }

        public string Accept(TEnum type, string json, string x)
        {
            return $"{x} = ({type.CsUnderingDefineType()}){json}.GetInt32();";
        }

        public string Accept(TString type, string json, string x)
        {
            return $"{x} = {json}.GetString();";
        }

        public string Accept(TBytes type, string json, string x)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x)
        {
            return $"{x}{TText.L10N_FIELD_SUFFIX} = {json}.GetProperty(\"{DText.KEY_NAME}\").GetString();{x} = {json}.GetProperty(\"{DText.TEXT_NAME}\").GetString();";
        }

        public string Accept(TBean type, string json, string x)
        {
            return $"{x} =  {type.CsUnderingDefineType()}.Deserialize{type.Bean.Name}({json});";
        }

        public string Accept(TArray type, string json, string x)
        {
            return $"{{ var _json0 = {json}; int _n = _json0.GetArrayLength(); {x} = new {type.ElementType.CsUnderingDefineType()}[_n]; int _index=0; foreach(JsonElement __e in _json0.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}[_index++] = __v; }}   }}";
        }

        public string Accept(TList type, string json, string x)
        {
            return $"{{ var _json0 = {json}; {x} = new {type.CsUnderingDefineType()}(_json0.GetArrayLength()); foreach(JsonElement __e in _json0.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}   }}";
        }

        public string Accept(TSet type, string json, string x)
        {
            return $"{{ var _json0 = {json}; {x} = new {type.CsUnderingDefineType()}(_json0.GetArrayLength()); foreach(JsonElement __e in _json0.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} __v;  {type.ElementType.Apply(this, "__e", "__v")}  {x}.Add(__v); }}   }}";
        }

        public string Accept(TMap type, string json, string x)
        {
            return @$"{{ var _json0 = {json}; {x} = new {type.CsUnderingDefineType()}(_json0.GetArrayLength()); foreach(JsonElement __e in _json0.EnumerateArray()) {{ {type.KeyType.CsUnderingDefineType()} __k;  {type.KeyType.Apply(this, "__e[0]", "__k")} {type.ValueType.CsUnderingDefineType()} __v;  {type.ValueType.Apply(this, "__e[1]", "__v")}  {x}.Add(__k, __v); }}   }}";
        }

        public string Accept(TVector2 type, string json, string x)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y") } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y") } float __z; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"z\")", "__z") }  {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x") } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y") } float __z; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"z\")", "__z") }  float __w; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"w\")", "__w") } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x)
        {
            return $"{x} = {json}.GetInt32();";
        }
    }
}
