package {{x.top_module}}

const (
    {{~for item in x.items ~}}
    {{x.go_full_name}}_{{item.name}} = {{item.int_value}}
    {{~end~}}
)
