using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class GoJsonCodeRender : GoCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_beanRender;
        protected override string Render(DefBean b)
        {
            string package = "cfg";

            var template = t_beanRender ??= Template.Parse(@"
{{-
    go_full_name = x.go_full_name
    parent_def_type = x.parent_def_type
    is_abstract_type = x.is_abstract_type
    export_fields = x.export_fields
    hierarchy_not_abstract_children = x.hierarchy_not_abstract_children
-}}

package {{package}}

{{x.go_import}}

type {{go_full_name}} struct {
    {{~if parent_def_type~}}
    {{parent_def_type.go_full_name}}
    {{~end~}}
    {{~for field in export_fields ~}}
    {{field.cs_style_name}} {{go_define_type field.ctype}}
    {{~end~}}
}

{{~if !is_abstract_type~}}
func ({{go_full_name}}) GetTypeId() int {
    return {{x.id}}
}
{{~end~}}

func New{{go_full_name}}(_buf map[string]interface{}) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{~if parent_def_type~}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{~end~}}
    {{~for field in export_fields ~}}
    {{go_deserialize_json_field field.ctype (""_v."" + field.go_style_name) field.name '_buf'}}
    {{~end~}}
    return
}
{{~if is_abstract_type~}}
func NewChild{{go_full_name}}(_buf map[string]interface{}) (_v interface{}, err error) {
    var id string
    var _ok_ bool
    if id, _ok_ = _buf[""__type__""].(string) ; !_ok_ {
        return nil, errors.New(""type id missing"")
    }
    switch id {
        {{~for child in hierarchy_not_abstract_children~}}
        case ""{{child.name}}"": return New{{child.go_full_name}}(_buf);
        {{~end~}}
        default: return nil, errors.New(""unknown type id"")
    }
    return
}
{{~end~}}

");
            var result = template.RenderCode(b, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_tableRender;
        protected override string Render(DefTable p)
        {
            // TODO 目前只有普通表支持多态. 单例表和双key表都不支持
            string package = "cfg";
            var template = t_tableRender ??= Template.Parse(@"
{{-
    go_full_name = x.go_full_name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
    index_field = x.index_field
    index_field1 = x.index_field1
    index_field2 = x.index_field2
-}}

package {{package}}

{{~if x.is_map_table~}}
type {{go_full_name}} struct {
    _dataMap map[{{go_define_type key_type}}]{{go_define_type value_type}}
    _dataList []{{go_define_type value_type}}
}

func New{{go_full_name}}(_buf []map[string]interface{}) (*{{go_full_name}}, error) {
	_dataList := make([]{{go_define_type value_type}}, 0, len(_buf))
	dataMap := make(map[{{go_define_type key_type}}]{{go_define_type value_type}})
	for _, _ele_ := range _buf {
		if _v, err2 := {{go_deserialize_type value_type '_ele_'}}; err2 != nil {
			return nil, err2
		} else {
			_dataList = append(_dataList, _v)
{{~if value_type.is_dynamic ~}}
    {{~for child in value_type.bean.hierarchy_not_abstract_children~}}
            if __v, __is := _v.(*{{child.go_full_name}}) ; __is {
                dataMap[__v.{{index_field.cs_style_name}}] = _v
                continue
            }
    {{~end~}}
{{~else~}}
			dataMap[_v.{{index_field.cs_style_name}}] = _v
{{~end~}}
		}
	}
	return &{{go_full_name}}{_dataList:_dataList, _dataMap:dataMap}, nil
}

func (table *{{go_full_name}}) GetDataMap() map[{{go_define_type key_type}}]{{go_define_type value_type}} {
    return table._dataMap
}

func (table *{{go_full_name}}) GetDataList() []{{go_define_type value_type}} {
    return table._dataList
}

func (table *{{go_full_name}}) Get(key {{go_define_type key_type}}) {{go_define_type value_type}} {
    return table._dataMap[key]
}


{{~else~}}

import ""errors""

type {{go_full_name}} struct {
    _data {{go_define_type value_type}}
}

func New{{go_full_name}}(_buf []map[string]interface{}) (*{{go_full_name}}, error) {
	if len(_buf) != 1 {
        return nil, errors.New("" size != 1 "")
	} else {
		if _v, err2 := {{go_deserialize_type value_type '_buf[0]'}}; err2 != nil {
			return nil, err2
		} else {
		    return &{{go_full_name}}{_data:_v}, nil
		}
	}
}

func (table *{{go_full_name}}) Get() {{go_define_type value_type}} {
    return table._data
}

{{~end~}}
");
            var result = template.RenderCode(p, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            string package = "cfg";

            var template = t_serviceRender ??= Template.Parse(@"

package {{package}}

type JsonLoader func(string) ([]map[string]interface{}, error)

type {{name}} struct {
    {{~for table in tables ~}}
    {{table.name}} *{{table.go_full_name}}
    {{~end~}}
}

func NewTables(loader JsonLoader) (*{{name}}, error) {
    var err error
    var buf []map[string]interface{}

    tables := &{{name}}{}
    {{~for table in tables ~}}
    if buf, err = loader(""{{table.json_output_data_file}}"") ; err != nil {
        return nil, err
    }
    if tables.{{table.name}}, err = New{{table.go_full_name}}(buf) ; err != nil {
        return nil, err
    }
    {{~end~}}
    return tables, nil
}

");
            var result = template.Render(new
            {
                Name = name,
                Namespace = module,
                Tables = tables,
                Package = package,
            });

            return result;
        }
    }
}
