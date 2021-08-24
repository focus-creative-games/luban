package {{package}}

const (
    {{~for item in x.items ~}}
    {{x.go_full_name}}_{{item.name}} = {{go_const_value item.ctype item.value}}
    {{~end~}}
)
