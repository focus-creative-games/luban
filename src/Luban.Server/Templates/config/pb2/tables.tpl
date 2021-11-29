{{~
    name = x.name
    namespace = x.namespace
    tables = x.tables
~}}

message {{name}} {
{{~index = 0~}}
{{~for table in tables~}}
    {{~index = index + 1~}}
    {{table.pb_full_name}} {{name}} = {{index}};
{{~end~}}
}
