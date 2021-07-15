using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Common.Utils
{
    public static class LuaStringTemplate
    {
        public const string BaseDefines = @"
{{
    consts = x.consts
    enums = x.enums
    beans = x.beans
    tables = x.tables
}}
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
end";

        public const string MethodHeader = @"local function InitTypes(methods)
    local readBool = methods.readBool
    local readByte = methods.readByte
    local readShort = methods.readShort
    local readFshort = methods.readFshort
    local readInt = methods.readInt
    local readFint = methods.readFint
    local readLong = methods.readLong
    local readFlong = methods.readFlong
    local readFloat = methods.readFloat
    local readDouble = methods.readDouble
    local readSize = methods.readSize

    local readString = methods.readString

    local function readVector2(bs)
        return { x = readFloat(bs), y = readFloat(bs) }
    end

    local function readVector3(bs)
        return { x = readFloat(bs), y = readFloat(bs), z = readFloat(bs) }
    end

    local function readVector4(bs)
        return { x = readFloat(bs), y = readFloat(bs), z = readFloat(bs), w = readFloat(bs) }
    end

    local function readList(bs, keyFun)
        local list = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(list, keyFun(bs))
        end
        return list
    end

    local readArray = readList

    local function readSet(bs, keyFun)
        local set = {}
        local v
        for i = 1, readSize(bs) do
            tinsert(set, keyFun(bs))
        end
        return set
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

    local function readNullableBool(bs)
        if readBool(bs) then
            return readBool(bs)
        end
    end";
    }
}
