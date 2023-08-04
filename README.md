
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

- 强大的数据解析和转换能力 {excel(csv,xls,xlsx)、json、bson、xml、yaml、lua、unity ScriptableObject} => {binary、json、bson、xml、lua、yaml、erlang、 custom format}
- 增强的excel格式，可以简洁地配置出像简单列表、子结构、结构列表，以及任意复杂的深层次的嵌套结构。
- 完备的类型系统，**支持OOP类型继承**，搭配excel、json、lua、xml等格式数据**灵活优雅**表达行为树、技能、剧情、副本之类复杂GamePlay数据
- 支持生成c#、java、go、c++、lua、python、javascript、typescript、erlang、rust、gdscript 代码
- 支持生成 protobuf(schema + binary + json)、flatbuffers(schema + json)、msgpack(binary)
- 强大的数据校验能力。ref引用检查、path资源路径、range范围检查等等
- 完善的本地化支持。静态文本值本地化、动态文本值本地化、时间本地化、main-patch多地区版本
- 强大灵活的自定义能力，支持自定义代码模板和数据模板
- **通用型生成和缓存工具**。也可以用于生成协议、数据库之类的代码，甚至可以用作对象缓存服务
- **良好支持主流引擎、全平台、主流热更新方案、主流前后端框架**。支持Unity、Unreal、Cocos2x、Godot、微信小游戏等主流引擎。工具自身跨平台，能在Win,Linux,Mac平台良好工作。

