using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class JavaCodeBinRender : JavaCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;

        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
package {{x.namespace_with_top_module}};

import bright.serialization.*;

{{
    name = x.name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

/**
 * {{x.comment}}
 */
public {{x.java_class_modifier}} class {{name}} extends {{if parent_def_type}} {{x.parent_def_type.full_name_with_top_module}} {{else}} bright.serialization.AbstractBean {{end}}
{
    public {{name}}(ByteBuf _buf)
    { 
        {{~if parent_def_type~}}
        super(_buf);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{java_deserialize '_buf' field.java_style_name field.ctype}}
        {{~if field.index_field~}}
        for({{java_box_define_type field.ctype.element_type}} _v : {{field.java_style_name}})
        {
            {{field.java_style_name}}_Index.put(_v.{{field.index_field.java_style_name}}, _v); 
        }
        {{~end~}}
        {{~end~}}
    }

    public {{name}}({{- for field in hierarchy_export_fields }}{{java_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}})
    {
        {{~if parent_def_type~}}
        super({{ for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}}, {{end}}{{end}});
        {{~end~}}
        {{~ for field in export_fields ~}}
        this.{{field.java_style_name}} = {{field.name}};
        {{~if field.index_field~}}
        for({{java_box_define_type field.ctype.element_type}} _v : {{field.java_style_name}})
        {
            {{field.java_style_name}}_Index.put(_v.{{field.index_field.java_style_name}}, _v); 
        }
        {{~end~}}
        {{~end~}}
    }

    public static {{name}} deserialize{{name}}(ByteBuf _buf)
    {
    {{~if x.is_abstract_type~}}
        switch (_buf.readInt())
        {
        {{~for child in x.hierarchy_not_abstract_children~}}
            case {{child.full_name_with_top_module}}.ID: return new {{child.full_name_with_top_module}}(_buf);
        {{~end~}}
            default: throw new SerializationException();
        }
    {{~else~}}
        return new {{name}}(_buf);
    {{~end~}}
    }

    {{~ for field in export_fields ~}}
    /**
     * {{field.comment}}
     */
    public final {{java_define_type field.ctype}} {{field.java_style_name}};
    {{~if field.index_field~}} 
    public final java.util.HashMap<{{java_box_define_type field.index_field.ctype}}, {{java_box_define_type field.ctype.element_type}}> {{field.java_style_name}}_Index = new java.util.HashMap<>();
    {{~end~}}
    {{~if field.gen_ref~}}
    public {{field.java_ref_validator_define}}
    {{~end~}}
    {{~end~}}

{{~if !x.is_abstract_type~}}
    public static final int ID = {{x.id}};

    @Override
    public int getTypeId() { return ID; }
{{~end~}}

    @Override
    public void serialize(ByteBuf os)
    {
        throw new UnsupportedOperationException();
    }

    @Override
    public void deserialize(ByteBuf os)
    {
        throw new UnsupportedOperationException();
    }

    public void resolve(java.util.HashMap<String, Object> _tables)
    {
        {{~if parent_def_type~}}
        super.resolve(_tables);
        {{~end~}}
        {{~ for field in export_fields ~}}
        {{~if field.gen_ref~}}
            {{java_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
            {{java_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }

    @Override
    public String toString()
    {
        return ""{{full_name}}{ ""
    {{~for field in hierarchy_export_fields ~}}
        + ""{{field.java_style_name}}:"" + {{java_to_string field.java_style_name field.ctype}} + "",""
    {{~end~}}
        + ""}"";
    }
}

");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
package {{x.namespace_with_top_module}};

import bright.serialization.*;

{{~
    name = x.name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
~}}

/**
 * {{x.comment}}
 */
public final class {{name}}
{
    {{~if x.is_map_table ~}}
    private final java.util.HashMap<{{java_box_define_type key_type}}, {{java_box_define_type value_type}}> _dataMap;
    private final java.util.ArrayList<{{java_box_define_type value_type}}> _dataList;
    
    public {{name}}(ByteBuf _buf)
    {
        _dataMap = new java.util.HashMap<{{java_box_define_type key_type}}, {{java_box_define_type value_type}}>();
        _dataList = new java.util.ArrayList<{{java_box_define_type value_type}}>();
        
        for(int n = _buf.readSize() ; n > 0 ; --n)
        {
            {{java_box_define_type value_type}} _v;
            {{java_deserialize '_buf' '_v' value_type}}
            _dataList.add(_v);
            _dataMap.put(_v.{{x.index_field.java_style_name}}, _v);
        }
    }

    public java.util.HashMap<{{java_box_define_type key_type}}, {{java_box_define_type value_type}}> getDataMap() { return _dataMap; }
    public java.util.ArrayList<{{java_box_define_type value_type}}> getDataList() { return _dataList; }

{{~if value_type.is_dynamic~}}
    @SuppressWarnings(""unchecked"")
    public <T extends {{java_box_define_type value_type}}> T getAs({{java_define_type key_type}} key) { return (T)_dataMap.get(key); }
{{~end~}}
    public {{java_box_define_type value_type}} get({{java_define_type key_type}} key) { return _dataMap.get(key); }

    public void resolve(java.util.HashMap<String, Object> _tables)
    {
        for({{java_box_define_type value_type}} v : _dataList)
        {
            v.resolve(_tables);
        }
    }

    {{~else~}}
    private final {{java_define_type value_type}} _data;

    public final {{java_define_type value_type}} data() { return _data; }

    public {{name}}(ByteBuf _buf)
    {
        int n = _buf.readSize();
        if (n != 1) throw new SerializationException(""table mode=one, but size != 1"");
        {{java_deserialize '_buf' '_data' value_type}}
    }


    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
    /**
     * {{field.comment}}
     */
     public {{java_define_type field.ctype}} {{field.java_getter_name}}() { return _data.{{field.java_style_name}}; }
    {{~end~}}

    public void resolve(java.util.HashMap<String, Object> _tables)
    {
        _data.resolve(_tables);
    }

    {{~end~}}
}
");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(@"
package {{package}};

import bright.serialization.*;

public final class {{name}}
{
    public  static interface  IByteBufLoader {
        ByteBuf load(String file) throws java.io.IOException;
    }

    {{~for table in tables ~}}
    /**
     * {{table.comment}}
     */
    public final {{table.full_name_with_top_module}} {{table.name}};
    {{~end~}}

    public {{name}}(IByteBufLoader loader) throws java.io.IOException {
        var tables = new java.util.HashMap<String, Object>();
        {{~for table in tables ~}}
        {{table.name}} = new {{table.full_name_with_top_module}}(loader.load(""{{table.output_data_file}}"")); 
        tables.put(""{{table.full_name}}"", {{table.name}});
        {{~end~}}

        {{~ for table in tables ~}}
        {{table.name}}.resolve(tables); 
        {{~end~}}
    }
}

");
            var result = template.Render(new
            {
                Name = name,
                Package = module,
                Tables = tables,
            });

            return result;
        }
    }
}
