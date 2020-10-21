using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;
using System.Collections.Generic;

namespace Luban.Job.Cfg.Generate
{
    class GoCodeRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefConst c: return Render(c);
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                case DefTable r: return Render(r);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        [ThreadStatic]
        private static Template t_constRender;

        public string Render(DefConst c)
        {
            string package = "cfg";

            var template = t_constRender ??= Template.Parse(@"
package {{package}}

const (
    {{- for item in x.items }}
    {{x.go_full_name}}_{{item.name}} = {{go_const_value item.ctype item.value}}
    {{-end}}
)
");
            var result = template.RenderCode(c, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_enumRender;

        public string Render(DefEnum e)
        {
            string package = "cfg";

            var template = t_enumRender ??= Template.Parse(@"
package {{package}}

const (
    {{- for item in x.items }}
    {{x.go_full_name}}_{{item.name}} = {{item.value}}
    {{-end}}
)

");
            var result = template.RenderCode(e, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_beanRender;

        public string Render(DefBean b)
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

import ""bright/serialization""

{{x.go_import}}

type {{go_full_name}} struct {
    {{if parent_def_type}}{{parent_def_type.go_full_name}}{{end}}
    {{- for field in export_fields }}
    {{field.cs_style_name}} {{go_define_type field.ctype}}
    {{-end}}
}

{{if !is_abstract_type}}
func ({{go_full_name}}) GetTypeId() int {
    return {{x.id}}
}
{{end}}

func New{{go_full_name}}(_buf *serialization.ByteBuf) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{if parent_def_type}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{end}}
    {{- for field in export_fields }}
    {{go_deserialize_field field '_buf'}}
    {{-end}}
    return
}
{{if is_abstract_type}}
func NewChild{{go_full_name}}(_buf *serialization.ByteBuf) (_v interface{}, err error) {
    var id int32
    if id, err = _buf.ReadInt() ; err != nil {
        return
    }
    switch id {
            case 0 : return nil, nil
        {{- for child in hierarchy_not_abstract_children}}
            case {{child.id}}: return New{{child.go_full_name}}(_buf);
        {{-end}}
    }
    return
}
{{end}}

");
            var result = template.RenderCode(b, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }


        [ThreadStatic]
        private static Template t_tableRender;
        public string Render(DefTable p)
        {
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

import ""bright/serialization""

{{if x.is_two_key_map_table}}

type {{go_full_name}} struct {
    _dataListMap map[{{go_define_type key_type1}}][]{{go_define_type value_type}}
    _dataMapMap map[{{go_define_type key_type1}}]map[{{go_define_type key_type2}}]{{go_define_type value_type}}
    _dataList []{{go_define_type value_type}}
}

func New{{go_full_name}}(_buf *serialization.ByteBuf) (*{{go_full_name}}, error) {
	if size, err := _buf.ReadSize() ; err != nil {
		return nil, err
	} else {
		_dataList := make([]{{go_define_type value_type}}, 0, size)
		_dataListMap := make(map[{{go_define_type key_type1}}][]{{go_define_type value_type}})
        _dataMapMap := make(map[{{go_define_type key_type1}}]map[{{go_define_type key_type2}}]{{go_define_type value_type}})

		for i := 0 ; i < size ; i++ {
			if _v, err2 := {{go_deserialize_type value_type '_buf'}}; err2 != nil {
				return nil, err2
			} else {
				_dataList = append(_dataList, _v)
                
                _dataMap := _dataMapMap[_v.{{index_field1.cs_style_name}}]
                if _dataMap == nil {
                    _dataMap = make(map[{{go_define_type key_type2}}]{{go_define_type value_type}})
                    _dataMapMap[_v.{{index_field1.cs_style_name}}] = _dataMap
                }
                _dataMap[_v.{{index_field2.cs_style_name}}] = _v
                

                _dataList := _dataListMap[_v.{{index_field1.cs_style_name}}]
                if _dataList == nil {
                    _dataList = make([]{{go_define_type value_type}}, 0)
                }
                _dataList = append(_dataList, _v)
				_dataListMap[_v.{{index_field1.cs_style_name}}] = _dataList
			}
		}
		return &{{go_full_name}}{_dataList:_dataList, _dataMapMap:_dataMapMap, _dataListMap:_dataListMap}, nil
	}
}

func (table *{{go_full_name}}) GetDataMapMap() map[{{go_define_type key_type1}}]map[{{go_define_type key_type2}}]{{go_define_type value_type}} {
    return table._dataMapMap
}

func (table *{{go_full_name}}) GetDataListMap() map[{{go_define_type key_type1}}][]{{go_define_type value_type}} {
    return table._dataListMap
}

func (table *{{go_full_name}}) GetDataList() []{{go_define_type value_type}} {
    return table._dataList
}

func (table *{{go_full_name}}) Get(key1 {{go_define_type key_type1}}, key2 {{go_define_type key_type2}}) {{go_define_type value_type}} {
    if v , ok := table._dataMapMap[key1] ; ok {
        return v[key2]
    } else {
        return nil
    }
}


{{else if x.is_map_table }}
type {{go_full_name}} struct {
    _dataMap map[{{go_define_type key_type}}]{{go_define_type value_type}}
    _dataList []{{go_define_type value_type}}
}

func New{{go_full_name}}(_buf *serialization.ByteBuf) (*{{go_full_name}}, error) {
	if size, err := _buf.ReadSize() ; err != nil {
		return nil, err
	} else {
		_dataList := make([]{{go_define_type value_type}}, 0, size)
		dataMap := make(map[{{go_define_type key_type}}]{{go_define_type value_type}})

		for i := 0 ; i < size ; i++ {
			if _v, err2 := {{go_deserialize_type value_type '_buf'}}; err2 != nil {
				return nil, err2
			} else {
				_dataList = append(_dataList, _v)
				dataMap[_v.{{index_field.cs_style_name}}] = _v
			}
		}
		return &{{go_full_name}}{_dataList:_dataList, _dataMap:dataMap}, nil
	}
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


{{else}}

import ""errors""

type {{go_full_name}} struct {
    _data {{go_define_type value_type}}
}

func New{{go_full_name}}(_buf *serialization.ByteBuf) (*{{go_full_name}}, error) {
	if size, err := _buf.ReadSize() ; err != nil {
		return nil, err
    } else if size != 1 {
        return nil, errors.New("" size != 1 "")
	} else {
		if _v, err2 := {{go_deserialize_type value_type '_buf'}}; err2 != nil {
			return nil, err2
		} else {
		    return &{{go_full_name}}{_data:_v}, nil
		}
	}
}

func (table *{{go_full_name}}) Get() {{go_define_type value_type}} {
    return table._data
}

{{end}}
");
            var result = template.RenderCode(p, new Dictionary<string, object>() { ["package"] = package });

            return result;
        }

        [ThreadStatic]
        private static Template t_serviceRender;
        public string RenderService(string name, string module, List<DefTable> tables)
        {
            string package = "cfg";

            var template = t_serviceRender ??= Template.Parse(@"
package {{package}}

import ""bright/serialization""

type ByteBufLoader func(string) (*serialization.ByteBuf, error)

type {{name}} struct {
    {{- for table in tables }}
    {{table.name}} *{{table.go_full_name}}
    {{-end}}
}

func NewTables(loader ByteBufLoader) (*{{name}}, error) {
    var err error
    var buf *serialization.ByteBuf

    tables := &{{name}}{}
    {{- for table in tables }}
    if buf, err = loader(""{{table.output_data_file}}"") ; err != nil {
        return nil, err
    }
    if tables.{{table.name}}, err = New{{table.go_full_name}}(buf) ; err != nil {
        return nil, err
    }
    {{-end}}
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
