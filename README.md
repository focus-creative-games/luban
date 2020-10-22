[//]: # (Author: bug)
[//]: # (Date: 2020-10-20 20:24:07)

# Luban
[![license](http://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/Tencent/puerts/blob/master/LICENSE)
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban)

## 介绍

Luban 是一个强大的生成与缓存工具。最初只用于对象生成，对象可以是常规代码、配置数据、类似protobuf的消息代码，也可以是游戏资源如assetbundle。
在大型项目中，由于配置或资源数据庞大，生成对象可能会花费相当多的时间。比如一个典型的MMORPG项目，后期全量生成配置，即使用了多线程加速，所需时间
也在10秒的级别。因此使用client/server模式，新增缓存机制来加速生成过程，能将生成时间时间降低到秒级。

Luban 最初是为了解决传统excel导出工具功能过于薄弱，无法很好解决MMORPG游戏复杂配置需求的痛点问题。 自2015年以来，经历过 MMORPG、卡牌、SLG 等多个上线项目的考验，
实际项目过程中不断迭代和优化，最终由一个增强型的配置工具成为一个 **相对完备的游戏配置数据解决方案**。


## 文档
* [主页](https://focus-creative-games.github.io/luban/index.html)
* 各语言的简介: [English](README.en-us.md), [简体中文](README.md)

## 特性

### 完备的类型系统

* 基础内置类型
   - bool,byte,short,fshort,int,fint,long,flong,float,double,string,text,bytes
   - vector2, vector3,vector4
   - datetime
* 可空类型
   - bool?,byte?,short?,fshort?,int?,fint?,long?,flong?,float?,double?
   - vector2?,vector3?,vector4?
   - datetime?
* 自定义枚举 enum 及相应可空类型
* 自定义常量 const
* 自定义结构 bean
* 多态bean

支持定义类似无限层次的OOP类型继承体系(比如父类Shape，子类Circle,Rectangle)，在表达复杂配置时极为有效，对程序友好，同时策划也可以比较简洁地配置出Shape数据，不必定义一个通用型的包含所有字段的Shape了。

* 支持容器类型 array。 value 可以为内置类型，也可以为自定义类型
* 支持容器类型 list。 value 可以为内置类型，也可以为自定义类型	
* 支持容器类型 set。 value 只能为内置类型或者enum类型，不支持 bean 类型
* 支持容器类型 map。 key 只能为内置类型或者enum类型，不支持 bean 类型。 value 可以为内置类型，也可以为自定义类型


### 多种原始文件格式支持
一个复杂项目中，总有一部分数据(10-20%)不适合excel编辑（比如技能、AI、副本等等），这些数据一般通过专用编辑器来编辑和导出。遇到的典型问题是，这种配置数据是无法与excel数据统一处理的，造成游戏内有多种配置数据加载方案，程序需要花费很多时间去处理这些数据的加载问题。另外这些复杂数据无法使用数据检验和分组导出以及本地化等等excel导表工具的机制。Luban能够处理excel族、xml、json、lua、目录等多种数据源，统一导出数据和生成代码，所有数据源都能使用数据检验、分组导出等等机制，程序彻底从复杂配置处理中解脱出来。

* 支持excel族文件。 csv 及 xls,xlsx等格式
* 支持从指定excel里的某个单元薄读入。
* 支持json。 每个json文件当作一个记录读入
* 支持lua。 每个lua文件当作一个记录读入
* 支持xml。 每个xml文件当作一个记录读入
* 支持目录。 递归目录下的所有文件，每个文件当作一个记录读入。允许不同类型的文件混合，比如目录下可以同时有json,lua,xml,excel之类的格式。
* 每个表允许指定多个数据源，可以使用以上所有的组合。
   - 多对一。比如可以在一个excel里用不同页签配置不同的表。比如 装备升级表和进阶表都在 装备表.xlsx中
   - 一对多。比如任务表可以来 任务1.xlsx，任务2.xlsx 等等多个表。
   - 多对多。还可以是以上组合，不过实际中很少见）

### 支持增强的excel格式，可以比较方便地填写任意复杂的数据
* 强制用 true,false表示 bool变量。
* 强制用枚举名及别名表示枚举常量。比如用 白绿红之类表示品质，而不是1,2,3这样的magic number
* 支持整数的常量替换。比如说 升级丹道具id 为 1122，所有填升级丹id的地方可以填 升级丹 来表示。减少填写错误
* 支持可空变量. 用 null 表示空数据.
* 支持 datetime 数据类型. 时间格式标准为以下几种，最终被转化为utc时间方便程序处理
    - yyyy-MM-dd HH:mm:ss
    - yyyy-MM-dd HH:mm
    - yyyy-MM-dd HH
    - yyyy-MM-dd
* 支持用sep拆分单元格。在一个单元格里填写多个数据。
* 支持多行数据。例如，章节配置里有一个复杂小节列表字段。支持多行填写这个列表数据。
* 支持多级标题头,方便对应一些比较深的数据。比如  a.b.c 这种。
* 支持多态别名，可以方便填写多态数据。比如说 圆,5 或者 长方形,3,4
* 支持在excel里比较简洁填写出任意复杂的配置。比如 equips:list,Equip (包含 int id, string name, float attr) 这样一个复杂结构列表数据，可以填成  1,abasfa,0.5|2,xxxyy;0.9。而大多数导表工具只支持 简单类型的列表，必须拆分为 equip_ids:list,int; equip_names:list,string;equip_attrs:list,float，无论对于策划还是程序都非常不友好。
	
### 多种导出数据格式支持
 **导出格式与原始数据解耦**。无论源数据是 excel、lua、xml、json 或者它们的混合, 最终都被以**统一的格式**导出，极大简化了生成代码的复杂性。 目前支持以下几种导出格式：
* binary格式。与pb格式类似。所占空间最小，加载最快。
* json 格式。
* lua 格式。
* 扩展其他格式也很容易。（像前几种数据格式导出只用200行代码）

### 支持表与字段级别分组
  支持自定义分组类型。既支持按分组选择性导出一部分表，也支持选择性导出表中的一部分字段。比如为前后端和编辑器分别导出他们所用的数据。

### 支持数据标签
支持 是、否、test 三种标签。可以为每行数据加标签。比如标签为"否"表示这行数据不被导出。 如果为 "test"，则只在测试导出情况下才导出。用于比如说单元测试内部开发时导出配置，但对外发布时不导出的情形。

### 强大的数据校验能力
* 完整的数据内建约束检查
* ref 检查。检查表引用合法性。比如 Monster表中的dropId必须是合法的 TbDrop表的key.
* path 检查。检查资源引用合法性。比如 Monster表的icon必须是合法的图标资源。对于防止策划填错极其有用，不再需要运行时才发现资源配置错误了。
* range 检查。检查数值范围。
* 扩展的自定义检查。使用自定义代码进行高级检查。提交配置前就能完成本地检查，避免运行时才发现错误，极大减少迭代成本。

### 多种数据表模式
* one 格式，即单例表模式
* map 格式，即普通key-value表模式。 任何符合set 的value要求的类型都可以做key
* bmap 格式，即双主键模式。 任何符合 set 的value要求的类型都可以作 key1和key

### 本地化支持
* 支持**本地化时间**。 配置中的 datetime会根据指定的timezone及localtime转化为正确的utc时间，方便程序处理
* 支持**静态本地化**。 配置中的text类型在导出时已经转化为正确的本地化后的字符串
* 支持**动态本地化**。 配置中的text类型能运行时全部切换为某种本地化后的字符串

### 代码编辑器支持
  支持 emmylua anntations。生成的lua包含符合emmylua 格式anntations信息。配合emmylua有良好的配置代码提示能力。

### 资源导出支持  
  支持 res 资源标记。可以一键导出配置中引用的所有资源列表(icon,ui,assetbundle等等)

### 代码模块化
生成较优的模块化的代码。比如
- c#   cfg.item.ItemInfo
- c++  cfg::item::ItemInfo
- lua  item.ItemInfo
- go   item_ItemInfo

### 生成极快，大型项目也能秒级导出
  使用 client/server模式，利用服务器强大的硬件(大内存+多线程)，同时配合缓存机制（如果数据和定义未修改，直接返回之前生成过的结果），即使到项目中后期数据规模比较	大也能1秒（传统在10秒以上）左右生成所有数据并且完成数据校验。考虑策划和程序经常使用生成工具，积少成多，也能节省大量研发时间。

### 数据模块化
策划可以方便地按需求自己组织数据目录和结构，不影响逻辑表。

### 支持主流的游戏开发语言
   - c++ (11+)
   - c# (.net framework 2+. dotnet core 2+)
   - java (1.6+)
   - go (1.10+)
   - lua (5.1+)
   - js 和 typescript (3.0+)
   - python (2.7+ 及 3.0+)
   
### 支持主流引擎和平台
   - unity + c#
   - unity + tolua,xlua
   - unity + ILRuntime
   - unreal + c++
   - unreal + unlua
   - unreal + puerts
   - cocos2d-x + lua
   - cocos2d-x + js
   - 微信小程序平台
   - 其他家基于js的小程序平台
   - 其他所有支持lua的引擎和平台
   - 其他所有支持js的引擎和平台

### 提供定制开发服务 ^_^

## 使用示例
  * Lua 使用示例  
    ``` Lua
    local data = require("TbDataFromJson")
    local cfg = data[32]
    print(cfg.name)
    ```
	
  * C# 使用示例
	``` C#
	var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes("output_data/" + file)));
	Console.WriteLine(tables.TbSingleton.Name);
	Console.WriteLine(tables.TbDataFromJson.Get(12).X1);
	Console.WriteLine(tables.TbTwoKey.Get(1, 10).X8);
	```
  * [更多语言的例子](docs/samples.md)

## RoadMap
- [ ] 新增 unity 内置编辑器  
- [ ] 新增 unreal 内置编辑器  
- [ ] 补充单元测试
- [ ] 支持多国家和地区本地化所需的表merge操作

## 布署 
    TODO

## 开发环境架设
* 安装 VS2019 社区版
* 安装 .dotnet core sdk 3.1

## 如何贡献?
* [Contributing](CONTRIBUTING.md) explains what kinds of changes we welcome
- [Workflow Instructions](docs/workflow/README.md) explains how to build and test

## Useful Links

* [.NET Core source index](https://source.dot.net) 
* 社区的其它实现
    * [tabtoy](https://github.com/davyxu/tabtoy)

## 支持和联系
    QQ 群: 692890842
    邮箱: taojingjian#gmail.com
    
## License

Luban is licensed under the [MIT](LICENSE.TXT) license.