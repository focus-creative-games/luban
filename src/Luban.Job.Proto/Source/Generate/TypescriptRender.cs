using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Luban.Job.Proto.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Proto.Generate
{
    class TypescriptRender
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
            return RenderUtil.RenderTypescriptConstClass(c);
        }

        private string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
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


{{x.typescript_namespace_begin}}

export {{if x.is_abstract_type}} abstract {{end}} class {{name}} extends {{if parent_def_type}}{{x.parent}}{{else}}BeanBase{{end}} {
{{~if x.is_abstract_type~}}
    static serializeTo(_buf_ : Bright.Serialization.ByteBuf, _bean_ : {{name}}) {
        if (_bean_ == null) {
            _buf_.WriteInt(0)
            return
        }
        _buf_.WriteInt(_bean_.getTypeId())
        _bean_.serialize(_buf_)
    }

    static deserializeFrom(_buf_ : Bright.Serialization.ByteBuf) : {{name}} {
        let  _bean_ :{{name}}
        switch (_buf_.ReadInt()) {
            case 0 : return null
        {{~ for child in x.hierarchy_not_abstract_children~}}
            case {{child.id}}: _bean_ = new {{child.full_name}}(_buf_); break
        {{~end~}}
            default: throw new Error()
        }
        _bean_.deserialize(_buf_)
        return _bean_
    }
{{else}}
    static readonly ID = {{x.id}}
    getTypeId() { return {{name}}.ID }
{{~end~}}



    {{~ for field in fields ~}}
     {{field.ts_style_name}}{{if field.is_nullable}}?{{end}} : {{ts_define_type field.ctype}}
    {{~end~}}

    constructor() {
        super()
    {{~ for field in fields ~}}
        this.{{field.ts_style_name}} = {{ts_ctor_default_value field.ctype}}
    {{~end~}}
    }
    

    serialize(_buf_ : Bright.Serialization.ByteBuf) {
        {{~if parent_def_type~}}
        super.serialize(_buf_)
        {{~end~}}
        {{~ for field in fields ~}}
        {{ts_bin_serialize ('this.' + field.ts_style_name) '_buf_' field.ctype}}
        {{~end~}}
    }

    deserialize(_buf_ : Bright.Serialization.ByteBuf) {
        {{~if parent_def_type~}}
        super.deserialize(_buf_)
        {{~end~}}
        {{~ for field in fields ~}}
        {{ts_bin_deserialize ('this.' + field.ts_style_name) '_buf_' field.ctype}}
        {{~end~}}
    }

    toString(): string {
        return ""{{full_name}}{ ""
    {{~ for field in hierarchy_fields ~}}
            + ""{{field.ts_style_name}}:"" + this.{{field.ts_style_name}} + "",""
    {{~end~}}
        + ""}""
    }
}
{{x.typescript_namespace_end}}
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
{{x.typescript_namespace_begin}}

export class {{name}} extends Protocol {
    static readonly ID = {{x.id}}
    getTypeId() { return {{name}}.ID }


    {{~ for field in fields ~}}
     {{field.ts_style_name}}{{if field.is_nullable}}?{{end}} : {{ts_define_type field.ctype}}
    {{~end~}}

    constructor() {
        super()
    {{~ for field in fields ~}}
        this.{{field.ts_style_name}} = {{ts_ctor_default_value field.ctype}}
    {{~end~}}
    }

    serialize(_buf_ : Bright.Serialization.ByteBuf) {
        {{~ for field in fields ~}}
        {{ts_bin_serialize ('this.' + field.ts_style_name) '_buf_' field.ctype}}
        {{~end~}}
    }

    deserialize(_buf_ : Bright.Serialization.ByteBuf) {
        {{~ for field in fields ~}}
        {{ts_bin_deserialize ('this.' + field.ts_style_name) '_buf_' field.ctype}}
        {{~end~}}
    }

    toString(): string {
        return ""{{full_name}}{ ""
    {{~ for field in fields ~}}
            + ""{{field.ts_style_name}}:"" + this.{{field.ts_style_name}} + "",""
    {{~end~}}
        + ""}""
    }
}
{{x.typescript_namespace_end}}
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

// TODO {{full_name}}

");
            var result = template.RenderCode(r);

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStubs(string name, string module, List<DefTypeBase> protos, List<DefTypeBase> rpcs)
        {
            var template = t_stubRender ??= Template.Parse(@"

    type ProtocolFactory = () => Protocol

    export class {{name}} {
        static readonly Factories = new Map<number, ProtocolFactory>([

        {{~ for proto in protos ~}}
            [{{proto.full_name}}.ID, () => new {{proto.full_name}}()],
        {{~end~}}

        {{~ for rpc in rpcs ~}}
            // TODO RPC .. [{{rpc.full_name}}.ID] = () => new {{rpc.full_name}}(),
        {{~end~}}
        ])
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
