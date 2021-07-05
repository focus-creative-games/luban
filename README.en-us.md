# Luban

[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban )
![Docker Auto](https://img.shields.io/docker/cloud/automated/hugebug4ever/luban.svg)
![Docker Build](https://img.shields.io/docker/cloud/build/hugebug4ever/luban.svg)

![icon](docs/images/icon.png)

## Introduction

Luban is a general-purpose object generation and caching solution. On this basis, a **game configuration solution** that is **complete, powerful, flexible and easy to use** is implemented.

Luban was originally designed for super-large projects such as seamless open world MMORPG. It is good at handling large and complex configuration data and structures. It is also suitable for use in card, turn-based, ARPG and other light and medium games.

Based on the design of **meta definition + data source**, luban implements a **complete type system**, enhances the excel format, and provides support for multiple data sources such as json, xml, lua, and unified data definition and loading The game configuration pipeline of, inspection, data export and code generation completely solves the problem that it is difficult to configure complex data in excel in medium and large projects and the coexistence of multiple configuration schemes such as excel and json in one project.

The Luban generation process is extremely fast. For ordinary meter guide tools, it often takes tens of seconds to generate the configuration in the later stage of a typical MMORPG project. Luban uses the client/server cloud generation model, through multi-threading and generation + object caching mechanism, in most cases, the entire generation process can be completed within 1 second.

Luban is suitable for developers with the following needs:
1. I hope to find a fast and powerful game configuration solution that has passed the test of the online project and meets the configuration requirements of **medium and large** game projects
2. Hope to easily customize configuration and message generation according to project requirements to meet more stringent memory and performance requirements
3. Hope to do other custom generation or caching



====** If you feel usable, please give it a like, your support will give us great motivation ^_^**====

## Support and contact

If you have any questions about the use, please add the QQ group to ask in time, and someone can help solve it at any time.

  - QQ group: 692890842
  - Email: taojingjian#gmail.com


## Documentation

- [Homepage](https://focus-creative-games.github.io/luban/index.html)
- [Features](docs/traits.md)
- [Introduction to Excel Configuration Data](docs/data_excel.md)
- [Document Catalog](docs/catalog.md)
- [FAQ](docs/faq.md)
- [[TODO] Quick Start and Advanced](docs/start_up.md)
- [[TODO] Complete Manual](docs/manual.md)
- **[====>Highly recommended to explore: example project <====](https://github.com/focus-creative-games/luban_examples)**

## Features
- Supports enhanced excel format, you can fill in arbitrarily complex data more concisely in excel
- Supports multiple data formats of excel, json, xml, and lua, which basically unifies the configuration data in the game
- Powerful and complete type system. Supports all common primitive types, datetime types, container types list, set, map, enumeration and structure, **polymorphic structure** and **nullable type**
- Flexible data source definition. A table can come from multiple files or multiple tables defined in a file, or all files in a directory or even from cloud tables, and a combination of the above
- Support table and field level grouping. You can selectively export tables and fields used by the client or server
- Multiple export data format support. Support export data formats such as binary, json, lua, etc.
- Support data tags. You can choose to export the data that meets the requirements, and you don’t have to manually comment out those test data when planning to release the official data.
- Powerful data verification capabilities. Support built-in data format check; support ref table reference check (don't worry about filling in the wrong id for planning); support path resource check (don't worry about filling in the wrong resource path for planning); support advanced custom verification (for example, two fields and must be 100)
- Support constant alias. Planning no longer has to write specific item IDs for items such as Upgrade Dan
- Support a variety of common data table modes. one (singleton table), map (regular key-value table)
- Support emmylua anntations. The generated lua contains anntations information that conforms to the emmylua format. Cooperate with emmylua, have good configuration code prompt ability
- Support res resource mark. You can export all resource lists (icon, ui, assetbundle, etc.) referenced in the configuration with one click
- Good modularity of generated code
- **Localization Support**
   - Support time localization. The datetime type data will be converted to UTC time in the target area at that time according to the specified timezone, which is convenient for the program to use.
  - **Support text static localization. When exporting, all text type data is correctly replaced with the final localized string.**
  - [TODO] supports dynamic localization of text. Dynamically switch all text type data to the target localized string at runtime.
  - **Support main + patches data merging. On the basic data, differential data is applied to generate the final complete data, which is suitable for the production of configuration data with subtle differences in multiple regions.**
- Support mainstream game development languages
   - c++ (11+)
   - c# (.net framework 4+. dotnet core 3+)
   - java (1.6+)
   - go (1.10+)
   - lua (5.1+)
   - js and typescript (3.0+)
   - python (2.7+ and 3.0+)
- Support mainstream engines and platforms
   - unity + c#
   - unity + tolua, xlua
   - unity + ILRuntime
   - unity + puerts
   - unreal + c++
   - unreal + unlua
   - unreal + sluaunreal
   - unreal + puerts
   - cocos2d-x + lua
   - cocos2d-x + js
   - WeChat Mini Program Platform
   - Other js-based small program platforms
   - All other engines and platforms that support lua
   - All other engines and platforms that support js


## luban workflow Pipeline

![pipeline](docs/images/pipeline.jpg)

## Quick preview

Unlike common table guide tools that focus on excel, the definition of luban is separated from the data, and a separate xml definition is used to define the table and structure. The data file only contains data.

### Regular primitive data

The following is a table containing all common simple primitive data.

```xml
<bean name="DemoPrimitiveTypesTable">
<var name="x1" type="bool"/>
<var name="x2" type="byte"/>
<var name="x3" type="short"/>
<var name="x4" type="int" />
<var name="x5" type="long" />
<var name="x6" type="float"/>
<var name="x7" type="double"/>
<var name="s1" type="string" />
<var anme="s2" type="text"/> Localized string, composed of two fields, key and text
<var name="v2" type="vector2"/>
<var name="v3" type="vector3"/>
<var name="v4" type="vector4"/>
<var name="t1" type="datetime"/>
</bean>

<table name="TbDemoPrimitive" index="x4" value="DemoPrimitiveTypesTable" input="demo_primitive.xlsx"/>
```

![ex_2](docs/images/examples/ex_2.png)


- name="TbDemoPrimitive" means that the data table name is TbDemoPrimitive, and this class name is used when the table code is generated.
- value="DemoPrimitiveTypesTable" means that the type of each row of the data table (ie V in KV) is DemoPrimitiveTypesTable.
- index="x4" means that the data table uses the x4 field of type <value> as the key. You can leave it blank, **default is the first field**.
- input="demo_primitive.xlsx" means that the data file of the data table is demo_primitives.xlsx

### Enumeration

Define the enumeration class, and force the enumeration name or alias to be filled in the configuration to improve the readability of the configuration.

```xml
<enum name="DemoEnum">
<var name="RED" alias="Red" value="1"/>
<var name="BLUE" alias="蓝" value="3"/>
<var name="GREEN" alias="green" value="5"/>
</enum>

<bean name="DemoEnumTable">
<var name="id" type="int"/>
<var name="x2" type="DemoEnum"/>
</bean>

<table name="TbDemoEnum" value="DemoEnumTable" input="demo_enum.xlsx"/>
```

![ex_12](docs/images/examples/ex_12.png)

### Custom structure bean

```xml
<bean name="IntRange">
<var name="min" type="int"/>
<var name="max" type="int"/>
</bean>

<bean name="DemoBeanTable">
<var name="id" type="int"/>
<var name="range" type="IntRange"/>
</bean>

<table name="TbDemoBean" value="DemoBeanTable" input="demo_bean.xlsx"/>
```

![ex_22](docs/images/examples/ex_22.png)

### Polymorphic structure bean
The type inheritance system that supports OOP is convenient for expressing multiple types of data, and is often used in modules such as skills and AI.

```xml
<bean name="Shape">
	<var name="id" type="int"/>
	<bean name="Circle">
		<var name="radius" type="float"/>
	</bean>
	<bean name="Rectangle" alias="长方形">
		<var name="width" type="float"/>
		<var name="height" type="float"/>
	</bean>
	<bean name="Curve">
		<bean name="Line" alias="直线">
			<var name="direction" type="vector2"/>
		</bean>
		<bean name="Parabola" alias="抛物线">
			<var name="param_a" type="float"/>
			<var name="param_b" type="float"/>
		</bean>
	</bean>
</bean>

<bean name="ShapeTable">
	<var name="id" type="int"/>
	<var name="shape" type="Shape"/>
</bean>

<table name="TbDemoShape" value="DemoShapeTable" input="demo_shape.xlsx"/>
```

![ex_32](docs/images/examples/ex_32.png)

### Nullable data type
There are often semantic requirements for null values in configuration data. In actual projects, 0 or -1 are often used mixedly to express null values, which is neither natural, clear nor uniform. Luban draws on the concept of nullable variables in c#, and specifically provides support for nullable data. All native data types, as well as enum, bean, and polymorphic bean types have corresponding nullable data types. The definition method is <type name>?, which is the same as the definition method of the Nullable type in c#. For example, bool?, int?, long?, double?, EColor?, DemoType?

```xml
	<bean name="DemoType1">
		<var name="x1" type="int"/>
	</bean>
	<bean name="DemoDynamic"> 多态数据结构
		<var name="x1" type="int"/>
		
		<bean name="DemoD2" alias="测试别名">
			<var name="x2" type="int"/>
		</bean>
		
		<bean name="DemoD3">
			<var name="x3" type="int"/>
		</bean>
	</bean>
	<bean name="TestNull">
		<var name="id" type="int"/>
		<var name="x1" type="int?"/>
		<var name="x2" type="DemoEnum?"/>
		<var name="x3" type="DemoType1?"/>
		<var name="x4" type="DemoDynamic?"/>
	</bean>
	<table name="TbTestNull" value="TestNull" input="test/test_null.xlsx"/>
```

![ex_42](docs/images/examples/ex_42.png)

### Simple primitive data list type
Generally speaking, you can either fill in a cell separated by a comma ",", or fill in one data for each cell. note! Empty cells will be ignored.

```xml
<bean name="CollectionTable">
	<var name="id" type="int"/>
	<var name="items" type="list,int"/>
	<var name="coefs" type="list,int"/>
</bean>

<table name="TbSimpleCollection" value="CollectionTable" input="collection.xlsx">
```

![ex_52](docs/images/examples/ex_52.png)

### Structure list
For the structure list type, there are multiple types of filling. Plan to choose the most suitable filling method according to the specific situation.

1. Expand all.
```xml
	<bean name="Item">
		<var name="id" type="int"/>
		<var name="name" type="string"/>
		<var name="num" type="int"/>
	</bean>

	<bean name="CollectionTable2">
		<var name="id" type="int"/>
		<var name="items" type="list,Item"/>
	</bean>

	<table name="TbBeanCollection" value="CollectionTable2" input="collection2.xlsx">
```

![ex_61](docs/images/examples/ex_61.png)

1. Each Item is in a cell

```xml
	<bean name="Item" sep=",">
		<var name="id" type="int"/>
		<var name="name" type="string"/>
		<var name="num" type="int"/>
	</bean>

	<bean name="CollectionTable2">
		<var name="id" type="int"/>
		<var name="items" type="list,Item"/>
	</bean>

	<table name="TbBeanCollection" value="CollectionTable2" input="collection2.xlsx">
```

![ex_62](docs/images/examples/ex_62.png)

1. All data are in one cell
```xml
	<bean name="Item" sep=",">
		<var name="id" type="int"/>
		<var name="name" type="string"/>
		<var name="num" type="int"/>
	</bean>

	<bean name="CollectionTable2">
		<var name="id" type="int"/>
		<var name="items" type="list,Item" sep="|"/>
	</bean>

	<table name="TbBeanCollection" value="CollectionTable2" input="collection2.xlsx">
```

![ex_63](docs/images/examples/ex_63.png)

### Polymorphic structure list

```xml
<bean name="CollectionTable3">
	<var name="id" type="int"/>
	<var name="shapes" type="list,Shape" sep=","/>
</bean>

<table name="TbBeanCollection3" value="CollectionTable3" input="collection3.xlsx">
```

![ex_71](docs/images/examples/ex_71.png)

### Multi-line records

It is often encountered that some records contain a list: bean type structure list. If one line of configuration is forcibly required, the readability and maintainability will be poor. If the table is disassembled, it will not be friendly to planning and procedures. We support multi-row configuration for this type of data, and only need to add the attribute multi_rows="1" after the multi-row field.
Examples are as follows:

The **levels** field in the hero level table is a list. We mark it as multi_rows and fill in multiple rows.


definition:

![multi_define](docs/images/examples/multi_01.png)

data:

![multi_data](docs/images/examples/multi_02.png)


### Singleton table

Singleton refers to the meaning of singleton in code mode, which is used to configure only one copy of data globally.

```xml
<bean name="SingletonTable">
	<var name="init_gold_num" type="int"/>
	<var name="guild_module_open_level" type="int"/>
</bean>

<table name="TbSingleton" value="SingletonTable" mode="one" input="examples.xlsx"/>
```

Luban supports horizontal and vertical tables, and the default is horizontal table. For singleton tables, the vertical table is more comfortable to fill in, so we fill in row:0 in cell B1 of excel to indicate that it is a vertical table.

![ex_a1](docs/images/examples/ex_a1.png)

### Data Validation

It supports key reference checking, path resource checking, and range checking.

- Reference check

   For simple data types such as int, long, and string, you can check whether the data is a valid id of a table, which is very common in game configuration. For example, in the TbBonus table below, item_id must be a legal item id, then explicitly constrained by ref="item.TbItem". If an illegal integer value is filled in, a warning will be printed during the generation process. It is found that the reference error will not stop the generation, and the data will still be exported. Because in the actual project development process, it is very common to temporarily generate some illegal data due to frequent data changes. These incorrect data may not affect most of the tests. If you stop the generation , Will cause unrelated development students to often be blocked in the development process.

   Sometimes you don't want to check the value of 0 or empty string, you can pass ref="<full table name>?" to indicate that the default value data is ignored, such as ref="item.TbItem?". If it is a nullable data type such as int?, ref="<table name>?" is not required, and null data will be automatically ignored, but the value of 0 will still be checked and a warning will be issued.

```xml
	<module name="item">
		<bean name="Item">
			<var name="id" type="int">
			<var name="num" type="int">
		</bean>

		<table name="TbItem" value="Item" input="item/item.xlsx">

		<bean name="Bonus1">
			<var name="id" type="int">
			<var name="item_id" type="int" ref="item.TbItem">
			<var name="num" type="int">
		</bean>
		<table name="TbBonus" value="Bonus1" input="item/bonus.xlsx">

		<bean name="Bonus2">
			<var name="id" type="int">
			<var name="item_id" type="int?" ref="item.TbItem">
			<var name="num" type="int">
		</bean>
		<table name="TbBonus2" value="Bonus2" input="item/bonus2.xlsx">
	</module>
```

![ex_e1](docs/images/examples/ex_e1.png)

![ex_e2](docs/images/examples/ex_e2.png)

![ex_e3](docs/images/examples/ex_e3.png)

- path resource check

It is used to check whether the resource path filled in the planning is legal, and to prevent errors that cannot be found when the resource is running. The goal has been to implement a special resource check mechanism for Unity and UE4. For details, please see [Complete Manual](docs/manual.md)

For example, the icon field in the equipment table of the unity project must be a valid resource, add the definition path="unity" in the icon field

Definition:

![path_define](docs/images/examples/path_01.png)

Data:

![path_data](docs/images/examples/path_02.png)

- range check

It is used to check whether the data filled in the planning is within the legal range, and supports [x,y],[x,],(x,y),(x,) and other open and closed interval writing. For details, please see [Complete Manual](docs/manual.md)

Example: The hero's position coordinates must be within the range of [1,5], then add the range="[1,5]" attribute to the position field

Definition:

![range_define](docs/images/examples/range_01.png)

Data:

![range_data](docs/images/examples/range_02.png)

### Group export

In most projects, the data exported to the front and back ends are not exactly the same. Some tables may only be required by the front-end or the back-end, and some fields may be required only by the front-end or the back-end.

Luban supports two levels of grouping at the same time:
- Table level grouping

The definition method is to define the group attribute in the table. If the group is not defined, it will be exported to all groups by default. If the group is defined, it will only be exported to the specified group. There can be multiple, separated by commas ",".

&lt; table name="xxx" group="&lt;group1&gt;,&lt;group2&gt;..." &gt;

For example: The TbDemoGroup_C table is only used by the client, TbDemoGroup_S can only be used by the server, and TbDemoGroup_E is only used by the editor.
It is defined as follows:

![group_table](docs/images/examples/group_02.png)

- Field level grouping

The definition method is to assign the group attribute to var. If it is not specified, it will be exported to all groups by default. It can be multiple, separated by a comma ",". Compared with most table guide tools that only support group export of **table top-level fields**, luban supports group export of any bean field granularity level.

&lt;var name="xxx" group="&lt;group1&gt;,&lt;group2&gt; ..." /&gt;


For example, the id, x1, and x4 fields in the TbDemoGroup table are required for the front and back ends; x3 is only required for the back end; and the x2 field is only required for the front end. x5 is a bean type, it is exported to the front and back ends, but its sub-fields can also be grouped and filtered, x5.y1, x2.y4 will be exported at the front and back ends, x5.x3 is only exported to the back end, and x5.x2 is only exported to the front end .
It is defined as follows:

![group_var](docs/images/examples/group_01.png)


### Tag data filtering

Selectively export data that meets the requirements based on data tags. It is suitable for some situations, such as: some test data was produced during the research and development period. We hope that these data will not be exported when it is officially launched, but we do not want to manually delete those records in the excel sheet, because these test data are still needed for intranet testing. A more elegant way is to mark these data as TEST (or test, or other labels), and ignore data with some labels when exporting.

Example: 102149001 and 102149002 are test items and hope they will not be included in the official release. As long as you turn off -- export- test in the command line option, these test data will not be exported.

![tag](docs/images/examples/tag_01.png)

### Constant alias

Some digital frequencies are often used in the project, such as upgrading the Dan item id. Plan to fill in the number every time, it is easy to make mistakes and fill in the wrong. We allow you to specify constant aliases for integers. When the tool exports the configuration, it will automatically replace the aliases with the corresponding integers.

### Multiple data sources

- One data table comes from two excel files

Specify that the data of the data table comes from multiple files by means of excel file 1, excel file 2..., and different files are separated by commas ",". When the data source is an excel file and @ is not used to specify a cell table, all cell tables in the excel file will be read in. For example, the data of the TbItem table comes from item1.xlsx and item2.xlsx in the item directory.

```xml
		<bean name="Item">
			<var name="id" type="int">
			<var name="num" type="int">
		</bean>

		<table name="TbItem" value="Item" input="item/item1.xlsx,item/item2.xlsx">
```

![ex_c1](docs/images/examples/ex_c1.png)

![ex_c2](docs/images/examples/ex_c2.png)

![ex_c3](docs/images/examples/ex_c3.png)

![ex_c4](docs/images/examples/ex_c4.png)

- Two data tables are from different cell tables of the same excel file
To
Specify the data from a certain unit table of the excel file by way of <unit table name>@excel file, you can specify multiple unit tables, separated by commas ",". In the example, TbItem occupies two cell tables, table1 and table3; TbEquip occupies two cell tables, table2 and table4. The cell tables occupied by the same data table need not be consecutive. In the example, TbItem and TbEquip are deliberately accounted for two non-adjacent unit tables.
```xml
	<bean name="Item">
		<var name="id" type="int">
		<var name="num" type="int">
	</bean>

	<table name="TbItem" value="Item" input="table1@examples.xlsx,table3@examples.xlsx">

	<bean name="Equip">
		<var name="id" type="int">
		<var name="count" type="int">
	</bean>

	<table name="TbEquip" value="Equip" input="table2@examples.xlsx,table4@examples.xlsx">
```
![ex_b1](docs/images/examples/ex_b1.png)

![ex_b2](docs/images/examples/ex_b2.png)

![ex_b3](docs/images/examples/ex_b3.png)

![ex_b4](docs/images/examples/ex_b4.png)


- The data of a data table comes from all the files under the **directory**
To
When using a directory as the data source, all files in the entire directory tree will be traversed, except for the file whose name starts with ",.~" (character comma or period or tilde), read in the data in each file. If it is the data of the excel family, multiple records will be read from each file. If it is the data of the xml, lua, json family, each file will be read as one record. You can specify multiple directories as data sources at the same time, separated by commas ",".
```xml
	<bean name="Item">
		<var name="id" type="int">
		<var name="num" type="int">
	</bean>

	<table name="TbItem" value="Item" input="item.xlsx">

```
![ex_d1](docs/images/examples/ex_d1.png)

![ex_c1](docs/images/examples/ex_c1.png)

![ex_c3](docs/images/examples/ex_c3.png)

### json data source
In a large and complex project, some table data is saved in json format, such as skills, AI, plot, etc. Conventional table guide tools can only process excel. Data such as xml and json are generally processed by programmers themselves, which eventually leads to several configuration loading schemes in the game, and the front and back end
Editor developers have to spend a lot of time writing code to process these data, which is troublesome and difficult to locate errors.

Luban unifies all configurations through **definition + data source**. The usage of json data source is basically the same as that of excel data source, the only difference is
The format of the input data file is changed from xlsx to json. In the actual project, if json is used as the data format, in order to facilitate editor processing, generally one record occupies one file, and all records are unified under one directory, so the data source becomes a directory. The input="test/json_datas" directory in the figure below.

```xml
<bean name="DemoType2" >
	<var name="x4" type="int" convert="DemoEnum"/>
	<var name="x1" type="bool"/>
	<var name="x2" type="byte"/>
	<var name="x3" type="short" ref="test.TbFullTypes"/>
	<var name="x5" type="long" convert="DemoEnum"/>
	<var name="x5" type="long" convert="DemoEnum"/>
	<var name="x6" type="float"/>
	<var name="x7" type="double"/>
	<var name="x8_0" type="fshort"/>
	<var name="x8" type="fint"/>
	<var name="x9" type="flong"/>
	<var name="x10" type="string" path="normal;*.txt"/>
	<var name="x12" type="DemoType1"/>
	<var name="x13" type="DemoEnum"/>
	<var name="x14" type="DemoDynamic" sep=","/>多态数据结构
	<var name="v2" type="vector2"/>
	<var name="v3" type="vector3"/>
	<var name="v4" type="vector4"/>
	<var name="t1" type="datetime"/>
	<var name="k1" type="array,int"/> 使用;来分隔
	<var name="k2" type="list,int"/>
	<var name="k3" type="linkedlist,int"/>
	<var name="k4" type="arraylist,int"/>
	<var name="k5" type="set,int"/>
	<var name="k6" type="treeset,int"/>
	<var name="k7" type="hashset,int"/>
	<var name="k8" type="map,int,int"/>
	<var name="k9" type="list,DemoE2" sep="," index="y1"/>
	<var name="k15" type="array,DemoDynamic" sep=","/> 
</bean>

<table name="TbDataFromJson" value="DemoType2" input="test/json_datas"/>
```

Using the directory as the data source, recursively traverse the entire directory tree, **after sorting by file**, read each json data as a record in turn.

![ex_81](docs/images/examples/ex_81.png)

The content of the 1.json file is as follows

```json
 {
	"x1":true,
	"x2":3,
	"x3":128,
	"x4":1,
	"x5":11223344,
	"x6":1.2,
	"x7":1.23432,
	"x8_0":12312,
	"x8":112233,
	"x9":223344,
	"x10":"hq",
	"x12": { "x1":10},
	"x13":"B",
	"x14":{"__type__": "DemoD2", "x1":1, "x2":2},
	"v2":{"x":1, "y":2},
	"v3":{"x":1.1, "y":2.2, "z":3.4},
	"v4":{"x":10.1, "y":11.2, "z":12.3, "w":13.4},
	"t1":"1970-01-01 00:00:00",
	"k1":[1,2],
	"k2":[2,3],
	"k3":[1,3],
	"k4":[1,5],
	"k5":[1,6],
	"k6":[1,7],
	"k7":[2,3],
	"k8":[[2,2],[4,10]],
	"k9":[{"y1":1, "y2":true},{"y1":2, "y2":false}],
	"k15":[{"__type__": "DemoD2", "x1":1, "x2":2}]
 }
```

### xml data source
definition

```xml

<table name="TbDataFromXml" value="DemoType2" input="test/xml_datas"/>
```

Take the directory as the data source, traverse the entire directory tree recursively, and read each xml data as a record.

The content of the 1.xml file is as follows
```xml
 <data>
	<x1>true</x1>
	<x2>4</x2>
	<x3>128</x3>
	<x4>1</x4>
	<x5>112233445566</x5>
	<x6>1.3</x6>
	<x7>1112232.43123</x7>
	<x8>112233</x8>
	<x8_0>123</x8_0>
	<x9>112334</x9>
	<x10>yf</x10>
	<x12>		<x1>1</x1>	</x12>
	<x13>C</x13>
	<x14 __type__="DemoD2">		<x1>1</x1>		<x2>2</x2>	</x14>
	<v2>1,2</v2>
	<v3>1.2,2.3,3.4</v3>
	<v4>1.2,2.2,3.2,4.3</v4>
	<t1>1970-01-01 00:00:00</t1>
	<k1>    <item>1</item>	<item>2</item>	</k1>
	<k2>	<item>1</item>	<item>2</item>	</k2>
	<k3>	<item>1</item>	<item>2</item>	</k3>
	<k4>	<item>1</item>	<item>2</item>	</k4>
	<k5>	<item>1</item>	<item>2</item>	</k5>
	<k6>	<item>1</item>	<item>2</item>	</k6>
	<k7>	<item>1</item>	<item>3</item>	</k7>
	<k8>
		<item> <key>2</key><value>10</value></item>
		<item> <key>3</key><value>30</value></item>
	</k8>
	<k9>
		<item>	<y1>1</y1>	<y2>true</y2>	</item>
		<item>	<y1>2</y1>	<y2>false</y2>	</item>
	</k9>
	<k15>
		<item __type__="DemoD2">	<x1>1</x1>	<x2>2</x2>	</item>
	</k15>
</data>
```
### lua data source

definition

```xml
<table name="TbDataFromLua" value="DemoType2" input="test/lua_datas"/>
```
Take the directory as the data source, traverse the entire directory tree recursively, and read each lua data as a record.

The content of 1.lua file is as follows

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
   x8 = 12,
   x8_0 = 13,
   x9 = 123,
   x10 = "yf",
   x12 = {x1=1},
   x13 = "D",
   x14 = {__type__="DemoD2", x1 = 1, x2=3},
   v2 = {x = 1,y = 2},
   v3 = {x=0.1, y= 0.2,z=0.3},
   v4 = {x=1,y=2,z=3.5,w=4},
   t1 = "1970-01-01 00:00:00",
   k1 = {1,2},
   k2 = {2,3},
   k3 = {3,4},
   k4 = {1,2},
   k5 = {1,3},
   k6 = {1,2},
   k7 = {1,8},
   k8 = {[2]=10,[3]=12},
   k9 = {{y1=1,y2=true}, {y1=10,y2=false}},
   k15 = {{ __type__="DemoD2", x1 = 1, x2=3}},
}
```

------

## Code usage example

Here is only a brief display of the usage of lua, c#, typescript, and go languages ​​in development. For more languages ​​and more detailed usage examples and codes, please see [Sample Project](https://github.com/focus-creative-games/luban_examples ).

- Lua usage example

  ```Lua
  -- Access a singleton table
  print(require("TbGlobal").name)
  -- Access ordinary key-value table
  print(require("TbItem")[12].x1)
  ```

- C# usage example

  ```C#
  // One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
  var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes("<data path>/" + file)));
  // Access a singleton table
  Console.WriteLine(tables.TbGlobal.Name);
  // Access the ordinary key-value table
  Console.WriteLine(tables.TbItem.Get(12).X1);
  // Support operator [] usage
  Console.WriteLine(tables.TbMail[1001].X2);
  ```

- Typescript usage example

```typescript
// One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))
// Access a singleton table
console.log(tables.TbGlobal.name)
// Access ordinary key-value table
console.log(tables.TbItem.get(12).x1)
```

- golang usage example
```go
// One line of code can load all configurations. cfg.Tables contains an instance field for all tables.
if tables, err := cfg.NewTables(loader); err != nil {
   println(err.Error())
   return
}
// Access a singleton table
println(tables.TbGlobal.Name)
// Access the ordinary key-value table
println(tables.TbItem.Get(12).X1)

```
- [Examples in more languages](docs/samples.md)

------
## route map

- [ ] Added unity built-in editor
- [ ] Added unreal built-in editor
- [ ] Supplementary unit test

## Development environment setup

- Install [VS2019 Community Edition](https://visualstudio.microsoft.com/zh-hans/vs/)
- Install [.dotnet core sdk 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

## Subordinate luban-server

- Based on docker

docker run -d --rm --name luban-server -p 8899:8899 focuscreativegames/luban-server:latest

- Based on .net 5 runtime (recommended for win platform, cross-platform, no need to recompile)
- Install .net 5 runtime by yourself.
- Copy Luban.Server from [example project](https://github.com/focus-creative-games/luban_examples/tree/main/Tools/Luban.Server) (** can be cross-platform, even on linux and mac platforms No need to recompile **)
- Run dotnet Luban.Server.dll in the Luban.Server directory

## Use luban-client
- Based on docker (recommended for linux and mac platforms)

   docker run --rm -v $PWD/.cache.meta:/bin/.cache.meta focuscreativegames/luban-client <parameter>

   To Remind! The .cache.meta file is used to save the md5 cache of the files generated locally or submitted to the remote. **strongly recommended** add the -v mapping, otherwise every time all the md5 involved in the file is recalculated, this may cause more in the later stage of the project A delay of a few seconds.
- Based on .net 5 runtime (recommended for win platform, cross-platform, no need to recompile)
- Install .net 5 runtime by yourself.
- Copy Luban.Client from [example project](https://github.com/focus-creative-games/luban_examples/tree/main/Tools/Luban.Client) (** can be cross-platform, even on linux and mac platforms No need to recompile **)
- Run dotnet Luban.Client.dll <parameters> in the Luban.Client directory

## How to contribute

- [Contributing](CONTRIBUTING.md) explains what kinds of changes we welcome
- [Workflow Instructions](docs/workflow/README.md) explains how to build and test

## Useful links

- [.NET Core source index](https://source.dot.net)



## License

Luban is licensed under the [MIT](LICENSE.TXT) license.