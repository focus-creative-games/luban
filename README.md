[//]: # (Author: bug)
[//]: # (Date: 2020-10-20 20:24:07)

# Luban

## 介绍

Luban 是一个强大的生成与缓存工具。最初只用于对象生成，对象可以是常规代码、配置数据、类似protobuf的消息代码，也可以是游戏资源如assetbundle。
在大型项目中，由于配置或资源数据庞大，生成对象可能会花费相当多的时间。比如一个典型的MMORPG项目，后期全量生成配置，即使用了多线程加速，所需时间
也在10秒的级别。因此使用client/server模式，新增缓存机制来加速生成过程，能将生成时间时间降低到秒级，在项目中后期积少成多，可以帮助节省不少策划和程序的时间。

Luban 最初是为了解决传统excel导出工具功能过于薄弱，无法很好解决MMORPG游戏复杂配置需求的痛点问题。 自2015年以来，经历过 MMORPG、卡牌、SLG 等多个上线项目的考验，
实际项目过程中不断迭代和优化，理解逐渐深入，最终由一个增强型的配置工具成为一个 **相对完备的游戏配置数据解决方案**。

## 核心优势
相对于传统excel导表工具，Luban的核心优势在于：

* 完备的数据类型系统，增强的excel格式支持。
    > 策划可以在excel里比较简洁填写出任意复杂的配置，比如 equips:list,Equip(包含 int id, string name, float attr) 这样一个复杂结构列表数据。而大多数导表工具
	只支持 简单类型的列表，必须拆分为  equip_ids:list,int; equip_names:list,string;equip_attrs:list,float，无论对于策划还是程序都非常不友好。
* 支持多态 bean 
	> 支持定义类似 父类Shape，子类 Circle,Rectange 这样的OOP类型体系，在表达复杂配置时极为有效，对程序友好，同时策划也可以比较简洁地配置出Shape数据，不必定义
	一个通用型的包含所有字段的Shape了（对程序不友好，而且浪费内存）。
* 先进的定义 + 数据源的设计，支持多数据源，统一所有游戏配置
	> 一个复杂项目中，总有一部分数据(10-20%)不适合excel编辑（比如技能、AI、副本等等），这些数据一般通过专用编辑器来编辑和导出。遇到的典型问题是，这种配置数据是无法
	与excel数据统一处理的，造成游戏内有多种配置数据加载方案，程序需要花费很多时间去处理这些数据的加载问题。另外这些复杂数据无法使用数据检验和分组导出以及本地化
	等等excel导表工具的机制。Lunan 能够处理 excel族、xml、json、lua、目录等多种数据源，统一导出数据和生成代码，所有数据源都能使用数据检验、分组导出等等机制，
	程序彻底从复杂配置处理中解脱出来。
* 极快，大型项目也能秒级导出
	> 使用 client/server模式，利用服务器强大的硬件(大内存+多线程)，同时配合缓存机制（如果数据和定义未修改，直接返回之前生成过的结果），即使到项目中后期数据规模比较
	大也能1秒（传统在10秒以上）左右生成所有数据并且完成数据校验。考虑策划和程序经常使用生成工具，积少成多，也能节省大量研发时间。
* 支持主流的游戏开发语言
   - c++ (11+)
   - c# (.net framework 2+. dotnet core 2+)
   - java (1.6+)
   - go (1.10+)
   - lua (5.1+)
   - js 和 typescript (3.0+)
   - python (2.7+ 及 3.0+)
   
* 支持主流引擎
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

## 文档
* [主页](https://focus-creative-games.github.io/luban/index.html)
* 各语言的简介: [English](README.en-us.md), [简体中文](README.md)

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
  
## 特性
  * [完备的数据类型支持](docs/feature.md#支持的数据类型)
  * [多类型数据源支持](docs/feature.md#多数据源支持)
  * [多种数据表模式](docs/feature.md#多种数据表模式)
  * [按组导出数据](docs/feature.md#如何自定义导出分组)
  * [生成速度快](docs/feature.md#生成极快)
  * [增强 Excel 的表达](docs/feature.md#增强的-excel-格式)
  * [根据开发效率需求定制的数据输出格式](docs/feature.md#支持多种导出数据格式)
  * [本地化支持](docs/feature.md#本地化支持)
  * [代码提示支持](docs/feature.md#代码编辑器支持)
  * [强大的数据校验能力](docs/feature.md#强大的数据校验能力)
  * [资源导出支持](docs/feature.md#资源导出支持)
  * [自动代码生成](docs/feature.md#优秀的代码生成)
  * [数据分组](docs/feature.md#良好的数据组织)
  * [多语言支持](docs/feature.md#支持的语言-覆盖主流的语言)
  * [多服务器引擎支持](docs/feature.md#支持的服务器引擎-满足语言版本的情况下)
  * [多客户端引擎支持](docs/feature.md#支持的客户端引擎-满足语言版本的情况下)
  * [扩展能力](docs/feature.md#强大的扩展能力)
  * [ ] 提供定制开发服务 ^_^

## RoadMap
- [ ] 新增 unity 内置编辑器  
- [ ] 新增 unreal 内置编辑器  
- [ ] 补充单元测试

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