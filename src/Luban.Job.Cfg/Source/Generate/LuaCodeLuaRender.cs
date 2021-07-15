using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Cfg.Generate
{
    class LuaCodeLuaRender : LuaCodeRenderBase
    {
        [ThreadStatic]
        private static Template t_allRender;

        public override string RenderAll(List<DefTypeBase> types)
        {
            var consts = types.Where(t => t is DefConst).ToList();
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var tables = types.Where(t => t is DefTable).ToList();
            var template = t_allRender ??= Template.Parse(LuaStringTemplate.BaseDefines + @"
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
    { name='{{table.name}}', file='{{table.output_data_file_escape_dot}}', mode='map', index='{{table.index}}', value_type='{{table.value_ttype.bean.full_name}}' },
    {{~else~}}
    { name='{{table.name}}', file='{{table.output_data_file_escape_dot}}', mode='one', value_type='{{table.value_ttype.bean.full_name}}'},
    {{end}}
{{~end~}}
}

return { consts = consts, enums = enums, beans = beans, tables = tables }

");
            return template.RenderCode(new { Consts = consts, Enums = enums, Beans = beans, Tables = tables });
        }
    }
}
