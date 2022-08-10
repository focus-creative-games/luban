using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Luban.Job.Common.Utils;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsUnityJsonDeserialize : ITypeFuncVisitor<string, string, int, string>
    {
        public static CsUnityJsonDeserialize Ins { get; } = new();

        public string Accept(TBool type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsBoolean) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TByte type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TShort type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFshort type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TInt type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFint type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TLong type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFlong type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TFloat type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TDouble type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TEnum type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = ({type.CsUnderingDefineType()}){json}.AsInt; }}";
        }

        public string Accept(TString type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsString) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }

        public string Accept(TBytes type, string json, string x, int depth)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x, int depth)
        {
            return $"{{ if(!{json}[\"{DText.KEY_NAME}\"].IsString) {{ throw new SerializationException(); }}  {x}{TText.L10N_FIELD_SUFFIX} = {json}[\"{DText.KEY_NAME}\"]; if(!{json}[\"{DText.TEXT_NAME}\"].IsString) {{ throw new SerializationException(); }}  {x} = {json}[\"{DText.TEXT_NAME}\"]; }}";
        }

        public string Accept(TBean type, string json, string x, int depth)
        {
            string src = $"{type.Bean.FullName}.Deserialize{type.Bean.Name}({json})";
            return $"{{ if(!{json}.IsObject) {{ throw new SerializationException(); }}  {x} = {ExternalTypeUtil.CsCloneToExternal(type.Bean.FullName, src)};  }}";
        }

        public string Accept(TArray type, string json, string x, int depth)
        {
            string _n = $"_n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            string __index = $"__index{depth}";
            string tempJsonName = __json;
            string typeStr = $"{type.ElementType.Apply(CsDefineTypeName.Ins)}[{_n}]";
            if (type.Dimension > 1)
            {
                if (type.FinalElementType == null)
                {
                    throw new System.Exception("多维数组没有元素类型");
                }
                typeStr = $"{type.FinalElementType.Apply(CsUnderingDefineTypeName.Ins)}[{_n}]";
                for (int i = 0; i < type.Dimension - 1; i++)
                {
                    typeStr += "[]";
                }
            }
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} int {_n} = {tempJsonName}.Count; {x} = new {typeStr}; int {__index}=0; foreach(JSONNode {__e} in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
        }

        public string Accept(TList type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            string tempJsonName = __json;
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}({tempJsonName}.Count); foreach(JSONNode {__e} in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  {x}.Add({__v}); }}   }}";
        }

        public string Accept(TSet type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            string tempJsonName = __json;
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}(/*{tempJsonName}.Count*/); foreach(JSONNode {__e} in {tempJsonName}.Children) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, __e, __v, depth + 1)}  {x}.Add({__v}); }}   }}";
        }

        public string Accept(TMap type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __k = $"_k{depth}";
            string __v = $"_v{depth}";
            string __json = $"__json{depth}";
            string tempJsonName = __json;
            return @$"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsArray) {{ throw new SerializationException(); }} {x} = new {type.CsUnderingDefineType()}({tempJsonName}.Count); foreach(JSONNode {__e} in {tempJsonName}.Children) {{ {type.KeyType.CsUnderingDefineType()} {__k};  {type.KeyType.Apply(this, $"{__e}[0]", __k, depth + 1)} {type.ValueType.CsUnderingDefineType()} { __v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)}  {x}.Add({__k}, { __v}); }}   }}";
        }

        public string Accept(TVector2 type, string json, string x, int depth)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }}  float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x", depth) } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y", depth) } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x, int depth)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }} float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x", depth) } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y", depth) } float __z; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"z\"]", "__z", depth) }  {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x, int depth)
        {
            string tempJsonName = $"_json2";
            return $"{{ var {tempJsonName} = {json}; if(!{tempJsonName}.IsObject) {{ throw new SerializationException(); }} float __x; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"x\"]", "__x", depth) } float __y; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"y\"]", "__y", depth) } float __z; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"z\"]", "__z", depth) }  float __w; {TFloat.Ins.Apply(this, $"{tempJsonName}[\"w\"]", "__w", depth) } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x, int depth)
        {
            return $"{{ if(!{json}.IsNumber) {{ throw new SerializationException(); }}  {x} = {json}; }}";
        }
    }
}
