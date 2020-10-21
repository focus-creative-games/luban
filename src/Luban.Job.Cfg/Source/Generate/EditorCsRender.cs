using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Generate
{
    class EditorCsRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                // case DefBean b: return Render(b); editor 不需要生成 table 的定义
                // case CTable r: return Render(r);
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

namespace {{namespace}}
{
   
public {{cs_class_modifier}} class {{name}} : {{if parent_def_type}} {{parent}} {{else}} ISerializable {{if is_abstract_type}}, ITypeId {{end}} {{end}}
{
    public {{name}}() {{if parent_def_type}} : base() {{end}}
    {

    }

    public {{name}}(Bright.Common.NotNullInitialization _) {{if parent_def_type}} : base(_) {{end}}
    {
        {{- for field in fields }}
        {{if field.ctype.need_init}}{{field.proto_cs_init_field}} {{end}}
        {{-end}}
    }

    {{if is_abstract_type}}
    public static void Serialize{{name}}(ByteBuf _buf, {{name}} x)
    {
        if (x == null) { _buf.WriteInt(0); return; }
        _buf.WriteInt(x.GetTypeId());
        x.Serialize(_buf);
    }

    public static {{name}} Deserialize{{name}}(ByteBuf _buf)
    {
        {{name}} x;
        switch (_buf.ReadInt())
        {
            case 0 : return null;
        {{- for child in hierarchy_not_abstract_children}}
            case {{child.full_name}}.ID: x = new {{child.full_name}}(false); break;
        {{-end}}
            default: throw new SerializationException();
        }
        x.Deserialize(_buf);
        return x;
    }
    {{end}}
    {{- for field in fields }}
     public {{field.ctype.cs_define_type}} {{field.cs_style_name}};
    {{-end}}

    {{if !parent_def_type && is_abstract_type}}
        public abstract int GetTypeId();
    {{end}}
    {{if parent_def_type && !is_abstract_type}}
    public const int ID = {{id}};
    public override int GetTypeId()
    {
        return ID;
    }
    {{end}}

    public {{cs_method_modifer}} void Serialize(ByteBuf _buf)
    {
        {{if parent_def_type}} base.Serialize(_buf); {{end}}
        {{- for field in fields }}
        {{field.cs_serialize}}
        {{-end}}
    }

    public {{cs_method_modifer}} void Deserialize(ByteBuf _buf)
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
            + ""{{field.cs_style_name}}:"" + {{field.proto_cs_to_string}} + "",""
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
        private static Template t_tableRender;
        public string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
public sealed class {{name}} : Bright.Net.Protocol
{
    {{- for field in fields }}
     public {{field.ctype.cs_define_type}} {{field.cs_style_name}};
    {{-end}}
    public {{name}}()
    {
    }
    public {{name}}(Bright.Common.NotNullInitialization _)
    {
        {{- for field in fields }}
        {{if field.ctype.need_init}}{{field.proto_cs_init_field}} {{end}}
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
        + ""{{field.cs_style_name}}:"" + {{field.proto_cs_to_string}} + "",""
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
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<CfgDefTypeBase> protos)
        {
            var template = t_stubRender ??= Template.Parse(@"
using Bright.Serialization;

namespace {{namespace}}
{
   
public static class {{name}}
{
    public static System.Collections.Generic.Dictionary<int, Bright.Net.IProtocolFactory> Factories { get; } = new System.Collections.Generic.Dictionary<int, Bright.Net.IProtocolFactory>
    {
    {{- for proto in protos }}
        [{{proto.full_name}}.ID] = () => new {{proto.full_name}}(false),
    {{-end}}
    };
}

}

");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = protos.Where(p => p is DefTable).ToList(),
            });

            return result;
        }
    }
}
