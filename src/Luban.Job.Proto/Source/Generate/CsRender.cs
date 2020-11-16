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

        private string Render(DefConst c)
        {
            return RenderUtil.RenderCsConstClass(c);
        }

        private string Render(DefEnum e)
        {
            return RenderUtil.RenderCsEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        private string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
{{
    is_value_type = x.is_value_type
    is_abstract_type = x.is_abstract_type
    name = x.name
    full_name = x.full_name
    parent_def_type = x.parent_def_type
    parent = x.parent
    fields = x.fields
    hierarchy_fields = x.hierarchy_fields
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{
   
    public  {{if is_value_type}}struct{{else}}{{x.cs_class_modifier}} class{{end}} {{name}} : {{if parent_def_type}} {{parent}} {{else}} Bright.Serialization.BeanBase {{end}}
    {
        {{~if !is_value_type~}}
        public {{name}}()
        {
        }
        {{~end~}}

        public {{name}}(Bright.Common.NotNullInitialization _) {{if parent_def_type}} : base(_) {{end}}
        {
            {{~ for field in fields ~}}
                {{~if cs_need_init field.ctype~}}
            {{cs_init_field_ctor_value field}}
                {{~else if is_value_type~}}
            {field.cs_style_name} = default;
                {{~end~}}
            {{~end~}}
        }

        public static void Serialize{{name}}(ByteBuf _buf, {{name}} x)
        {
    {{~if is_abstract_type~}}
            if (x != null)
            {
                _buf.WriteInt(x.GetTypeId());
                x.Serialize(_buf);
            }
            else
            {
                _buf.WriteInt(0);
            }
    {{~else~}}
            x.Serialize(_buf);
    {{~end~}}
        }

        public static {{name}} Deserialize{{name}}(ByteBuf _buf)
        {
        {{~if is_abstract_type~}}
           {{full_name}} x;
            switch (_buf.ReadInt())
            {
                case 0 : return null;
            {{- for child in x.hierarchy_not_abstract_children}}
                case {{child.full_name}}.ID: x = new {{child.full_name}}(); break;
            {{-end}}
                default: throw new SerializationException();
            }
            x.Deserialize(_buf);
        {{~else~}}
            var x = new {{full_name}}();
            x.Deserialize(_buf);
        {{~end~}}
            return x;
        }
        {{~ for field in fields ~}}
         public {{cs_define_type field.ctype}} {{field.cs_style_name}};
        {{~end~}}

        {{~if !is_abstract_type~}}
        public const int ID = {{x.id}};
        public override int GetTypeId() => ID;

        public override void Serialize(ByteBuf _buf)
        {
            {{~ for field in hierarchy_fields ~}}
            {{cs_serialize '_buf' field.cs_style_name field.ctype}}
            {{~end~}}
        }

        public override void Deserialize(ByteBuf _buf)
        {
            {{~ for field in hierarchy_fields ~}}
            {{cs_deserialize '_buf' field.cs_style_name field.ctype}}
            {{~end~}}
        }

        public override string ToString()
        {
            return ""{{full_name}}{ ""
        {{~ for field in hierarchy_fields ~}}
            + ""{{field.cs_style_name}}:"" + {{cs_to_string field.cs_style_name field.ctype}} + "",""
        {{~end~}}
            + ""}"";
        }
        {{~end~}}
    }

}

");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_protoRender;
        private string Render(DefProto p)
        {
            var template = t_protoRender ??= Template.Parse(@"
{{
    name = x.name
    full_name = x.full_name
    parent = x.parent
    fields = x.fields
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{
   
    public sealed class {{name}} : Bright.Net.Codecs.Protocol
    {
        {{~ for field in fields ~}}
         public {{cs_define_type field.ctype}} {{field.cs_style_name}};
        {{~end~}}

        public {{name}}()
        {
        }

        public {{name}}(Bright.Common.NotNullInitialization _)
        {
            {{~ for field in fields ~}}
                {{~if field.ctype.need_init~}}
            {{cs_init_field_ctor_value field}}
                {{~end~}}
            {{~end~}}
        }
        public const int ID = {{x.id}};

        public override int GetTypeId()
        {
            return ID;
        }

        public override void Serialize(ByteBuf _buf)
        {
            {{~ for field in fields ~}}
            {{field.cs_serialize}}
            {{~end~}}
        }

        public override void Deserialize(ByteBuf _buf)
        {
            {{~ for field in fields ~}}
            {{field.cs_deserialize}}
            {{~end~}}
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
        {{~ for field in fields ~}}
            + ""{{field.cs_style_name}}:"" + {{cs_to_string field.cs_style_name field.ctype}} + "",""
        {{~end~}}
            + ""}"";
        }
    }

}

");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_rpcRender;
        private string Render(DefRpc r)
        {
            var template = t_rpcRender ??= Template.Parse(@"
{{
    name = x.name
    full_name = x.full_name
    parent = x.parent
    fields = x.fields
    targ_type = x.targ_type
    tres_type = x.tres_type
}}
using Bright.Serialization;

namespace {{x.namespace_with_top_module}}
{
   
    public sealed class {{name}} : Bright.Net.Codecs.Rpc<{{cs_define_type targ_type}}, {{cs_define_type tres_type}}>
    {
        public {{name}}()
        {
        }
        
        public const int ID = {{x.id}};

        public override int GetTypeId()
        {
            return ID;
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
            var result = template.RenderCode(r);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<DefTypeBase> protos, List<DefTypeBase> rpcs)
        {
            var template = t_stubRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
    public static class {{name}}
    {
        public static System.Collections.Generic.Dictionary<int, Bright.Net.Codecs.ProtocolCreator> Factories { get; } = new System.Collections.Generic.Dictionary<int, Bright.Net.Codecs.ProtocolCreator>
        {
        {{~ for proto in protos ~}}
            [{{proto.full_name}}.ID] = () => new {{proto.full_name}}(),
        {{~end~}}

        {{~ for rpc in rpcs ~}}
            [{{rpc.full_name}}.ID] = () => new {{rpc.full_name}}(),
        {{~end~}}
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
