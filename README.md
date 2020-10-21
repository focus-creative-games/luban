[//]: # (Author: bug)
[//]: # (Date: 2020-10-20 20:24:07)

# Gen

## 什么是 Gen

Gen 是一个强大的生成与缓存工具，用于但不限于 游戏配置、消息、资源格式转换 之类的生成。

相比传统简单的以excel为中心的表格导出工具，它提供了一个**完整的游戏配置数据解决方案**，有以下功能：
>
> * 数据定义
> * 数据编辑
> * 数据导出
> * 前后端代码生成
> * 本地化  
> * 编辑器数据load&save代码生成

Gen能够良好满足小型、中型、大型及超大型游戏项目的配置需求。
    
Gen 工具不仅适用于游戏行业，也非常适合传统的互联网项目。


## 文档
* [主页](https://focus-creative-games.github.io/bright_gen/index.html)
* 各语言的简介: [English](README.en-us.md), [简体中文](README.md)

## 使用示例
  * Lua 使用示例  
    ``` Lua
    local data = require("TbDataFromJson")
    local cfg = data[32]
    print(cfg.name)
    ```

  * [更多语言的例子](docs/samples.md)
  
## 特性
  * [完备的数据类型支持](docs/feature.md#支持的数据类型)
  * [多类型数据源支持](docs/feature.md#多数据源支持)
  * [多种数据表模式](docs/feature.md#多种数据表模式)
  * [按组导出数据](docs/feature.md#如何自定义导出分组)
  * [生成速度快](docs/feature.md#生成极快)
  * [增强 Excel 的表达](docs/feature.md#增强的-excel-格式)
  * [代码提示支持](docs/feature.md#代码编辑器支持)
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
- [x] 支持 python  

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

Birght Gen is licensed under the [MIT](LICENSE.TXT) license.