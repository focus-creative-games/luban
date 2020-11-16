using Luban.Job.Common.Defs;
using Luban.Job.Proto.Defs;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Proto.Generate
{
    class LuaRender
    {

        [ThreadStatic]
        private static Template t_allRender;
        public string RenderTypes(List<DefTypeBase> types)
        {
            var consts = types.Where(t => t is DefConst).ToList();
            var enums = types.Where(t => t is DefEnum).ToList();
            var beans = types.Where(t => t is DefBean).ToList();
            var protos = types.Where(t => t is DefProto).ToList();
            var rpcs = types.Where(t => t is DefRpc).ToList();
            var template = t_allRender ??= Template.Parse(@"
local setmetatable = setmetatable
local pairs = pairs
local ipairs = ipairs
local tinsert = table.insert

local function SimpleClass()
    local class = {}
    class.__index = class
    class.New = function(...)
        local ctor = class.ctor
        local o = ctor and ctor(...) or {}
        setmetatable(o, class)
        return o
    end
    return class
end


local function get_map_size(m)
    local n = 0
    for _ in pairs(m) do
        n = n + 1
    end
    return n
end

local consts =
{
    {{- for c in consts }}
    ---@class {{c.full_name}}
    {{- for item in c.items }}
     ---@field public {{item.name}} {{item.type}}
    {{-end}}
    ['{{c.full_name}}'] = {  {{ for item in c.items }} {{item.name}}={{item.to_lua_const_value}}, {{end}} };
    {{-end}}
}

local enums =
{
    {{- for c in enums }}
    ---@class {{c.full_name}}
    {{- for item in c.items }}
     ---@field public {{item.name}} int
    {{-end}}
    ['{{c.full_name}}'] = {  {{ for item in c.items }} {{item.name}}={{item.int_value}}, {{end}} };
    {{-end}}
}


local function InitTypes(methods)
    local readBool = methods.readBool
    local writeBool = methods.writeBool
    local readByte = methods.readByte
    local writeByte = methods.writeByte
    local readShort = methods.readShort
    local writeShort = methods.writeShort
    local readFshort = methods.readFshort
    local writeInt = methods.writeInt
    local readInt = methods.readInt
    local writeFint = methods.writeFint
    local readFint = methods.readFint
    local readLong = methods.readLong
    local writeLong = methods.writeLong
    local readFlong = methods.readFlong
    local writeFlong = methods.writeFlong
    local readFloat = methods.readFloat
    local writeFloat = methods.writeFloat
    local readDouble = methods.readDouble
    local writeDouble = methods.writeDouble
    local readSize = methods.readSize
    local writeSize = methods.writeSize

    local readString = methods.readString
    local writeString = methods.writeString
    local readBytes = methods.readBytes
    local writeBytes = methods.writeBytes

    local function readVector2(bs)
        return { x = readFloat(bs), y = readFloat(bs) }
    end
    
    local function writeVector2(bs, v)
        writeFloat(bs, v.x)
        writeFloat(bs, v.y)
    end

    local function readVector3(bs)
        return { x = readFloat(bs), y = readFloat(bs), z = readFloat(bs) }
    end
    
    local function writeVector3(bs, v)
        writeFloat(bs, v.x)
        writeFloat(bs, v.y)
        writeFloat(bs, v.z)
    end


    local function readVector4(bs)
        return { x = readFloat(bs), y = readFloat(bs), z = readFloat(bs), w = readFloat(bs) }
    end
    
    local function writeVector4(bs, v)
        writeFloat(bs, v.x)
        writeFloat(bs, v.y)
        writeFloat(bs, v.z)
        writeFloat(bs, v.w)
    end

    local function writeList(bs, list, keyFun)
        writeSize(bs, #list)
        for _, v in pairs(list) do 
            keyFun(bs, v)    
        end
    end

    local function readList(bs, keyFun)
        local list = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(list, keyFun(bs))
        end
        return list
    end

    local writeArray = writeList
    local readArray = readList

    local function writeSet(bs, set, keyFun)
        writeSize(bs, #set)
        for _, v in ipairs(set) do
            keyFun(bs, v)
        end
    end

    local function readSet(bs, keyFun)
        local set = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(set, keyFun(bs))
        end
        return set
    end

    local function writeMap(bs, map, keyFun, valueFun)
        writeSize(bs, get_map_size(map))
        for k, v in pairs(map) do
            keyFun(bs, k)
            valueFun(bs, v)
        end
    end

    local function readMap(bs, keyFun, valueFun)
        local map = {}
        for i = 1, readSize(bs) do
            local k = keyFun(bs)
            local v = valueFun(bs)
            map[k] = v
        end
        return map
    end

    local default_vector2 = {x=0,y=0}
    local default_vector3 = {x=0,y=0,z=0}
    local default_vector4 = {x=0,y=0,z=0,w=0}

    local beans = {}
{{ for bean in beans }}
    do
    ---@class {{bean.full_name}} {{if bean.parent_def_type}}:{{bean.parent}} {{end}}
    {{- for field in bean.fields}}
     ---@field public {{field.name}} {{field.lua_comment_type}}
    {{-end}}
        local class = SimpleClass()
        class._id = {{bean.id}}
        class._name = '{{bean.full_name}}'
        --local name2id = { {{for c in bean.hierarchy_not_abstract_children}} ['{{c.full_name}}'] = {{c.id}}, {{end}} }
        local id2name = { {{for c in bean.hierarchy_not_abstract_children}} [{{c.id}}] = '{{c.full_name}}', {{end}} }
{{if bean.is_abstract_type}}
        class._serialize = function(bs, self)
            writeInt(bs, self._id)
            beans[self._name]._serialize(bs, self)
        end
        class._deserialize = function(bs)
            local id = readInt(bs)
            return beans[id2name[id]]._deserialize(bs)
        end
{{else}}
        class._serialize = function(bs, self)
        {{- for field in bean.hierarchy_fields }}
            {{field.proto_lua_serialize_while_nil}}
        {{-end}}
        end
        class._deserialize = function(bs)
            local o = {
        {{- for field in bean.hierarchy_fields }}
            {{field.name}} = {{field.proto_lua_deserialize}},
        {{-end}}
            }
            setmetatable(o, class)
            return o
        end
{{end}}
        beans[class._name] = class
    end
{{end}}

    local protos = { }
{{ for proto in protos }}
    do
    ---@class {{proto.full_name}}
    {{- for field in proto.fields}}
     ---@field public {{field.name}} {{field.lua_comment_type}}
    {{-end}}
        local class = SimpleClass()
        class._id = {{proto.id}}
        class._name = '{{proto.full_name}}'
        class._serialize = function(self, bs)
        {{- for field in proto.fields }}
            {{field.proto_lua_serialize_while_nil}}
        {{-end}}
        end
        class._deserialize = function(self, bs)
        {{- for field in proto.fields }}
            self.{{field.name}} = {{field.proto_lua_deserialize}}
        {{-end}}
        end
        protos[class._id] = class
        protos[class._name] = class
    end
{{end}}

    local rpcs = { }
{{ for rpc in rpcs }}
    do
    ---@class {{rpc.full_name}}
        ---@field public is_request bool
        ---@field public rpc_id long
        ---@field public arg {{rpc.targ_type.lua_comment_type}}
        ---@field public res {{rpc.tres_type.lua_comment_type}}
        local class = SimpleClass()
        class._id = {{rpc.id}}
        class._name = '{{rpc.full_name}}'
        class._arg_name = '{{rpc.targ_type.bean.full_name}}'
        class._res_name = '{{rpc.tres_type.bean.full_name}}'
        class._serialize = function(self, bs)
            local composite_id = self.rpc_id * 2
            if self.is_request then
                writeLong(bs, composite_id)
                beans['{{rpc.targ_type.bean.full_name}}']._serialize(self.arg, bs)
            else
                writeLong(bs, composite_id + 1)
                beans['{{rpc.tres_type.bean.full_name}}']._serialize(self.res, bs)
            end
        end
        class._deserialize = function(self, bs)
            local composite_id = readLong(bs)
            self.rpc_id = composite_id // 2
            if composite_id % 2 == 0 then
                self.is_request = true
                self.arg = beans['{{rpc.targ_type.bean.full_name}}']._deserialize(bs)
            else
                self.is_request = false
                self.res = beans['{{rpc.tres_type.bean.full_name}}']._deserialize(bs)
            end
        end
        rpcs[class._id] = class
        rpcs[class._name] = class
    end
{{end}}

    return { consts = consts, enums = enums, beans = beans, protos = protos, rpcs = rpcs }
    end

return { InitTypes = InitTypes}


");
            return template.Render(new { Consts = consts, Enums = enums, Beans = beans, Protos = protos, Rpcs = rpcs });
        }
    }
}
