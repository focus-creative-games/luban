using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Proto.Generate
{
    class CsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefProto p: return Render(p);
                case DefRpc r: return Render(r);

                default: throw new Exception($"unknown render type:{o}");
            }
        }

        public string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        public string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace_with_top_module}}
{
   
    public  {{if is_value_type}}struct{{else}}{{cs_class_modifier}} class{{end}} {{name}} : {{if parent_def_type}} {{parent}} {{else}} BeanBase {{end}}
    {
        {{if !is_value_type}}
        public {{name}}()
        {
        }
        {{end}}

        public {{name}}(Bright.Common.NotNullInitialization _) {{if parent_def_type}} : base(_) {{end}}
        {
            {{if is_value_type}}
            {{- for field in fields }}
            {{if field.ctype.need_init}}{{field.proto_cs_init_field}}{{else}}{{field.proto_cs_init_field_default_value}} {{end}}
            {{-end}}
            {{else}}
            {{- for field in fields }}
            {{if field.ctype.need_init}}{{field.proto_cs_init_field}}{{end}}
            {{-end}}
            {{end}}
        }

        public static void Serialize{{name}}(ByteBuf _buf, {{name}} x)
        {
    {{if is_abstract_type}}
            if (x != null)
            {
                _buf.WriteInt(x.GetTypeId());
                x.Serialize(_buf);
            }
            else
            {
                _buf.WriteInt(0);
            }
    {{else}}
            x.Serialize(_buf);
    {{end}}
        }

        public static {{name}} Deserialize{{name}}(ByteBuf _buf)
        {
        {{if is_abstract_type}}
           {{full_name}} x;
            switch (_buf.ReadInt())
            {
                case 0 : return null;
            {{- for child in hierarchy_not_abstract_children}}
                case {{child.full_name}}.ID: x = new {{child.full_name}}(); break;
            {{-end}}
                default: throw new SerializationException();
            }
            x.Deserialize(_buf);
        {{else}}
            var x = new {{full_name}}();
            x.Deserialize(_buf);
        {{end}}
            return x;
        }
        {{- for field in fields }}
         public {{field.ctype.cs_define_type}} {{field.lan_style_name}};
        {{-end}}

        {{if !is_abstract_type}}
            public const int ID = {{id}};
            public override int GetTypeId() => ID;
        {{end}}

        public override void Serialize(ByteBuf _buf)
        {
            {{if parent_def_type}} base.Serialize(_buf); {{end}}
            {{- for field in fields }}
            {{field.cs_serialize}}
            {{-end}}
        }

        public override void Deserialize(ByteBuf _buf)
        {
            {{if parent_def_type}} base.Deserialize(_buf); {{end}}
            {{- for field in fields }}
            {{field.cs_deserialize}}
            {{-end}}
        }

        public override string ToString()
        {
            return ""{{full_name}}{ ""
        {{- for field in hierarchy_fields }}
            + ""{{field.lan_style_name}}:"" + {{field.proto_cs_to_string}} + "",""
        {{-end}}
            + ""}"";
        }
    }

}

");
            var result = template.Render(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_protoRender;
        public string Render(DefProto p)
        {
            var template = t_protoRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace_with_top_module}}
{
   
    public sealed class {{name}} : Bright.Net.Codecs.Protocol
    {
        {{- for field in fields }}
         public {{field.ctype.cs_define_type}} {{field.lan_style_name}};
        {{-end}}
        public {{name}}()
        {
        }

        public {{name}}(Bright.Common.NotNullInitialization _)
        {
            {{- for field in fields }}
            {{if field.ctype.need_init}}{{field.proto_cs_init_field}}{{end}}
            {{-end}}
        }
        public const int ID = {{id}};

        public override int GetTypeId()
        {
            return ID;
        }

        public override void Serialize(ByteBuf _buf)
        {
            {{- for field in fields }}
            {{field.cs_serialize}}
            {{-end}}
        }

        public override void Deserialize(ByteBuf _buf)
        {
            {{- for field in fields }}
            {{field.cs_deserialize}}
            {{-end}}
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return ""{{full_name}}{ ""
        {{- for field in fields }}
            + ""{{field.lan_style_name}}:"" + {{field.proto_cs_to_string}} + "",""
        {{-end}}
            + ""}"";
        }
    }

}

");
            var result = template.Render(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_rpcRender;
        public string Render(DefRpc r)
        {
            var template = t_rpcRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace_with_top_module}}
{
   
    public sealed class {{name}} : Bright.Net.Codecs.Rpc<{{targ_type.cs_define_type}}, {{tres_type.cs_define_type}}>
    {
        public {{name}}()
        {
        }
        
        public const int ID = {{id}};

        public override int GetTypeId()
        {
            return ID;
        }

        public override void SerializeArg(ByteBuf buf, {{targ_type.cs_define_type}} arg)
        {
            {{targ_type.cs_define_type}}.Serialize{{targ_type.bean.name}}(buf, arg);
        }

        public override {{targ_type.cs_define_type}} DeserializeArg(ByteBuf buf)
        {
            return {{targ_type.cs_define_type}}.Deserialize{{targ_type.bean.name}}(buf);
        }

        public override void SerializeRes(ByteBuf buf, {{tres_type.cs_define_type}} res)
        {
            {{tres_type.cs_define_type}}.Serialize{{tres_type.bean.name}}(buf, res);
        }

        public override {{tres_type.cs_define_type}} DeserializeRes(ByteBuf buf)
        {
            return {{tres_type.cs_define_type}}.Deserialize{{tres_type.bean.name}}(buf);
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            return $""{{full_name}}{%{ {{arg:{Arg},res:{Res} }} }%}"";
        }
    }
}

");
            var result = template.Render(r);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<ProtoDefTypeBase> protos, List<ProtoDefTypeBase> rpcs)
        {
            var template = t_stubRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
    public static class {{name}}
    {
        public static System.Collections.Generic.Dictionary<int, Bright.Net.Codecs.ProtocolCreator> Factories { get; } = new System.Collections.Generic.Dictionary<int, Bright.Net.Codecs.ProtocolCreator>
        {
        {{- for proto in protos }}
            [{{proto.full_name}}.ID] = () => new {{proto.full_name}}(),
        {{-end}}

        {{- for rpc in rpcs }}
            [{{rpc.full_name}}.ID] = () => new {{rpc.full_name}}(),
        {{-end}}
        };
    }

}

");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Protos = protos,
                Rpcs = rpcs,
            });

            return result;
        }
    }
}
