package {{package}}

const (
    {{~for item in x.items ~}}
    {{x.go_full_name}}_{{item.name}} = {{item.int_value}}
    {{~end~}}
)
