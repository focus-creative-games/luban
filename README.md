
# Luban

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban)  
![Docker Auto](https://img.shields.io/docker/cloud/automated/hugebug4ever/luban.svg)
![Docker Build](https://img.shields.io/docker/cloud/build/hugebug4ever/luban.svg)

![icon](docs/images/icon.png)

-----

## links

- [README - English](./README.en-us.md)
- [github link](https://github.com/focus-creative-games/luban)
- [gitee link](https://gitee.com/focus-creative-games/luban)

-----

## 介绍

luban是你的**最佳游戏配置解决方案**。

luban高效地处理游戏开发中常见的excel、json、xml之类的数据，检查数据错误，生成c#等各种语言的代码，导出成bytes或json等多种格式。

luban统一了游戏配置开发工作流，极大提升了策划和程序的工作效率。

## 核心特性

- 强大的数据解析和转换能力 {excel(csv,xls,xlsx), json, bson, xml, yaml, lua, unity ScriptableObject} => {binary, json, bson, xml, lua, yaml, erlang}
- 支持生成c#,java,go,c++,lua,python,javascript,typescript,erlang,rust代码
- 增强的excel格式，可以简洁地配置出像简单列表、子结构、结构列表，以及任意复杂的深层次的嵌套结构。
- 支持json、lua、xml等格式的数据来表达行为树、技能、剧情、副本之类复杂GamePlay数据
- 支持生成 protobuf(schema + binary + json)、flatbuffers(schema + json)、msgpack(binary)
- 强大的数据校验能力。ref引用检查，path资源路径检查等等。
- 支持unity、unreal、cocos2x、微信小游戏等主流引擎及 win、linux、mac等平台。
- 支持xLua、toLua、ILRuntime、Puerts、unLua、sLuaUnreal、XIL等等主流热更新方案
- 支持skynet、ET、GameFramework、QFramework、xlua-famework、KSFramework等第三方框架
- 完善的本地化支持。
- 强大灵活的自定义能力，支持代码模板和数据模板
- **==通用型生成和缓存工具==**。也可以用于生成协议、数据库之类的代码，甚至可以用作对象缓存服务。

完整特性请参见 [feature](https://focus-creative-games.github.io/lubandoc/feature.html)

## 文档

- [快速上手](https://focus-creative-games.github.io/lubandoc/start_up.html)
- [Document](https://focus-creative-games.github.io/lubandoc) ，比较完善，有使用疑问，请先查看此文档。
- **示例项目** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples))
- [版本变更记录](https://focus-creative-games.github.io/lubandoc/changelog.html)

- 支持与联系
  - QQ群: 692890842 （Luban开发交流群）。有使用方面的疑问请及时加QQ群询问，随时有人帮助解决。
  - 邮箱: taojingjian#gmail.com

## excel格式速览

完整示例请详见 [excel格式介绍](https://focus-creative-games.github.io/lubandoc/excel.html)

### 普通表

|##var| id | x1 | x5 | x6  | s1   | s2&sep=#  | v3  | t1 |
| -|- | -| -| -| -| - | - | - |
|##type|int|bool|long|float|string|text|vector3|datetime|
|##|id|desc1|desc2|desc3|desc4|desc7|desc1|time|
|| 1|false| 1000| 1.2| hello |key1#world1|1,2,3|1999-10-10 11:12:13|
|| 2|true| 1000| 2.4|world |key2#world2|2,4,5|1999-10-12 11:12:13|

### 多行表1

<table border="1">
<tr align="center">
<td>##var</td>
<td>id</td>
<td colspan="6">rewards1</td>
<td colspan="3">rewards2</td>
</tr>
<tr align="center">
<td>##type</td>
<td>int</td>
<td colspan="6">list,Reward</td>
<td colspan="3">list,Reward#sep=,</td>
</tr>
<tr align="center">
<td>##</td>
<td>id</td>
<td colspan="6">reward list desc1</td>
<td colspan="3">reward list desc2</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>1001</td><td>1</td><td>desc1</td><td>1002</td><td>2</td><td>desc2</td>
<td>1001,1,desc1</td><td>1002,2,desc2</td><td>1003,3,desc3</td>
</tr>

<tr align="center">
<td/>
<td>2</td>
<td>1001</td><td>1</td><td>desc1</td><td></td><td></td><td></td>
<td>1001,1,desc1</td><td>1002,2,desc2</td><td></td>
</tr>

</table>

### 多行表2

<table border="1">

<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="6">stages</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td>string</td>
  <td colspan="6">list,Stage</td>
</tr>
<tr align="center">
  <td>##var</td>
  <td/>
  <td/>
  <td>id</td>
  <td>name</td>
  <td>desc</td>
  <td>location</td>
  <td>item_id</td>
  <td>num</td>
</tr>
<tr align="center">
  <td>##</td>
  <td>id</td>
  <td>desc1</td>
  <td>desc1</td>
  <td>desc2</td>
  <td>desc3</td>
  <td>desc4</td>
  <td>desc5</td>
  <td>desc6</td>
</tr>

<tr align="center">
<td/>
<td>1</td>
<td>task1</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>3</td><td>stage3</td><td>stage desc3</td><td>1,2,3</td><td>1002</td><td>1</td>
</tr>
<tr align="center">
<td/><td>2</td>
<td>task2</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1002</td><td>1</td>
</tr>

</table>

### 多行表3

<table border="1">

<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="9">rewards</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td>string</td>
  <td colspan="9">list,Reward</td>
</tr>

<tr align="center">
  <td>##var</td>
  <td></td>
  <td></td>
  <td colspan="3">0</td>
  <td colspan="3">1</td>
  <td colspan="3">2</td>
</tr>
<tr align="center">
  <td>##var</td>
  <td/>
  <td/>
  <td>item_id</td><td>num</td><td>desc</td>
  <td>item_id</td><td>num</td><td>desc</td>
  <td>item_id</td><td>num</td><td>desc</td>
</tr>

<tr align="center"><td/><td>1</td><td>task1</td><td>1001</td><td>10</td><td>desc1</td><td>1002</td><td>12</td><td>desc2</td><td>1003</td><td>13</td><td>desc3</td></tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>1003</td><td>30</td><td>desc3</td><td>1004</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>1005</td><td>50</td><td>desc5</td><td/><td/><td/><td/><td/><td/></tr>
</table>

### 类型继承（适合技能、buff相关配置）

<table border="1">

<tr align="center"><td>##var</td><td>id</td><td colspan="4">shape</td><td colspan="4">shape2</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="4">Shape</td><td colspan="4">Shape</td></tr>
<tr align="center"><td>##var</td><td></td><td>\_\_type\_\_</td><td>radius</td><td>width</td><td>height</td><td></td><td></td><td></td><td></td></tr>
<tr align="center"><td/><td>1</td><td>Circle</td><td>10</td><td/><td/><td>Circle</td><td>100</td><td></td><td></td></tr>
<tr align="center"><td/><td>2</td><td>Rectangle</td><td></td><td>10</td><td>20</td><td>矩形</td><td>10</td><td>20</td><td></td></tr>
<tr align="center"><td/><td>3</td><td>圆</td><td>10</td><td/><td/><td>Triangle</td><td>15</td><td>15</td><td>15</td></tr>
<tr align="center"><td/><td>4</td><td>Circle</td><td>10</td><td/><td/><td>Rectangle</td><td>30</td><td>20</td><td></td></tr>

</table>

### 多主键表（联合索引）

多个key构成联合唯一主键。

|##var|key1|key2|key3| num|
|-|-|-|-|-|
|##type|int|long|string|int|
||1|1|aaa|123|
||1|1|bbb|124|
||1|2|aaa|134|
||2|1|aaa|124|
||5|6|xxx|898|

### 单例表

有一些配置全局只有一份，比如 公会模块的开启等级，背包初始大小，背包上限。此时使用单例表来配置这些数据比较合适。

|##var| guild_open_level | bag_init_capacity | bag_max_capacity | newbie_tasks |
| - |- | - | - | - |
| ##type | int | int | int | list,int|
| ## |desc1 | desc 2 | desc 3 | desc 4 |
| | 10 | 100| 500| 10001,10002 |

### 纵表

<table border="1">
<tr align="center">
<td>##var&column</td>
<td>##type</td>
<td>##</td>
<td></td>
</tr>
<tr align="center">
<td>guild_open_level</td><td>int</td><td>desc1</td><td>10</td>
</tr>
<tr align="center">
<td>bag_init_capacity</td><td>int</td><td>desc2</td><td>100</td>
</tr>
<tr align="center">
<td>bag_max_capacity</td><td>int</td><td>desc3</td><td>500</td>
</tr>
<tr align="center">
<td>newbie_tasks</td><td>list,int</td><td>desc4</td><td>10001,10002</td>
</tr>
</table>

## json、lua、xml、yaml 格式速览

以行为树为例，展示json格式下如何配置行为树配置。xml、lua、yaml等等格式请参见 [详细文档](https://focus-creative-games.github.io/lubandoc)。


```json
{
  "id": 10002,
  "name": "random move",
  "desc": "demo behaviour tree",
  "executor": "SERVER",
  "blackboard_id": "demo",
  "root": {
    "__type__": "Sequence",
    "id": 1,
    "node_name": "test",
    "desc": "root",
    "services": [],
    "decorators": [
      {
        "__type__": "UeLoop",
        "id": 3,
        "node_name": "",
        "flow_abort_mode": "SELF",
        "num_loops": 0,
        "infinite_loop": true,
        "infinite_loop_timeout_time": -1
      }
    ],
    "children": [
      {
        "__type__": "UeWait",
        "id": 30,
        "node_name": "",
        "ignore_restart_self": false,
        "wait_time": 1,
        "random_deviation": 0.5,
        "services": [],
        "decorators": []
      },
      {
        "__type__": "MoveToRandomLocation",
        "id": 75,
        "node_name": "",
        "ignore_restart_self": false,
        "origin_position_key": "x5",
        "radius": 30,
        "services": [],
        "decorators": []
      }
    ]
  }
}
```

## 代码使用预览

这儿只简略展示c#、typescript、go语言在开发中的用法，更多语言以及更详细的使用范例和代码见[示例项目](https://github.com/focus-creative-games/luban_examples)。

- C# 使用示例

```C#
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes(gameConfDir + "/" + file + ".bytes")));
// 访问一个单例表
Console.WriteLine(tables.TbGlobal.Name);
// 访问普通的 key-value 表
Console.WriteLine(tables.TbItem.Get(12).Name);
// 支持 operator []用法
Console.WriteLine(tables.TbMail[1001].Desc);
```

- typescript 使用示例

```typescript
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))
// 访问一个单例表
console.log(tables.TbGlobal.name)
// 访问普通的 key-value 表
console.log(tables.TbItem.get(12).Name)
```

- go 使用示例

```go
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
if tables , err := cfg.NewTables(loader) ; err != nil {
 println(err.Error())
 return
}
// 访问一个单例表
println(tables.TbGlobal.Name)
// 访问普通的 key-value 表
println(tables.TbItem.Get(12).Name)

```

## 路线图

- [ ] 新增 unity 内置编辑器
- [ ] 新增 unreal 内置编辑器
- [ ] 补充单元测试

## License

Luban is licensed under the [MIT](LICENSE.TXT) license.
