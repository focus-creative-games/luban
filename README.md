[//]: # (Author: bug)
[//]: # (Date: 2020-10-20 20:24:07)

# Luban
[![license](http://img.shields.io/badge/license-MIT-blue.svg)]
[![Build Status](https://travis-ci.com/focus-creative-games/luban.svg?branch=main)](https://travis-ci.com/focus-creative-games/luban)

![](docs/images/icon.png)

## 介绍

  - Luban 是一个强大的配置生成与缓存工具。
  - Luban 最初是为了解决传统excel导出工具功能过于薄弱，无法很好处理 MMORPG 游戏复杂配置需求的痛点问题。 
  - 生成目标可以是常规代码、配置数据、类似 protobuf 的消息代码，也可以是游戏资源如 assetbundle。
  - 在大型项目中，由于配置或资源数据庞大，生成对象可能会花费相当多的时间。
    - 比如一个典型的MMORPG项目，后期全量生成配置，即使用了多线程加速，所需时间也在10秒的级别。
    - 因此除了使用缓存，还使用了 client/server 模式，来加速生成过程。
  - 自2015年以来，经历过 MMORPG、卡牌、SLG 等多个上线项目的考验，
  - 实际项目过程中不断迭代和优化，最终由一个增强型的配置工具成为一个相对完备的游戏配置数据解决方案。


## 文档
* [主页](https://focus-creative-games.github.io/luban/index.html)
* 各语言的简介: [English](README.en-us.md), [简体中文](README.md)
* [特性](docs/traits.md)
* [使用说明](docs/catalog.md)


## 使用示例
  * Lua 使用示例  
    ``` Lua
    local data = require("TbDataFromJson")
    local cfg = data[32]
    print(cfg.name)
    ```
	
  * C# 使用示例
	``` C#
	// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
	var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes("output_data/" + file)));
	// 访问一个单例表
	Console.WriteLine(tables.TbSingleton.Name);
	// 访问普通的 key-value 表
	Console.WriteLine(tables.TbDataFromJson.Get(12).X1);
	// 访问 双键表
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
