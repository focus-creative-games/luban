local consts =
{
    {{~ for c in consts ~}}
    ---@class {{c.full_name}}
    {{~ for item in c.items ~}}
     ---@field public {{item.name}} {{item.type}}
    {{~end~}}
    ['{{c.full_name}}'] = {  {{ for item in c.items }} {{item.name}}={{lua_const_value item.ctype item.value}}, {{end}} };
    {{~end~}}
}

local enums =
{
    {{~ for c in enums ~}}
    ---@class {{c.full_name}}
    {{~ for item in c.items ~}}
     ---@field public {{item.name}} int
    {{~end~}}
    ['{{c.full_name}}'] = {  {{ for item in c.items }} {{item.name}}={{item.int_value}}, {{end}} };
    {{~end~}}
}

local beans = {}
{{~ for bean in beans ~}}
---@class {{bean.full_name}} {{if bean.parent_def_type}}:{{bean.parent}} {{end}}
{{~ for field in bean.export_fields~}}
---@field public {{field.name}} {{lua_comment_type field.ctype}}
{{~end~}}
beans['{{bean.full_name}}'] =
{
{{~ for field in bean.hierarchy_export_fields ~}}
    { name='{{field.name}}', type='{{lua_comment_type field.ctype}}'},
{{~end~}}
}

{{~end~}}

local tables =
{
{{~for table in tables ~}}
    {{~if table.is_map_table ~}}
    { name='{{table.name}}', file='{{table.output_data_file}}', mode='map', index='{{table.index}}', value_type='{{table.value_ttype.bean.full_name}}' },
    {{~else~}}
    { name='{{table.name}}', file='{{table.output_data_file}}', mode='one', value_type='{{table.value_ttype.bean.full_name}}'},
    {{end}}
{{~end~}}
}

return { consts = consts, enums = enums, beans = beans, tables = tables }
