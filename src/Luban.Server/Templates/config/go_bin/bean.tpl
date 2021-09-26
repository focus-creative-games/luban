{{-
    go_full_name = x.go_full_name
    parent_def_type = x.parent_def_type
    is_abstract_type = x.is_abstract_type
    export_fields = x.export_fields
    hierarchy_not_abstract_children = x.hierarchy_not_abstract_children
-}}

package {{package}}

import (
    "bright/serialization"
)

{{x.go_bin_import}}

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

{{~if is_abstract_type~}}
func New{{go_full_name}}(_buf *serialization.ByteBuf) (_v interface{}, err error) {
    var id int32
    if id, err = _buf.ReadInt() ; err != nil {
        return
    }
    switch id {
        {{~for child in hierarchy_not_abstract_children~}}
        case {{child.id}}: return New{{child.go_full_name}}(_buf)
        {{~end~}}
        default: return nil, errors.New("unknown type id")
    }
    return
}

func New{{go_full_name}}_Body(_buf *serialization.ByteBuf) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{~if parent_def_type~}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}_Body(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{~end~}}
    {{~for field in export_fields ~}}
    {{go_deserialize_field field.ctype ("_v." + field.go_style_name) '_buf'}}
    {{~end~}}
    return
}

{{~else~}}
func New{{go_full_name}}(_buf *serialization.ByteBuf) (_v *{{go_full_name}}, err error) {
    _v = &{{go_full_name}}{}
{{~if parent_def_type~}}
    var _p *{{parent_def_type.go_full_name}}
     if _p, err = New{{parent_def_type.go_full_name}}_Body(_buf) ; err != nil { return }
    _v.{{parent_def_type.go_full_name}} = *_p
{{~end~}}
    {{~for field in export_fields ~}}
    {{go_deserialize_field field.ctype ("_v." + field.go_style_name) '_buf'}}
    {{~end~}}
    return
}
{{~end~}}
