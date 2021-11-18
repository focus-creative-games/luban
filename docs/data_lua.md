[//]: # (Author: bug)
[//]: # (Date: 2020-11-01 16:26:41)

# Lua 数据源

## json数据格式

大多数数据格式符合直觉，有几个特殊点：
* 数据前有一个return，这是因为 lua 数据是当作 lua 文件加载的，每个加载后的结果当作一个记录读入。
* set 的格式为 {v1, v2, ...}
* 与json不同，lua 的table的key支持任意格式，所以lua的map可以直接  {[key1] = value1, [key2] = value2, ,,,} 


定义

```xml
<bean name="DemoType2" >
	<var name="x4" type="int"/>
	<var name="x1" type="bool"/>
	<var name="x5" type="long"/>
	<var name="x6" type="float"/>
	<var name="x7" type="double"/>
	<var name="x10" type="string"/>
	<var name="x12" type="DemoType1"/>
	<var name="x13" type="DemoEnum"/>
	<var name="x14" type="DemoDynamic"/>
	<var name="v2" type="vector2"/>
	<var name="v3" type="vector3"/>
	<var name="v4" type="vector4"/>
	<var name="t1" type="datetime"/>
	<var name="k1" type="array,int"/>
	<var name="k2" type="list,int"/>
	<var name="k8" type="map,int,int"/>
	<var name="k9" type="list,DemoE2"/>
	<var name="k15" type="array,DemoDynamic" /> 
</bean>
```

示例数据

```lua
return 
{
	x1 = false,
	x2 = 2,
	x3 = 128,
	x4 = 1122,
	x5 = 112233445566,
	x6 = 1.3,
	x7 = 1122,
	x10 = "yf",
	x12 = {x1=1},
	x13 = "D",
	x14 = { __type__="DemoD2", x1 = 1, x2=3},
	v2 = {x= 1,y = 2},
	v3 = {x=0.1, y= 0.2,z=0.3},
	v4 = {x=1,y=2,z=3.5,w=4},
	t1 = "1970-01-01 00:00:00",
	k1 = {1,2},
	k2 = {2,3},
	k8 = {[2]=10,[3]=12},
	k9 = { {y1=1,y2=true}, {y1=10,y2=false} },
	k15 = { { __type__="DemoD2", x1 = 1, x2=3} },
}
```

## 以复合lua文件形式组织

整个表在一个或者多个lua文件中组织。用法与json数据源相似，参见 [json数据源](./data_json.md)

## 以目录树形式组织

典型用法是，以目录为数据源（会遍历整棵目录树），目录树下每个lua文件为一个记录读入。如下示例，递归遍历test/lua_datas整个目录树，**按文件名排序后**依次将每个lua数据当作一个记录读入。

```xml
<table name="TbDataFromLua" value="DemoType2" input="test/lua_datas"/> 
```

## 数据tag
与excel格式类似，也支持记录tag，用 \_\_tag\_\_ 属性来指明tag，示例如下：

```lua
return {
	__tag__ = "dev",
	x = 1,
	y = 2,
}
```