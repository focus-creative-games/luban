# Luban

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban)  
![Docker Auto](https://img.shields.io/docker/cloud/automated/hugebug4ever/luban.svg)
![Docker Build](https://img.shields.io/docker/cloud/build/hugebug4ever/luban.svg)

![icon](docs/images/icon.png)

- -- --

## links

- [README - English](./README.en-us.md)
- [github link](https://github.com/focus-creative-games/luban)
- [gitee link](https://gitee.com/focus-creative-games/luban)

- -- --

## introduce

In medium and large game projects, excel configuration tables often have more complex data structures. Conventional table guide tools are either unable to support this type of demand, or they force planning and procedures to use tricks such as dismantling tables, which seriously affects the design. And development efficiency. In addition, in games with complex GamePlay, functions such as skills, behavior trees, and levels also have very complex data structures. They are often made with custom editors and saved in file formats such as json, xml, and excel- centric guides. The table tool cannot process these data, which brings trouble to the planning and program workflow.

Luban has the following core advantages compared to the conventional excel guide tool:

- Enhanced the excel format. You can configure excel to configure **arbitrarily complex** data more concisely, such as substructures, structure lists, and more complex deep nested structures can be directly parsed and processed.
- Complete type system and multiple raw data support (excel, xml, json, lua, yaml, unity asset), can easily express and analyze **arbitrarily complex** data. It means that luban can also handle complex configurations such as skills, behavior trees, copies, etc. that traditional excel guide tools cannot handle, completely freeing the program from complex configuration analysis.
- Support multiple export data formats such as binary, **protobuf** (corresponding pb definition can be generated at the same time), **msgpack**, **flatbuffers**, json, lua, etc.
- Complete workflow support. Such as ID foreign key reference check; resource legitimacy check; flexible data source definition (split tables or multiple tables into one); flexible group export mechanism; multiple localization support; extremely fast generation (within 300ms of daily iteration); [Excel2TextDiff](https://github.com/focus-creative-games/Excel2TextDiff) tool to facilitate diff to view differences between versions of excel files;
- **LubanAssistant Excel add- in**. Supports loading configuration data in text formats such as json, lua, xml, etc. into excel, batch editing, and finally saving back to the original file, which can better solve the problem of multi- person collaborative data editing conflicts and mergers in large- scale projects. The configuration made in the editor is difficult to modify in batches in excel.
- Support custom code and data templates. Powerful data expression capabilities make the configuration format of most projects often a subset of luban, so there is a lower project migration cost. After using the template to re- adapt the code and data generation, even if the project has been developed for a long time or is online Benefit from luban's powerful data processing capabilities.
- Universal generation tool. It can also be used to generate codes such as protocols and databases, and can even be used as an object caching service.

## Documentation

- [Quick Start](https://focus-creative-games.github.io/lubandoc/start_up.html)
- [**Document**](https://focus-creative-games.github.io/lubandoc), it is relatively complete, if you have any questions, please check this wiki first.
- **Example Project** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples) )
- [Version Change History](https://focus-creative-games.github.io/lubandoc/changelog.html)

- Support and contact
  - QQ group: 692890842 (Luban development exchange group). If you have any questions about use, please add to the QQ group to ask, and someone can help solve it at any time.
  - Email: taojingjian#gmail.com

## Features

- Supports multiple data formats such as excel, json, xml, lua, yaml, unity asset, etc., which basically unifies the common configuration data of the game
- **Powerful and complete type system**. **Any complex data structure can be expressed elegantly**. Supports all common primitive types, text localized types, datetime types, vector{2,3,4}, container types list, set, map, enumeration and structure, **polymorphic structure** and **nullable type* *.
- Support enhanced excel format. You can fill in arbitrarily complex nested data more concisely in excel.
- The generated code is clear, easy to read, and well modularized. Supports the convention of specifying variable naming style. Specially supports atomic hot update configuration at runtime.
- Flexible data source definition. A table can come from multiple files or multiple tables defined in a file, or all files in a directory or even from cloud tables, and a combination of the above
- Support table and field level grouping. Groups can be flexibly defined, and tables and fields used by the client or server or editor can be selectively exported
- Support the generation of **protobuf**, **msgpack**, **flatbuffers** corresponding definition files and corresponding data files (direct reflection export, efficient, and no need to generate code and then use the generated code to load and export)
- Multiple export data format support. Support binary, json, **protobuf**, **msgpack**, **flatbuffers**, lua, xml, erlang, **xlsx** and custom export data formats
- Support conversion between xlsx and json, lua, xml, yaml and other formats
- Powerful and flexible customization capabilities
  - Support code templates, you can use custom templates to customize the generated code format
  - **Support data template**, you can customize the export format with a template file. It means that you can use Luban as a **configuration processing front end** without changing the existing program code, and generate data in a custom format to work with the configuration loading code of your own project. Long- developed projects or old projects that have been launched can also benefit from luban's powerful data processing workflow
- Support data tags. You can choose to export the data that meets the requirements, and you don’t have to manually comment out those test data when planning to release the official data.
- Powerful data verification capabilities. Support built- in data format check; support ref table reference check (don’t worry about filling in the wrong id for planning); support path resource check (don’t worry about filling in the wrong resource path for planning); support range check
- Support constant alias. Planning no longer has to write specific item IDs for items such as Upgrade Dan
- Support a variety of common data table modes. singleton (singleton table), map (regular key- value table), **list (supports no index, multiple primary key joint index, multiple primary key independent index)**
- Support **external type**, that is, external type. You can reference existing enum and class classes in the configuration, such as UnityEngine.Color and UnityEngine.AudioType.
- Support res resource mark. You can export all resource lists (icon, ui, assetbundle, etc.) referenced in the configuration with one click
- Unify the configuration data of the custom editor. It works well with Unity and UE4 custom editors, and generates suitable c# (Unity) or c++ (UE4) codes for loading and saving json configuration for the editor. The saved json configuration can be recognized and processed by luban.
- Support emmylua anntations. The generated lua contains anntations information conforming to the emmylua format. Cooperate with emmylua, have good configuration code prompt ability
- **Localization Support**
  - Support time localization. The datetime type data will be converted to UTC time in the target area at that time according to the specified timezone, which is convenient for the program to use.
  - Support text static localization. When exporting, all text type data is correctly replaced with the final localized string. Most business functions no longer need to run to find the content of the text based on the localized id, simplifying the programmer's work.
  - Support text dynamic localization. Dynamically switch all text type data to target localized strings during runtime.
  - Support main + patches data merging. On the basic data, differential data is applied to generate the final complete data, which is suitable for the production of configuration data with subtle differences in multiple regions.
  - [TODO] [Original] Support the localization of any granularity and any type of data (such as int, bean, list, map).
- Generate extremely fast. It supports regular local cache incremental generation mode and cloud generation mode. Large projects like MMORPG can also be generated within seconds. The daily incremental generation is basically within 300ms, which greatly saves iteration time in the later stage of the project. In addition, it supports **watch monitoring mode**, and the data directory changes are regenerated immediately.
- **LubanAssistant**, Luban's Excel add- in. It supports loading configuration data in text formats such as json, lua, xml, etc. into excel, batch editing and processing, and finally saving back to the original file, which can better solve the problem of multi- person collaborative data editing conflicts and mergers in large- scale projects. The configuration made in the editor is difficult to modify in batches in excel.
- Excel2TextDiff. Convert excel to text and then diff to clearly compare the content changes between excel versions.
- Support mainstream game development languages
  - c++ (11+)
  - c# (.net framework 4+. dotnet core 3+)
  - java (1.6+)
  - go (1.10+)
  - lua (5.1+)
  - js and typescript (3.0+)
  - python (3.0+)
  - erlang (18+)
  - rust (1.5+)
  - Other languages ​​supported by protobuf, msgpack, flatbuffers
- Support mainstream engines and platforms
  - unity + c#
  - unity + [tolua](https://github.com/topameng/tolua), [xlua](https://github.com/Tencent/xLua)
  - unity + [ILRuntime](https://github.com/Ourpalm/ILRuntime)
  - unity + [puerts](https://github.com/Tencent/puerts)
  - unity + [GameFramework](https://github.com/EllanJiang/GameFramework)
  - unity + [ET game framework](https://github.com/egametang/ET)
  - unreal + c++
  - unreal + [unlua](https://github.com/Tencent/UnLua)
  - unreal + [sluaunreal](https://github.com/Tencent/sluaunreal)
  - unreal + [puerts](https://github.com/Tencent/puerts)
  - cocos2d- x + lua
  - cocos2d- x + js
  - [skynet](https://github.com/cloudwu/skynet)
  - WeChat Mini Program Platform
  - Other js- based small program platforms
  - All other engines and platforms that support lua
  - All other engines and platforms that support js

## Enhanced excel format

Luban supports the parsing of arbitrarily complex data structures in excel, even as complex as skills and behavior trees (but in practice, editors are generally used to make these data and save them in json format instead of filling them in excel). The following shows how to configure these data in luban from simple to complex.

### Native data type

Supports bool, int, float, string, text, datetime, vector2, vector3, vector4 and other types, and their filling is consistent with conventional recognition.

|##var| x1 | x3 | x4 | x5 | x6 | x7 | s1 | s2&sep=# | v2 | v3 | v4 | t1 |
| - |- |- |- |- |- |- |- |- |- |- |- |- |
|##type|bool|short|int|long|float|double|string|text |vector2|vector3|vector4|datetime|
|##|desc1|id|desc4|desc5|desc6|desc7|desc1|desc2 |desc2|desc3|desc4|desc1|
|| false| 10| 100| 1000| 1.23| 1.2345|hello |key1#world1|1,2|1,2,3|1,2,3,4|1999- 10- 10 11:12:13|
|| true | 20| 200| 1000| 1.23| 1.2345|world |key2#world2|1,2|1,2,3|1,2,3,4|1999- 10- 12 11:12:13|

### Native data list

Both array and list types can represent lists. The difference between them is that the code generated by array is an array, while the code generated by list is a list. For example, the c# code type generated by "array,int" is int[], and the c# code type generated by "list,int" is List&lt;int&gt;.

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>arr1</td>
  <td colspan="4">arr2</td>
  <td>arr3&sep=|</td>
  <td colspan="3">arr4</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</id>
  <td>array,int</td>
  <td colspan="4">list,int</td>
  <td>list,string</td>
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
<td>1,2,3</td>
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
<td>2|4|6</td>
<td>3</td><td>4</td><td>5</td><td>6</td>
<td>aaaa|bbbb|cccc</td>
<td>aaa</td><td>bbb</td><td>ccc</td>
</tr>

</table>

### Enumeration

Fill in the enumeration value in the form of enumeration name or alias or value.

Defined in xml

```xml
<enum name="ItemQuality">
 <var name="WHITE" alias="白" value="0"/>
 <var name="GREEN" alias="green" value="1"/>
 <var name="RED" alias="Red" value="2"/>
</enum>
```

Or define in \_\_enums\_\_.xlsx

<table border="1">
<tr align="center"><td>##var</td><td>full_name</td><td>flags</td><td>unique</td><td>comment</td> <td>tags</td><td colspan="5">*items</td></tr>
<tr align="center"><td>##var</td><td></td><td></td><td></td><td></td><td></td><td>name</td><td>alias</td><td>value</td><td>comment</td><td>tags</td></tr>
<tr align="center"><td/><td>ItemQuality</td><td>false</td><td>true</td><td/><td/><td>WHITE</td><td>White</td><td>0</td><td/><td/></tr>
<tr align="center"><td/><td></td><td></td><td></td><td/><td/><td>GREEN</td><td>Green</td><td>1</td><td/><td/></tr>
<tr align="center"><td/><td></td><td></td><td></td><td/><td/><td>RED</td><td>Red</td><td>2</td><td/><td/></tr>

</table>

The data table is as follows

|##var|id| quality| quality2 |
| - |- |- |- |
|##type|int|ItemQuality|ItemQuality|
| | 1| White | RED |
| | 2| GREEN | Red |
| | 3| RED | WHITE |
| | 4| 1 | 0 |

### Nested substructure

It is often encountered that a certain field is a structure, especially this structure will be reused in many configurations.

Suppose the task contains a field of reward information

Defined in xml

```xml
<bean name="Reward">
 <var name="item_id" type="int"/>
 <var name="count" type="int"/>
 <var name="desc" type="string">
</bean>
```

Or define in \_\_beans__.xlsx

<table border="1">
<tr align="center"><td>##var</td><td>full_name</td><td >sep</td><td>comment</td><td colspan="5"> fields</td> </tr>
<tr align="center"><td>##var</td><td></td><td/><td/><td>name</td><td>type</td><td>group</td><td>comment</td><td>tags</td></tr>
<tr><td></td><td>Reward</td><td/><td/><td>item_id</td><td>int</td><td></td><td>Item id</td><td/></tr>
<tr><td></td><td></td><td/><td/><td>count</td><td>int</td><td></td><td >Number</td><td/></tr>
<tr><td></td><td></td><td/><td/><td>desc</td><td>string</td><td></td><td >Description</td><td/></tr>
</table>

The data table is as follows

<table border="1">
<tr align="center">
<td>##var</td>
<td>id</td>
<td colspan="3">reward</td>
</tr>
<tr align="center">
<td>##type</td>
<td>int</td>
<td colspan="3">Reward</td>
</tr>
<tr align="center">
<td>##</td>
<td>id</td>
<td>Item id</td><td>Number</td><td>Description</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>1001</td><td>1</td><td>desc1</td>
</tr>
<tr align="center">
<td/>
<td>2</td>
<td>1002</td><td>100</td><td>desc2</td>
</tr>
</table>

### Simple structure list

It is also very common that a certain field is a structured list. For example, the reward information list contains multiple reward information, and each reward has multiple fields.

Assume that the gift package contains a list of item information. Support 3 types of filling modes, the specific choice is determined by planning flexibly.

- All fields are fully expanded, and each cell is filled with one element. The disadvantage is that it takes up more columns. Such as the items1 field.
- Each structure occupies a cell, use sep to divide structure subfields. Such as the items2 field.
- The entire list occupies one cell, use sep to split the list and structure subfields. Such as the items3 field.

The xml is defined as follows

```xml
<bean name="Reward">
 <var name="item_id" type="int"/>
 <var name="count" type="int"/>
 <var name="desc" type="string">
</bean>
```

Or it can be defined in \_\_beans\_\_.xlsx, which will not be repeated here. ==**The following examples involving structure definitions are only examples of xml**==.

The data table is as follows:

<table border="1">
<tr align="center">
<td>##var</td>
<td>id</td>
<td colspan="6">rewards1</td>
<td colspan="3">rewards2&sep=,</td>
<td>rewards3&sep=,|</td>
</tr>
<tr align="center">
<td>##type</td>
<td>int</td>
<td colspan="6">list,Reward</td>
<td colspan="3">list,Reward</td>
<td>list,Reward</td>
</tr>
<tr align="center">
<td>##</td>
<td>id</td>
<td colspan="6">reward list desc1</td>
<td colspan="3">reward list desc2</td>
<td>reward list desc3</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>1001</td><td>1</td><td>desc1</td><td>1002</td><td>2</td><td>desc2</td>
<td>1001,1,desc1</td><td>1002,2,desc2</td><td>1003,3,desc3</td>
<td>1001,1,desc1|1002,2,desc2</td>
</tr>

<tr align="center">
<td/>
<td>2</td>
<td>1001</td><td>1</td><td>desc1</td><td></td><td></td><td></td>
<td>1001,1,desc1</td><td>1002,2,desc2</td><td></td>
<td>1001,1,desc1|1002,2,desc2|1003,1,desc3</td>
</tr>

</table>

Or you can use a multi- level header to limit each element individually

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

<tr align="center"><td/><td>1</td><td>task1</td><td>1001</td><td>10</td><td>desc1</td><td>1002</td><td>12</td><td>desc2</td><td>1003</td><td>13</td><td>desc3</td> </tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>1003</td><td>30</td><td>desc3</td><td>1004</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>1005</td><td>50</td><td>desc5</td><td/><td/><td/><td/><td/><td/></tr>
</table>

### Multi- line structure list

Sometimes there are many fields in each structure of the list structure. If it is expanded horizontally, it occupies too many columns, which is not convenient for editing. If the table is split, it is inconvenient to program or plan. At this time, you can use the multi- line mode. Support any level of multi- row structure list (that is, each element in the multi- row structure can also be multiple rows), name&multi_rows=1 or *name can express a multi- row parsed field.

Assuming that each task contains multiple stages, there is a stage list field.

```xml
<bean name="Stage">
 <var name="id" type="int"/>
 <var name="name" type="string"/>
 <var name="desc" type="string"/>
 <var name="location" type="vector3"/>
 <var name="reward_item_id" type="int"/>
 <var name="reward_item_count" type="int"/>
</bean>
```

<table border="1">
<tr align="center">
<td>##var</td>
<td>id</td>
<td>name</td>
<td colspan="6">*stage2</td>
</tr>
<tr align="center">
<td>##type</td>
<td>int</td>
<td>string</td>
<td colspan="6">list,Stage</td>
</tr>
<tr align="center">
<td>##</td>
<td>id</td>
<td>desc</td>
<td colspan="6">stage info</td>
</tr>
<tr align="center">
<td/>
<td>1</td>
<td>task1</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/>
<td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>3</td><td>stage3</td><td>stage desc3</td><td>1,2,3</td><td>1002</td><td>1</td>
</tr>
<tr align="center">
<td/>
<td>2</td>
<td>task2</td>
<td>1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>
<tr align="center">
<td/><td/><td/><td>2</td><td>stage2</td><td>stage desc2</td><td>1,2,3</td><td>1002</td><td>1</td>
</tr>
</table>

### List table (no primary key)

Sometimes I just want to get a list of records without a primary key. mode="list" and index is empty, indicating that there is no primary key table.

Definition table

```xml
<table name="TbNotKeyList" value="NotKeyList" mode="list" input="not_key_list.xlsx"/>
```

Sample data sheet

|##var|x|y|z| num|
|- |- |- |- |- |
|##type|int|long|string|int|
||1|1|aaa|123|
||1|1|bbb|124|
||1|2|aaa|134|
||2|1|aaa|124|
||5|6|xxx|898|

### Multi- primary key table (joint index)

Multiple keys form a joint unique primary key. Use "+" to split the key to indicate the union relationship.

Definition table

```xml
<table name="TbUnionMultiKey" value="UnionMultiKey" index="key1+key2+key3" input="union_multi_key.xlsx"/>
```

Sample data sheet

|##var|key1|key2|key3| num|
|- |- |- |- |- |
|##type|int|long|string|int|
||1|1|aaa|123|
||1|1|bbb|124|
||1|2|aaa|134|
||2|1|aaa|124|
||5|6|xxx|898|

### Multi- primary key table (independent index)

Multiple keys, each independently and uniquely indexed. The difference with the joint index is the use of "," to divide the key, indicating an independent relationship.

Definition table

```xml
<table name="TbMultiKey" value="MultiKey" index="key1,key2,key3" input="multi_key.xlsx"/>
```

Sample data sheet

|##var|key1|key2|key3| num|
|- |- |- |- |- |
|##type|int|long|string|int|
||1|2|aaa|123|
||2|4|bbb|124|
||3|6|ccc|134|
||4|8|ddd|124|
||5|1|eee|898|

### Singleton table

Some configurations only have one copy globally, such as the opening level of the guild module, the initial size of the backpack, and the upper limit of the backpack. At this time, it is more appropriate to use a singleton table to configure these data.

|##var| guld_open_level | bag_init_capacity | bag_max_capacity | newbie_tasks |
|- |- |- |- |- |
| ##type | int | int | int | list,int|
| ## |desc1 | desc 2 | desc 3 | desc 4 |
| | 10 | 100| 500| 10001,10002 |

### Vertical table

Most tables are horizontal, that is, one record per row. Some tables, such as singleton tables, are more comfortable if they are filled vertically, with one field per row. A1 is ##column which means the vertical table mode is used. The above singleton table is filled in as follows in vertical table mode.

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

### Reference Check

The game configuration often needs to fill in foreign key data such as item id. These data must be legal id values. Luban supports checking the legitimacy of the id when it is generated. If there is an error, a warning will be issued. Not only the top- level fields of the table, but the sub- fields of the list and nested structure also support complete reference checking.

```xml
<bean name="Reward">
 <var name="item_id" type="int" ref="item.TbItem"/>
 <var name="count" type="int"/>
 <var name="desc" type="string">
</bean>
```

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td >item_id</td>
  <td>items</td>
  <td colspan="3">reward</td>
  <td colspan="3">rewards&sep=,</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td>int</td>
  <td>int&ref=item.TbItem</td>
  <td colspan="3">list,int&ref=item.TbItem</td>
  <td colspan="3">reward</td>
</tr>
<tr align="center">
  <td>##</td>
  <td>id</td>
  <td>desc1</td>
  <td>desc2</td>
  <td colspan="3">desc3</td>
  <td colspan="3">desc4</td>
</tr>
<tr align="center">
  <td/>
  <td>1</td>
  <td>1001</td>
  <td>1001,1002</td>
  <td>1001</td><td>10</td><td>1001</td>
  <td>1001,10,1001</td><td>1002,2,1002</td><td/>
</tr>
<tr align="center">
  <td/>
  <td>2</td>
  <td>1002</td>
  <td>1003,1004,1005</td>
  <td>1002</td><td>10</td><td>1002</td>
  <td>1004,10,item4</td><td>1005,2,item5</td><td>1010,1,10010</td>
</tr>
</table>

### Resource Check

The resource path is often filled in in the configuration, such as the resource of the prop icon. These data are all string types, which are very easy to fill in errors and cause the normal display during runtime. Luban supports the legitimacy check of unity and ue4 resources and the general file path check. Not only the top- level fields of the table, but the sub- fields of the list and nested structure also support complete reference checking.

Add the attribute path=unity or path=ue or path=normal;xxxx to these fields.

|##var| id | icon |
|- |- |- |
| ##type| int | string&path=unity|
| ##|id | icon desc |
| | 1| Assets/UI/1001.jpg|
| | 2| Assets/UI/1002.jpg|

### Group export

Flexible grouping definition, not just client and server grouping. The following grouping granularity is supported:

- Table level grouping
- Field level grouping (any bean field granularity, not limited to top- level fields)

### Data label filtering

During the development period, some configurations that are only used for development are often made, such as test props, such as configurations used for automated testing. It is hoped that these data will not be exported during the official release.

|##var| id | name | |
|- |- |- |- |
| ##type | int | string | |
| ## | id | desc1| Comments |
| | 1 | 1001 | Export forever |
|##| 2 | 1002 | Never export |
|test| 4 | item4 | - -export_exclude_tags do not export when test |
|TEST| 5 | item5 | - -export_exclude_tags do not export when test |
|dev |6 | item6 | - -export_exclude_tags don't export when dev |
| | 7|item7| Export forever |

## Advanced features

### Hierarchy title (hierarchy title)

In multi- line data or deeply nested data, if there are many data fields, it is not easy to distinguish the molecular elements when filling in. Luban provides hierarchical headings to achieve deep- level sub- field correspondence. Take the above multi- row data list as an example, the first column is ##var, which means this is a subfield row.

- Subheading of common bean structure

<table border="1">

<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="5">stage</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td>string</td>
  <td colspan="5">Stage</td>
</tr>
<tr align="center">
  <td>##var</td>
  <td/>
  <td/>
  <td>name</td>
  <td>desc</td>
  <td>location</td>
  <td>item_id</td>
  <td>num</td>
</tr>
<tr align="center">
  <td>##</td>
  <td>id</td>
  <td>name</td>
  <td>desc2</td>
  <td>desc3</td>
  <td>desc4</td>
  <td>desc5</td>
  <td>desc6</td>
</tr>

<tr align="center">
<td/>
<td>1</td><td>task1</td><td>stage1</td><td>stage desc1</td><td>1,2,3</td><td>1001</td><td>1</td>
</tr>

<tr align="center">
<td/>
<td>2</td><td>task2</td><td>stage2</td><td>stage desc2</td><td>3,4,5</td><td>2001</td><td>3</td>
</tr>

</table>

- Multi- line expansion of multi- level sub- headings of list and bean

<table border="1">

<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td>name</td>
  <td colspan="6">stages</td>
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
  <td>##type</td>
  <td>int</td>
  <td>string</td>
  <td colspan="6">list,Stage</td>
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

- Horizontally expand multi- level sub- headings of list and bean

<table border="1">
<tr align="center"><td>##var</td><td>id</td><td>name</td><td colspan="9">items</td></tr >
<tr align="center"><td>##type</td><td>int</td><td>string</td><td colspan="9">list,Item</td></tr>

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

<tr align="center"><td/><td>1</td><td>task1</td><td>1</td><td>10</td><td>desc1</td><td>2</td><td>12</td><td>desc2</td><td>3</td><td>13</td><td>desc3</td> </tr>
<tr align="center"><td/><td>2</td><td>task1</td><td>3</td><td>30</td><td>desc3</td><td>4</td><td>40</td><td>desc4</td><td/><td/><td/></tr>
<tr align="center"><td/><td>3</td><td>task1</td><td>5</td><td>50</td><td>desc5</td><td/><td/><td/><td/><td/><td/></tr>
</table>

- Multi- level subtitles of map type

<table border="1">

<tr align="center"><td>##var</td><td>id</td><td colspan="4">lans</td></tr>
<tr align="center"><td>##type</td><td>int</td><td colspan="4">map,string,string</td></tr>
<tr align="center"><td>##var</td><td/><td>ch- zn</td><td>en</td><td>jp</td><td >fr</td></tr>
<tr align="center"><td/><td>1</td><td>Apple</td><td>apple</td><td>aaa</td><td>aaa</td></tr>
<tr align="center"><td/><td>2</td><td>banana</td><td>banana</td><td>bbb</td><td>bbb</td></tr>

</table>

### Nullable data type

There are often semantic requirements for null values ​​in configuration data. In actual projects, 0 or - 1 are often used mixedly to express null values, which is neither natural, clear nor uniform. Luban draws on the concept of nullable variables in c#, and specifically provides support for nullable data. All native data types, as well as enum, bean, and polymorphic bean types have corresponding nullable data types. The definition method is <type name>?, which is the same as the definition method of the Nullable type in c#. For example, bool?, int?, long?, double?, EColor?, DemoType?

|##var|id|x1|x2|x3|x4|x5|
|- |- |- |- |- |- |- |
|##type|int|bool?|int?|float?|datetime?|QualityType?|
|##|id|desc1|desc2|desc3|desc4|desc5|
||1|true|1|1.2|1999- 09- 09 10:10:10| RED|
||2|null|null|null|null|null|
||3| |||||

### Type inheritance (inheritance)

It supports OOP's type inheritance system, which facilitates the expression of multiple types of data, and is often used in modules such as skills and AI. Type inheritance is the soul of the Luban type system. Without type inheritance, it is impossible to express arbitrarily complex data structures concisely, and therefore it is impossible to define and read complex configuration data from the configuration.

In practice, data such as skills and AI are generally produced with an editor and saved in a format such as json instead of editing in excel.

```xml
<bean name="Shape">
 <bean name="Circle">
  <var name="radius" type="float"/>
 </bean>
 <bean name="Rectangle" alias="rectangle">
  <var name="width" type="float"/>
  <var name="height" type="float"/>
 </bean>
 <bean name="Curve">
  <bean name="Line" alias="线">
   <var name="param_a" type="float"/>
   <var name="param_b" type="float"/>
  </bean>
  <bean name="Parabola" alias="Parabola">
   <var name="param_a" type="float"/>
   <var name="param_b" type="float"/>
  </bean>
 </bean>
</bean>

```

<table border="1">
<tr align="center">
  <td>##var</td>
  <td>id</td>
  <td colspan="4">shapes&sep=,</td>
</tr>
<tr align="center">
  <td>##type</td>
  <td>int</td>
  <td colspan="4">list,Shape</td>
</tr>
<tr align="center">
  <td>##</td>
  <td>id</td>
  <td colspan="4"> shape desc</td>
</tr>
<tr align="center">
  <td/>
  <td>1</td>
  <td>Circle,10</td>
  <td>Rectangle,100,200</td>
  <td/>
  <td/>
</tr>
<tr align="center">
  <td/>
  <td>2</td>
  <td>Circle,20</td>
  <td>Rectangle,100,200</td>
  <td>Line,5,8</td>
  <td>Parabola,15,30</td>
</tr>
</table>

### Field default value

We hope that when the cell in excel is left blank, the field takes the specified value instead of the default false, 0 or the like. Specify the default value by defining the default=xxx attribute of the field.

For example, for the record with id=2, x1 and x2 are both empty, x1=0, x2=- 1.

|##var|id | x1 | x2&default=- 1|
|- |- |- |- |
|##type| int | int | int |
|##|id|desc1|desc2|
||1 | 10 |20|
||2| | |
||3| | 30|

### Constant alias

There are often some commonly used values ​​similar to enumerations in the game, such as the id of the upgrade Dan, which must be filled in many places. If the id of the item is directly used, it is neither intuitive nor easy to make mistakes. Luban supports constant substitution. For example, SHENG_JI_DAN will be replaced with 11220304 when exporting.

``` xml
<enum name="EFunctionItemId">
 <var name="SHENG_JI_DAN" alias="Upgrade Dan" value="11220304"/>
 <var name="JIN_JIE_DAN" alias="Advanced Dan" value="11220506"/>
</enum>
```

|##var|id| item_id |
|- |- |- |
|##type|int| int&convert=EFunctionItemId|
|##|id| desc|
||1 | SHENG_JI_DAN|
||2| Advanced Dan|
||3| 1001|

### Flexible configuration file organization

The following organizational forms are supported, allowing developers to flexibly organize the configuration file structure according to the situation. For example, one table can correspond to one xlsx file; multiple tables can be placed in the same xlsx file; one table can correspond to multiple xlsx files; one table can correspond to one directory.

- All cell sheets from a certain excel file
- A specified cell book from an excel file
- From json, xml, lua, yaml files
- From json, xml, lua, yaml subfields (e.g. root.a.b)
- From all files under the directory tree, each file corresponds to a record
- Any combination of the above

### Other data sources

- [json](docs/data_json.md)
- [lua](docs/data_lua.md)
- [xml](docs/data_xml.md)
- [yaml](docs/data_yaml.md)

### Multiple export data formats

Support the following export data formats

- binary
- protobuf (binary, json)
- msgpack (binary)
- flatbuffers (json)
- json
- lua
- erlang
- Use templates to customize the generated data format (only text data format is supported)

The binary format loads the fastest, json loads the second, and lua loads the slowest.

The binary format occupies the smallest space, followed by lua, and json the largest.

Different export types only affect the size of the exported data and the performance of the loaded data, and do not affect the structure definition and the final load to the memory usage.

**Different export data types are transparent to the program and planning, and switching does not affect the data editing mode and the configuration mode used in the business code. **

### Editor support

Support the generation of c# (for unity) and c++ (for UE4) json configuration loading and saving code, which is convenient for students who make editors to load and save data that conforms to the luban configuration format.

### Custom code and data templates

[Custom Template](https://focus-creative-games.github.io/lubandoc/render_template.html)

### Localization

The following localization mechanisms are supported, see [Localization](https://focus-creative-games.github.io/lubandoc/l10n.html)

- Static localization
- Dynamic localization
- Multi- branch data
- Time localization
- [TODO] Data localization at any granularity (not just text and record level)

## Excel2TextDiff

Convert the excel file to text, and then call the diff tool for comparison. It works well with version management tools such as TortoiseGit and TortoiseSvn.

![pipeline](docs/images/examples/d_70.jpg)

## [LubanAssistant](https://github.com/focus-creative-games/luban_examples/tree/main/Tools/LubanAssistant) Excel plugin. Artifact

For the configuration table edited by multiple people, how to save the configuration in the xlsx format is prone to data conflict coverage, which is especially serious in large projects. In addition, when merging multi- branch data, xlsx cannot automatically resolve conflicts like text files, which brings a lot of trouble to the version maintenance of the project.

A reasonable solution is to save the configuration data in json, xml format and edit it in excel. LubanAssistant solves this problem well. Users not only enjoy Luban's powerful data processing capabilities, but also have good json readability and multi- version maintainability, as well as the convenient editing capabilities of excel.

![pipeline](docs/images/examples/e_10.jpg)

The corresponding content of the record with id 1 is as follows

```json
{
  "id":1,
  "x":5,
  "items":[
    {"x":1, "y":true, "z":"abcd", "a":{"x":10, "y":100}, "b":[1,3,5] },
    {"x":2, "y":false, "z":"abcd", "a":{"x":22, "y":33}, "b":[4,5]}
  ]
}
```

## Code preview

Here is only a brief display of the usage of c#, typescript, and go languages ​​in development. For more languages ​​and more detailed usage examples and codes, please see [Sample Project](https://github.com/focus-creative-games/luban_examples).

- C# usage example

```C#
// One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes(gameConfDir + "/" + file + ".bin")));
// Access a singleton table
Console.WriteLine(tables.TbGlobal.Name);
// Access ordinary key- value table
Console.WriteLine(tables.TbItem.Get(12).Name);
// Support operator [] usage
Console.WriteLine(tables.TbMail[1001].Desc);
```

- Typescript usage example

```typescript
// One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))
// Access a singleton table
console.log(tables.TbGlobal.name)
// Access ordinary key- value table
console.log(tables.TbItem.get(12).Name)
```

- go use example

```go
// One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
if tables, err := cfg.NewTables(loader); err != nil {
 println(err.Error())
 return
}
// Access a singleton table
println(tables.TbGlobal.Name)
// Access ordinary key- value table
println(tables.TbItem.Get(12).Name)

```

## Performance test data

hardware:

 Intel(R) Core i7- 10700 @ 2.9G 16 core
 32G RAM

data set

 500 excel sheets
 Each table has 1000 rows of relatively large records
 The file size of each table is 132k

Test Results:

| Format | Time- consuming full generation | Time- consuming incremental generation | Single output file size | Total output file size |
| -| -|-| - | - |
| bin | 15.652 s| 797 ms | 164 K | 59.5 M |
| json | 17.746 s| 796 ms | 1.11 M | 555 M |
| lua | 17.323 s| 739 ms | 433 K | 212 M |

## route map

- [ ] Added unity built- in editor
- [ ] Added unreal built- in editor
- [ ] Supplementary unit test

## Development environment setup

- Install [VS2022 Community Edition](https://visualstudio.microsoft.com/zh- hans/vs/)
- Install [.dotnet core sdk 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

## Installation and use

See [client&server installation and usage instructions](docs/luban_command_tools.md)

## How to contribute

- [Contributing](CONTRIBUTING.md) explains what kinds of changes we welcome
- [Workflow Instructions](docs/workflow/README.md) explains how to build and test

## Useful links

- [.NET Core source index](https://source.dot.net)

## License

Luban is licensed under the [MIT](LICENSE.TXT) license.