完整特性请参见 [feature](https://focus-creative-games.github.io/luban-doc/#/manual/traits)

## 文档

- [官方文档](https://focus-creative-games.github.io/luban-doc/)
- [快速上手](https://focus-creative-games.github.io/luban-doc/#/beginner/quickstart)
- **示例项目** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples))
- [版本变更记录](https://focus-creative-games.github.io/luban-doc/#/changelog)
- 支持与联系
  - QQ群: 692890842 （Luban开发交流群）。有使用方面的疑问请及时加QQ群询问，随时有人帮助解决。
  - 邮箱: luban@code-philosophy.com

## excel格式速览

### 普通表

|##var| id | x1 | x5 | x6  | s1   | s2  | v3  | t1 |
| -|- | -| -| -| -| - | - | - |
|##type|int|bool|long|float|string|text#sep=\||vector3|datetime|
|##|id|desc1|desc2|desc3|desc4|desc7|desc1|time|
|| 1|false| 1000| 1.2| hello |key1\|world1|1,2,3|1999-10-10 11:12:13|
|| 2|true| 1000| 2.4|world |key2\|world2|2,4,5|1999-10-12 11:12:13|


### 原生数据列表

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>arr1</td>
  <td colspan="4">arr2</td>
  <td>arr3</td>
  <td colspan="3">arr4</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</id>
  <td>(array#sep=;),int</td>
  <td colspan="4">list,int</td>
  <td>(list#sep=|),string</td>
  <td colspan="3">list,string</td>
</tr>
<tr align="center">
  <td>##</td>
  <td>id</id>
  <td>desc1</td>
  <td colspan="4">desc2</td>
  <td>desc3</td>
  <td colspan="3">desc4</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>1;2;3</td>
<td>1</td><td>2</td><td></td><td></td>
<td>xx|yy</td>
<td>xxx</td><td>zzz</td><td></td>
</tr>
<tr align="center">
<td/>
<td>2</td>
<td>2;4</td>
<td>3</td><td>4</td><td>5</td><td></td>
<td>aaaa|bbbb|cccc</td>
<td>aaa</td><td>bbb</td><td>ccc</td>
</tr>
<tr align="center">
<td/>
<td>3</td>
<td>2;4;6</td>
<td>3</td><td>4</td><td>5</td><td>6</td>
<td>aaaa|bbbb|cccc</td>
<td>aaa</td><td>bbb</td><td>ccc</td>
</tr>
</table>

### 子结构

Reward为包含 "int item_id; int count; string desc; " 这三个字段的子结构。

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td colspan="3">reward</td><td colspan="3">reward2</td><td>reward3</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="3">Reward</td><td colspan="3">Reward</td><td>Reward#sep=,</td></tr>
<tr align="center"><td>##var</td><td></td><td>item_id</td><td>count</td><td>desc</td><td></td><td></td><td></td><td/></tr>
<tr align="center"><td/><td>1</td><td>1001</td><td>10</td><td>item 1</td><td>1002</td><td>11</td><td>item 2</td><td>1002,1,item 3</td></tr>
<tr align="center"><td/><td>2</td><td>2001</td><td>10</td><td>item 2</td><td>2002</td><td>20</td><td>item 4</td><td>2003,2,item 5</td></tr>

</table>

### 结构列表 1

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

### 结构列表 2

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
<tr align="center"><td/><td>1</td><td>task1</td><td>1001</td><td>10</td><td>desc1</td><td>1002</td><td>12</td><td>desc2</td><td>1003</td><td>13</td><td>desc3</td></tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>1003</td><td>30</td><td>desc3</td><td>1004</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>1005</td><td>50</td><td>desc5</td><td/><td/><td/><td/><td/><td/></tr>
</table>

### 结构列表 3

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

### 多行表

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="6">*stages</td>
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

### 多级多行表

多行表的列表字段，每个列表元素还可以是多行。支持任意多级的多行嵌套。另外也允许有多个多行字段，每个字段的行数还可以不同。

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="10">*stages</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td>string</td>
  <td colspan="10">list,Stage</td>
</tr>
<tr align="center">
  <td>##var</td>
  <td/>
  <td/>
  <td>id</td>
  <td>name</td>
  <td>desc</td>
  <td colspan="3">*tips</td>
  <td colspan="4">*rules</td>
</tr>
<tr align="center">
  <td>##var</td>
  <td/>
  <td/>
  <td></td>
  <td></td>
  <td></td>
  <td>location</td>
  <td>item_id</td>
  <td>num</td>
  <td>id</td>
  <td>name</td>
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
  <td>id</td>
  <td>desc</td>
  <td>item id</td>
  <td>count</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>task1</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td><td>1</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td></td><td></td><td></td><td></td>
</tr>
<tr align="center">
<td/><td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1001</td><td>1</td><td>1</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td></td><td></td><td></td><td>2</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td></td><td></td><td></td><td>3</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>3</td><td>stage3</td><td>stage desc3</td><td>1,2,3</td><td>1002</td><td>1</td><td>1</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td>1</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td></td><td></td><td></td><td></td>
</tr>
<tr align="center">
<td/><td>2</td>
<td>task2</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td><td></td><td></td><td></td><td></td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td></td><td></td><td></td><td></td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td></td><td></td><td></td><td></td>
</tr>
<tr align="center">
<td/><td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1002</td><td>1</td><td>1</td><td>hello</td><td>5001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td></td><td></td><td></td><td>2,2,2</td><td>1002</td><td>2</td><td></td><td></td><td></td><td></td>
</tr>
</table>

### 类型继承（适合技能、buff相关配置）

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td colspan="4">shape</td><td colspan="4">shape2</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="4">Shape</td><td colspan="4">Shape</td></tr>
<tr align="center"><td>##var</td><td></td><td>$type</td><td>radius</td><td>width</td><td>height</td><td></td><td></td><td></td><td></td></tr>
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

### 多主键表（独立索引）

多个key独立索引。

|##var|key1|key2|key3| num|
|-|-|-|-|-|
|##type|int|long|string|int|
||1|2|aaa|123|
||2|4|bbb|124|
||3|6|ccc|134|
||4|8|ddd|124|
||5|10|eee|898|

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
<td>##var#column</td>
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

以行为树为例，展示json格式下如何配置行为树配置。xml、lua、yaml等等格式请参见 [详细文档](https://focus-creative-games.github.io/luban-doc/#/manual/datasource)。

```json
{
  "id": 10002,
  "name": "random move",
  "desc": "demo behaviour tree",
  "executor": "SERVER",
  "blackboard_id": "demo",
  "root": {
    "$type": "Sequence",
    "id": 1,
    "node_name": "test",
    "desc": "root",
    "services": [],
    "decorators": [
      {
        "$type": "UeLoop",
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
        "$type": "UeWait",
        "id": 30,
        "node_name": "",
        "ignore_restart_self": false,
        "wait_time": 1,
        "random_deviation": 0.5,
        "services": [],
        "decorators": []
      },
      {
        "$type": "MoveToRandomLocation",
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

这儿只简略展示c#、typescript、go、c++ 语言在开发中的用法，更多语言以及更详细的使用范例和代码见[示例项目](https://github.com/focus-creative-games/luban_examples)。

- C# 使用示例

```C#
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes($"{gameConfDir}/{file}.bytes")));
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

- c++ 使用示例

```c++
    cfg::Tables tables;
    if (!tables.load([](ByteBuf& buf, const std::string& s) { buf.clear(); return buf.loadFromFile("../GenerateDatas/bytes/" + s + ".bytes"); }))
    {
        std::cout << "== load fail == " << std::endl;
        return;
    }
    std::cout << tables.TbGlobal->name << std::endl;
    std::cout << tables.TbItem.get(12)->name << std::endl;
```

## 路线图

- [ ] 新增 unity 内置编辑器
- [ ] 新增 unreal 内置编辑器
- [ ] 补充单元测试

## 友情链接

- [Luban Unity插件](https://github.com/LiuOcean/Luban_Unity_GUI) 在Unity里完成生成命令的配置和生成操作
- [LubanTools](https://github.com/MOMOLAXI/LubanTools) Luban的Win GUI命令配置和生成工具

## License

Luban is licensed under the [MIT](LICENSE.TXT) license.
