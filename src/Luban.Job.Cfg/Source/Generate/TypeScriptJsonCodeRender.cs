using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class TypeScriptJsonCodeRender : CodeRenderBase
    {
        public override string Render(DefConst c)
        {
            return RenderUtil.RenderTypescriptConstClass(c);
        }

        public override string Render(DefEnum e)
        {
            return RenderUtil.RenderTypescriptEnumClass(e);
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"

{{
    name = x.name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

{{x.typescript_namespace_begin}}

export {{if x.is_abstract_type}} abstract {{end}} class {{name}} {{if parent_def_type}} extends {{x.parent}}{{end}} {
{{~if x.is_abstract_type~}}
    static constructorFrom(_json_: any): {{name}} {
        switch (_json_.__type__) {
        {{~ for child in x.hierarchy_not_abstract_children~}}
            case '{{child.name}}': return new {{child.full_name}}(_json_)
        {{~end~}}
            default: throw new Error()
        }
    }
{{~end~}}

    constructor(_json_: any) {
        {{~if parent_def_type~}}
        super(_json_)
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if !field.ctype.is_nullable~}}
        if (_json_.{{field.name}} == null) { throw new Error() }
        {{~end~}}
        {{ts_json_constructor ('this.' + field.ts_style_name) ( '_json_.' + field.name) field.ctype}}
        {{~end~}}
    }

    {{~ for field in export_fields ~}}
    readonly {{field.ts_style_name}}{{if field.is_nullable}}?{{end}}: {{ts_define_type field.ctype}}
    {{~if field.gen_ref~}}
    {{field.ts_ref_validator_define}}
    {{~end~}}
    {{~end~}}

    resolve(_tables: Map<string, any>) {
        {{~if parent_def_type~}}
        super.resolve(_tables)
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
        {{ts_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
        {{ts_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }
}

{{x.typescript_namespace_end}}
");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
   {{ 
        name = x.name
        key_type = x.key_ttype
        key_type1 =  x.key_ttype1
        key_type2 =  x.key_ttype2
        value_type =  x.value_ttype
    }}
{{x.typescript_namespace_begin}}
export class {{name}}{
    {{~if x.is_map_table ~}}
    private _dataMap: Map<{{ts_define_type key_type}}, {{ts_define_type value_type}}>
    private _dataList: {{ts_define_type value_type}}[]
    constructor(_json_: any) {
        this._dataMap = new Map<{{ts_define_type key_type}}, {{ts_define_type value_type}}>()
        this._dataList = []
        for(var _json2_ of _json_) {
            let _v: {{ts_define_type value_type}}
            {{ts_json_constructor '_v' '_json2_' value_type}}
            this._dataList.push(_v)
            this._dataMap.set(_v.{{x.index_field.ts_style_name}}, _v)
        }
    }

    getDataMap(): Map<{{ts_define_type key_type}}, {{ts_define_type value_type}}> { return this._dataMap; }
    getDataList(): {{ts_define_type value_type}}[] { return this._dataList; }

    get(key: {{ts_define_type key_type}}): {{ts_define_type value_type}}  { return this._dataMap.get(key); }

    resolve(_tables: Map<string, any>) {
        for(var v of this._dataList) {
            v.resolve(_tables)
        }
    }

    {{~else~}}

     private _data: {{ts_define_type value_type}}
    constructor(_json_: any) {
        if (_json_.length != 1) throw new Error('table mode=one, but size != 1')
        {{ts_json_constructor 'this._data' '_json_[0]' value_type}}
    }

    getData(): {{ts_define_type value_type}} { return this._data; }

    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
     get  {{field.ts_style_name}}(): {{ts_define_type field.ctype}} { return this._data.{{field.ts_style_name}}; }
    {{~end~}}

    resolve(_tables: Map<string, any>) {
        this._data.resolve(_tables)
    }

    {{end}}
}
{{x.typescript_namespace_end}}
");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(@"
{{
    name = x.name
    namespace = x.namespace
    tables = x.tables

}}

type JsonLoader = (file: string) => any

export class {{name}} {
    {{~ for table in tables ~}}
    private _{{table.name}}: {{table.full_name}}
    get {{table.name}}(): {{table.full_name}}  { return this._{{table.name}};}
    {{~end~}}

    constructor(loader: JsonLoader) {
        let tables = new Map<string, any>()
        {{~for table in tables ~}}
        this._{{table.name}} = new {{table.full_name}}(loader('{{table.json_output_data_file}}'))
        tables.set('{{table.full_name}}', this._{{table.name}})
        {{~end~}}

        {{~ for table in tables ~}}
        this._{{table.name}}.resolve(tables)
        {{~end~}}
    }
}

");
            var result = template.RenderCode(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
            });

            return result;
        }
    }
}
