using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class CppCodeBinRender : CodeRenderBase
    {
        public override string Render(DefConst c)
        {
            return RenderUtil.RenderCppConstClass(c);
        }

        public override string Render(DefEnum c)
        {
            return RenderUtil.RenderCppEnumClass(c);
        }

        public string RenderForwardDefine(DefBean b)
        {
            return $"{b.CppNamespaceBegin} class {b.Name}; {b.CppNamespaceEnd} ";
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
{{x.cpp_namespace_begin}}



{{
    name = x.name
    parent_def_type = x.parent_def_type
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

/**
{{x.comment}}
 */
class {{name}} : public {{if parent_def_type}} {{parent_def_type.cpp_full_name}} {{else}} bright::CfgBean {{end}}
{
    public:

    static bool deserialize{{name}}(ByteBuf& _buf, {{name}}*& _out);

    {{name}}()
    { 

    }

{{~if !hierarchy_export_fields.empty?~}}
    {{name}}({{- for field in hierarchy_export_fields }}{{cpp_define_type field.ctype}} {{field.name}}{{if !for.last}},{{end}} {{end}}) 
    {{~if parent_def_type~}}
            : {{parent_def_type.cpp_full_name}}({{ for field in parent_def_type.hierarchy_export_fields }}{{field.name}}{{if !for.last}}, {{end}}{{end}})
    {{~end~}}
    {

        {{~ for field in export_fields ~}}
        this->{{field.cpp_style_name}} = {{field.name}};
        {{~end~}}
    }
{{~end~}}
    virtual ~{{name}}() {}

    bool deserialize(ByteBuf& _buf);

    {{~ for field in export_fields ~}}
    /**
     {{field.comment}}
     */
    {{cpp_define_type field.ctype}} {{field.cpp_style_name}};
    {{~if field.index_field~}} 
    std::unordered_map<{{cpp_define_type field.index_field.ctype}}, {{cpp_define_type field.ctype.element_type}}> {{field.cpp_style_name}}_Index;
    {{~end~}}
    {{~if field.gen_ref~}}
    {{field.cpp_ref_validator_define}}
    {{~end~}}
    {{~end~}}

{{~if !x.is_abstract_type~}}
    static constexpr int ID = {{x.id}};

    int getTypeId() const { return ID; }
{{~end~}}

    virtual void resolve(std::unordered_map<bright::String, void*>& _tables);
};

{{x.cpp_namespace_end}}

");
            var result = template.RenderCode(b);

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        public override string Render(DefTable p)
        {
            var template = t_tableRender ??= Template.Parse(@"
{{x.cpp_namespace_begin}}

{{~
    name = x.name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
~}}

/**
{{x.comment}}
 */
class {{name}}
{
    {{~if x.is_map_table ~}}
    private:
    std::unordered_map<{{cpp_define_type key_type}}, {{cpp_define_type value_type}}> _dataMap;
    std::vector<{{cpp_define_type value_type}}> _dataList;
    
    public:
    bool load(ByteBuf& _buf)
    {        
        int n;
        if (!_buf.readSize(n)) return false;
        for(; n > 0 ; --n)
        {
            {{cpp_define_type value_type}} _v;
            {{cpp_deserialize '_buf' '_v' value_type}}
            _dataList.push_back(_v);
            _dataMap[_v->{{x.index_field.cpp_style_name}}] = _v;
        }
        return true;
    }

    const std::unordered_map<{{cpp_define_type key_type}}, {{cpp_define_type value_type}}>& getDataMap() const { return _dataMap; }
    const std::vector<{{cpp_define_type value_type}}>& getDataList() const { return _dataList; }

    {{cpp_define_type value_type}} get({{cpp_define_type key_type}} key)
    { 
        auto it = _dataMap.find(key);
        return it != _dataMap.end() ? it->second : nullptr;
    }

    void resolve(std::unordered_map<bright::String, void*>& _tables)
    {
        for(auto v : _dataList)
        {
            v->resolve(_tables);
        }
    }

    {{~else~}}
     private:
    {{cpp_define_type value_type}} _data;

    public:
    {{cpp_define_type value_type}} data() const { return _data; }

    bool load(ByteBuf& _buf)
    {
        int n;
        if (!_buf.readSize(n)) return false;
        if (n != 1) return false;
        {{cpp_deserialize '_buf' '_data' value_type}}
        return true;
    }

    void resolve(std::unordered_map<bright::String, void*>& _tables)
    {
        _data->resolve(_tables);
    }

    {{~ for field in value_type.bean.hierarchy_export_fields ~}}
    /**
    {{field.comment}}
     */
    {{cpp_define_type field.ctype}}& {{field.cpp_getter_name}}() const { return _data->{{field.cpp_style_name}}; }
    {{~end~}}
    {{~end~}}
};
{{x.cpp_namespace_end}}
");
            var result = template.RenderCode(p);

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            var template = t_serviceRender ??= Template.Parse(@"
class {{name}}
{
    public:
    {{~for table in tables ~}}
    /**
     {{table.comment}}
     */
     {{table.cpp_full_name}} {{table.name}};
    {{~end~}}

    bool load(std::function<bool(ByteBuf&, const bright::String&)> loader)
    {
        std::unordered_map<bright::String, void*> __tables__;

        ByteBuf buf;
        {{~for table in tables~}}
        if (!loader(buf, ""{{table.output_data_file}}"")) return false;
        if (!{{table.name}}.load(buf)) return false;
        __tables__[""{{table.full_name}}""] = &{{table.name}};
        {{~end~}}

        {{~for table in tables ~}}
        {{table.name}}.resolve(__tables__); 
        {{~end~}}
        return true;
    }
};


");
            var result = template.Render(new
            {
                Name = name,
                Tables = tables,
            });

            return result;
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStub(string topModule, List<DefTypeBase> types)
        {
            var template = t_stubRender ??= Template.Parse(@"
#include <algorithm>
#include ""gen_types.h""

using ByteBuf = bright::serialization::ByteBuf;

namespace {{x.top_module}}
{
    {{~for type in x.types~}}

    bool {{type.cpp_full_name}}::deserialize(ByteBuf& _buf)
    {
        {{~if type.parent_def_type~}}
        if (!{{type.parent_def_type.cpp_full_name}}::deserialize(_buf))
        {
            return false;
        }
        {{~end~}}

        {{~ for field in type.export_fields ~}}
        {{cpp_deserialize '_buf' field.cpp_style_name field.ctype}}
        {{~end~}}

        return true;
    }

    bool {{type.cpp_full_name}}::deserialize{{type.name}}(ByteBuf& _buf, {{type.cpp_full_name}}*& _out)
    {
    {{~if type.is_abstract_type~}}
        int id;
        if (!_buf.readInt(id)) return false;
        switch (id)
        {
        {{~for child in type.hierarchy_not_abstract_children~}}
            case {{child.cpp_full_name}}::ID: { _out = new {{child.cpp_full_name}}(); if (_out->deserialize(_buf)) { return true; } else { delete _out; _out = nullptr; return false;} }
        {{~end~}}
            default: { _out = nullptr; return false;}
        }
    {{~else~}}
        _out = new {{type.cpp_full_name}}();
        if (_out->deserialize(_buf))
        {
            return true;
        }
        else
        { 
            delete _out;
            _out = nullptr;
            return false;
        }
    {{~end~}}
    }

    void {{type.cpp_full_name}}::resolve(std::unordered_map<bright::String, void*>& _tables)
    {
        {{~if type.parent_def_type~}}
        {{type.parent_def_type.name}}::resolve(_tables);
        {{~end~}}
        {{~ for field in type.export_fields ~}}
        {{~if field.gen_ref~}}
        {{cpp_ref_validator_resolve field}}
        {{~else if field.has_recursive_ref~}}
        {{cpp_recursive_resolve field '_tables'}}
        {{~end~}}
        {{~end~}}
    }
    {{~end~}}
}
");
            return template.RenderCode(new
            {
                TopModule = topModule,
                Types = types,
            });
        }
    }
}
