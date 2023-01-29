using Luban.Job.Cfg.Datas;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
using Luban.Job.Common.Utils;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class CsJsonDeserialize : ITypeFuncVisitor<string, string, int, string>
    {
        public static CsJsonDeserialize Ins { get; } = new();
        public string Accept(TBool type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetBoolean();";
        }

        public string Accept(TByte type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetByte();";
        }

        public string Accept(TShort type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt16();";
        }

        public string Accept(TFshort type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt16();";
        }

        public string Accept(TInt type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt32();";
        }

        public string Accept(TFint type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt32();";
        }

        public string Accept(TLong type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt64();";
        }

        public string Accept(TFlong type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt64();";
        }

        public string Accept(TFloat type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetSingle();";
        }

        public string Accept(TDouble type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetDouble();";
        }

        public string Accept(TEnum type, string json, string x, int depth)
        {
            return $"{x} = ({type.CsUnderingDefineType()}){json}.GetInt32();";
        }

        public string Accept(TString type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetString();";
        }

        public string Accept(TBytes type, string json, string x, int depth)
        {
            throw new NotSupportedException();
        }

        public string Accept(TText type, string json, string x, int depth)
        {
            return $"{x}{TText.L10N_FIELD_SUFFIX} = {json}.GetProperty(\"{DText.KEY_NAME}\").GetString();{x} = {json}.GetProperty(\"{DText.TEXT_NAME}\").GetString();";
        }

        public string Accept(TBean type, string json, string x, int depth)
        {
            string src = $"{type.Bean.FullName}.Deserialize{type.Bean.Name}({json})";
            return $"{x} = {ExternalTypeUtil.CsCloneToExternal(type.Bean.FullName, src)};";
        }

        public string Accept(TArray type, string json, string x, int depth)
        {
            string _n = $"_n{depth}";
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            string __index = $"__index{depth}";
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
            return $"{{ var {__json} = {json}; int {_n} = {__json}.GetArrayLength(); {x} = new {typeStr}; int {__index}=0; foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}[{__index}++] = {__v}; }}   }}";
        }

        public string Accept(TList type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            return $"{{ var {__json} = {json}; {x} = new {type.CsUnderingDefineType()}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
        }

        public string Accept(TSet type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __v = $"__v{depth}";
            string __json = $"__json{depth}";
            return $"{{ var {__json} = {json}; {x} = new {type.CsUnderingDefineType()}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.ElementType.CsUnderingDefineType()} {__v};  {type.ElementType.Apply(this, $"{__e}", $"{__v}", depth + 1)}  {x}.Add({__v}); }}   }}";
        }

        public string Accept(TMap type, string json, string x, int depth)
        {
            string __e = $"__e{depth}";
            string __k = $"_k{depth}";
            string __v = $"_v{depth}";
            string __json = $"__json{depth}";
            return @$"{{ var {__json} = {json}; {x} = new {type.CsUnderingDefineType()}({__json}.GetArrayLength()); foreach(JsonElement {__e} in {__json}.EnumerateArray()) {{ {type.KeyType.CsUnderingDefineType()} {__k};  {type.KeyType.Apply(this, $"{__e}[0]", __k,depth + 1)} {type.ValueType.CsUnderingDefineType()} {__v};  {type.ValueType.Apply(this, $"{__e}[1]", __v, depth + 1)}  {x}.Add({__k}, {__v}); }}   }}";
        }

        public string Accept(TVector2 type, string json, string x, int depth)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x", depth) } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y", depth) } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y); }}";
        }

        public string Accept(TVector3 type, string json, string x, int depth)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x", depth) } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y", depth) } float __z; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"z\")", "__z", depth) }  {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y,__z); }}";
        }

        public string Accept(TVector4 type, string json, string x, int depth)
        {
            return $"{{ var _json0 = {json}; float __x; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"x\")", "__x", depth) } float __y; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"y\")", "__y", depth) } float __z; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"z\")", "__z", depth) }  float __w; {TFloat.Ins.Apply(this, "_json0.GetProperty(\"w\")", "__w", depth) } {x} = new {type.Apply(CsDefineTypeName.Ins)}(__x, __y, __z, __w); }}";
        }

        public string Accept(TDateTime type, string json, string x, int depth)
        {
            return $"{x} = {json}.GetInt64();";
        }
    }
}
