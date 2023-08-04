# Luban

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban )
![Docker Auto](https://img.shields.io/docker/cloud/automated/hugebug4ever/luban.svg)
![Docker Build](https://img.shields.io/docker/cloud/build/hugebug4ever/luban.svg)

![icon](docs/images/icon.png)

-----

## links

- [README - English](./README.en-us.md)
- [github link](https://github.com/focus-creative-games/luban)
- [gitee link](https://gitee.com/focus-creative-games/luban)

-----

## introduce

luban is your **best game configuration solution**.

Luban efficiently processes data such as excel, json, and xml common in game development, checks data errors, generates codes in various languages such as c#, and exports them into various formats such as bytes or json.

luban unifies the game configuration development workflow, greatly improving the efficiency of planning and programming.

## Core features

- Powerful data analysis and conversion capabilities {excel(csv,xls,xlsx), json, bson, xml, yaml, lua, unity ScriptableObject} => {binary, json, bson, xml, lua, yaml, erlang, custom format }
- Enhanced excel format, which can succinctly configure simple lists, substructures, structured lists, and arbitrarily complex deep nested structures.
- Complete type system, **Support OOP type inheritance**, with data in excel, json, lua, xml and other formats **Flexible and elegant** Express complex GamePlay data such as behavior trees, skills, plots, and copies
- Supports generating c#, java, go, c++, lua, python, javascript, typescript, erlang, rust, gdscript code
- Support generating protobuf(schema + binary + json), flatbuffers(schema + json), msgpack(binary)
- Powerful data verification capability. ref reference check, path resource path, range range check, etc.
- Perfect localization support. Static text value localization, dynamic text value localization, time localization, main-patch multi-region version
- Powerful and flexible customization capabilities, support for custom code templates and data templates
- **Universal generation and caching tool**. It can also be used to generate code such as protocols, databases, and even as an object caching service
- **Good support for mainstream engines, all platforms, mainstream hot update solutions, and mainstream front-end and back-end frameworks**. Supports mainstream engines such as Unity, Unreal, Cocos2x, Godot, and WeChat games. The tool itself is cross-platform and can work well on Win, Linux, and Mac platforms.

See [feature](https://focus-creative-games.github.io/luban-doc/#/manual/traits) for complete features

## Documentation

- [Official Documentation](https://focus-creative-games.github.io/luban-doc/)
- [Quickstart](https://focus-creative-games.github.io/luban-doc/#/beginner/quickstart)
- **Example Project** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples) )
- [Version Change Log](https://focus-creative-games.github.io/luban-doc/#/changelog)
- Support and contact
   - QQ group: 692890842 (Luban development exchange group). If you have any questions about usage, please join the QQ group to ask in time, and someone will help you solve it at any time.
   - Email: luban@code-philosophy.com

## Excel format overview

For a complete example, please refer to [Excel Format Introduction](https://focus-creative-games.github.io/lubandoc/excel.html)

### Normal table

|##var| id | x1 | x5 | x6 | s1 | s2 | v3 | t1 |
| -|- | -| -| -| -| - | - | - |
|##type|int|bool|long|float|string|text#sep=\||vector3|datetime|
|##|id|desc1|desc2|desc3|desc4|desc7|desc1|time|
|| 1|false| 1000| 1.2| hello |key1\|world1|1,2,3|1999-10-10 11:12:13|
|| 2|true| 1000| 2.4|world |key2\|world2|2,4,5|1999-10-12 11:12:13|

### Raw data list

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

### substructure

Reward is a substructure containing the three fields of "int item_id; int count; string desc;".

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td colspan="3">reward</td><td colspan="3">reward2</ td><td>reward3</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="3">Reward</td><td colspan="3">Reward</ td><td>Reward#sep=,</td></tr>
<tr align="center"><td>##var</td><td></td><td>item_id</td><td>count</td><td>desc</td><td></td><td></td><td></td><td/></tr>
<tr align="center"><td/><td>1</td><td>1001</td><td>10</td><td>item 1</td><td>1002< /td><td>11</td><td>item 2</td><td>1002,1,item 3</td></tr>
<tr align="center"><td/><td>2</td><td>2001</td><td>10</td><td>item 2</td><td>2002< /td><td>20</td><td>item 4</td><td>2003,2,item 5</td></tr>
</table>

### Structure List 1

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

### Structure List 2

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
<tr align="center"><td/><td>1</td><td>task1</td><td>1001</td><td>10</td><td>desc1</ td><td>1002</td><td>12</td><td>desc2</td><td>1003</td><td>13</td><td>desc3</td> </tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>1003</td><td>30</td><td>desc3</ td><td>1004</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>1005</td><td>50</td><td>desc5</ td><td/><td/><td/><td/><td/><td/></tr>
</table>

### Structure List 3

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
<tr align="center"><td/><td>1</td><td>task1</td><td>1001</td><td>10</td><td>desc1</ td><td>1002</td><td>12</td><td>desc2</td><td>1003</td><td>13</td><td>desc3</td> </tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>1003</td><td>30</td><td>desc3</ td><td>1004</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>1005</td><td>50</td><td>desc5</ td><td/><td/><td/><td/><td/><td/></tr>
</table>

### Multi-row table

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

### Multi-level subheadings of type map

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td colspan="4">lans</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="4">map,string,string</td></tr>
<tr align="center"><td>##var</td><td/><td>ch-zn</td><td>en</td><td>jp</td><td >fr</td></tr>
<tr align="center"><td/><td>1</td><td>apple</td><td>apple</td><td>aaa</td><td>aaa</td></tr>
<tr align="center"><td/><td>2</td><td>banana</td><td>banana</td><td>bbb</td><td>bbb</td></tr>
</table>

### Type inheritance (suitable for skills, buff related configuration)

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td colspan="4">shape</td><td colspan="4">shape2</ td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="4">Shape</td><td colspan="4">Shape</ td></tr>
<tr align="center"><td>##var</td><td></td><td>$type</td><td>radius</td><td>width</td><td>height</td><td></td><td></td><td></td><td></td></tr>
<tr align="center"><td/><td>1</td><td>Circle</td><td>10</td><td/><td/><td>Circle</td><td>100</td><td></td><td></td></tr>
<tr align="center"><td/><td>2</td><td>Rectangle</td><td></td><td>10</td><td>20</td ><td>Rectangle</td><td>10</td><td>20</td><td></td></tr>
<tr align="center"><td/><td>3</td><td>Circle</td><td>10</td><td/><td/><td>Triangle</td><td>15</td><td>15</td><td>15</td></tr>
<tr align="center"><td/><td>4</td><td>Circle</td><td>10</td><td/><td/><td>Rectangle</td><td>30</td><td>20</td><td></td></tr>
</table>

### Multiple primary key table (joint index)

Multiple keys form a joint unique primary key.

|##var|key1|key2|key3| num|
|-|-|-|-|-|
|##type|int|long|string|int|
||1|1|aaa|123|
||1|1|bbb|124|
||1|2|aaa|134|
||2|1|aaa|124|
||5|6|xxx|898|

### Multiple primary key table (independent index)

Multiple keys are indexed independently.

|##var|key1|key2|key3| num|
|-|-|-|-|-|
|##type|int|long|string|int|
||1|2|aaa|123|
||2|4|bbb|124|
||3|6|ccc|134|
||4|8|ddd|124|
||5|10|eee|898|

### Singleton table

Some configurations only have one copy globally, such as the opening level of the guild module, the initial size of the backpack, and the upper limit of the backpack. In this case, it is more appropriate to use a singleton table to configure these data.

|##var| guild_open_level | bag_init_capacity | bag_max_capacity | newbie_tasks |
| - |- | - | - | - |
| ##type | int | int | int | list,int|
| ## |desc1 |desc2 |desc3 |desc4|
| | 10 | 100 | 500 | 10001, 10002 |

### Vertical table

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

## json, lua, xml, yaml format quick overview

Take the behavior tree as an example to show how to configure the behavior tree configuration in json format. For formats such as xml, lua, yaml, etc., please refer to [detailed documentation](https://focus-creative-games.github.io/luban/data_source/).


````json
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
````

## Code usage preview

Here we only briefly show the usage of c#, typescript, and go languages ​​in development. For more languages ​​and more detailed usage examples and codes, see [Example Project](https://github.com/focus-creative-games/luban_examples).

- C# usage example

````C#
// One line of code can load all configuration. cfg.Tables contains one instance field for all tables.
var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes(gameConfDir + "/" + file + ".bytes")));
// access a singleton table
Console.WriteLine(tables.TbGlobal.Name);
// access the normal key-value table
Console.WriteLine(tables.TbItem.Get(12).Name);
// support operator [] usage
Console.WriteLine(tables.TbMail[1001].Desc);
````

- Typescript usage example

```typescript
// One line of code can load all configuration. cfg.Tables contains one instance field for all tables.
let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))
// access a singleton table
console.log(tables.TbGlobal.name)
// access the normal key-value table
console.log(tables.TbItem.get(12).Name)
````

- go example

````go
// One line of code can load all configuration. cfg.Tables contains one instance field for all tables.
if tables , err := cfg.NewTables(loader) ; err != nil {
 println(err.Error())
 return
}
// access a singleton table
println(tables.TbGlobal.Name)
// access the normal key-value table
println(tables.TbItem.Get(12).Name)

````

## route map

- [ ] Added unity built-in editor
- [ ] Added unreal built-in editor
- [ ] Supplemental unit tests

## License

Luban is licensed under the [MIT](LICENSE.TXT) license.