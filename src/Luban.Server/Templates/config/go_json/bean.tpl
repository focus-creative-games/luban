{{~
    go_full_name = x.go_full_name
    parent_def_type = x.parent_def_type
    is_abstract_type = x.is_abstract_type
    export_fields = x.export_fields
    hierarchy_not_abstract_children = x.hierarchy_not_abstract_children
~}}

package {{x.top_module}}

{{x.go_json_import}}

type {{go_full_name}} struct {
    {{~if parent_def_type~}}
    {{parent_def_type.go_full_name}}
    {{~end~}}
    {{~for field in export_fields ~}}
    {{field.convention_name}} {{go_define_type field.ctype}}
    {{~end~}}
}

{{~if !is_abstract_type~}}
func ({{go_full_name}}) GetTypeId() int {
    return {{x.id}}
}
{{~end~}}

{{~if is_abstract_type~}}
func New{{go_full_name}}(_buf map[string]interface{}) (_v interface{}, err error) {
    var id string
    var _ok_ bool
    if id, _ok_ = _buf["__type__"].(string) ; !_ok_ {
        return nil, errors.New("type id missing")
    }
    switch id {
        {{~for child in hierarchy_not_abstract_children~}}
        case "{{child.name}}": return New{{child.go_full_name}}(_buf);
        {{~end~}}
        default: return nil, errors.New("unknown type id")
    }
    return
}

func New{{go_full_name}}_Body(_buf map[string]interface{}) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{~if parent_def_type~}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}_Body(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{~end~}}
    {{~for field in export_fields ~}}
    {{go_deserialize_json_field field.ctype ("_v." + field.convention_name) field.name '_buf'}}
    {{~end~}}
    return
}
{{~else~}}
func New{{go_full_name}}(_buf map[string]interface{}) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{~if parent_def_type~}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}_Body(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{~end~}}
    {{~for field in export_fields ~}}
    {{go_deserialize_json_field field.ctype ("_v." + field.convention_name) field.name '_buf'}}
    {{~end~}}
    return
}
{{~end~}}
