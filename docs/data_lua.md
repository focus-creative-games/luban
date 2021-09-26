[//]: # (Author: bug)
[//]: # (Date: 2020-11-01 16:26:41)

# Lua 数据

## 说明
* 与 json 相似定义。  
* 唯一区别在于， lua 的table的key支持任意格式，所以 lua 的map 可以直接  {[key1] = value1, [key2] = value2, ,,,}
* 注意 数据前有一个  return 语句。因为 lua 数据是当作 lua 文件加载的，每个加载后的结果当作一个记录读入。

## 例子
```xml
<table name="TbDataFromLua" value="DemoType2" input="test/lua_datas"/> 
```
其中 1.lua 文件内容如下
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
	k9 = {{y1=1,y2=true}, {y1=10,y2=false}},
	k15 = {{ __type__="DemoD2", x1 = 1, x2=3}},
}
```
