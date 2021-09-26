
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
----

## 介绍

目前已经存在很多导表工具（如tabtoy、xls2json），它们功能多为excel文件到其他格式的转换工具及简单代码生成器，勉强满足中小类型项目的需求。
在中大型游戏项目中，基本都会有技能、行为树之类的复杂功能。这些功能有非常复杂的数据结构，往往使用自定义编辑器制作，并以json、xml等文件格式保存。就算常规的excel表，也经常出现复杂的数据结构需求。这些简单工具面对此类需求要么无法支持，要么就强迫策划和程序使用拆表等奇技淫巧，严重影响开发效率。

luban相较于常规的excel导表工具有以下核心优势：
- 增强了excel格式。可以比较简洁地excel配置**任意复杂**的数据，像子结构、结构列表，以及更复杂的深层次的嵌套结构都能直接解析处理。
- 完备的类型系统和多原始数据支持（xml、json、lua、yaml），可以轻松表达和解析**任意复杂**的数据。意味着传统excel导表工具无法处理的技能、行为树、副本等等复杂配置，luban也能够统一处理了，彻底将程序从复杂的配置解析中完全解放出来。
- 完善的工作流支持。如id的外键引用检查;资源合法性检查;灵活的数据源定义（拆表或者多表合一）;灵活的分组导出机制；多种本地化支持;生成极快（日常迭代300ms以内）等等。

====**如果觉得不错，烦请点个star，你的支持会给予我们巨大动力 ^_^**====

## 文档

- [主页](https://focus-creative-games.github.io/luban/index.html)
- [快速开始](docs/install.md)
- [client&server安装与使用说明](docs/luban_install_manual.md)
- [文档目录](docs/catalog.md)
- [[TODO] 完整手册](docs/manual.md)
- **====>强烈推荐查看：示例项目** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples)) **<====**

- 支持与联系
  - QQ群: 692890842 （Luban开发交流群）。有使用方面的疑问请及时加QQ群询问，随时有人帮助解决。
  - 邮箱: taojingjian#gmail.com
  - Skypy群: https://join.skype.com/xr2nhdMKjac0

## 特性
- 支持excel族、json、xml、lua、yaml 多种数据格式，基本统一了游戏常见的配置数据
  - [配置数据简介 - Excel](docs/data_excel.md)
  - [配置数据简介 - Json](docs/data_json.md)
  - [配置数据简介 - XML](docs/data_xml.md)
  - [配置数据简介 - YAML](docs/data_yaml.md)
  - [配置数据简介 - Lua ](docs/data_lua.md)
- **强大完备的类型系统**。**可以优雅表达任意复杂的数据结构**, 支持
  - 常见原生类型、
  - datetime类型、
  - 容器类型list,set,map、
  - 枚举和结构 
  - **多态结构**
  - **可空类型**
- 支持增强的 excel 格式。可以在excel里比较简洁填写出非常复杂的数据（比如顶层字段包含"list,A"类型字段， 而A是结构并且其中又包含"list,B"类型字段，B也是结构并且包含"list,C"这样的字段...）。
- 生成代码清晰易读、良好模块化。特地支持运行时原子性热更新配置。
- 生成极快。支持常规的本地缓存增量生成模式，也支持云生成模式。MMORPG这样大型项目也能秒内生成。日常增量生成基本在300ms以内，项目后期极大节省了迭代时间。另外支持**watch监测模式**，数据目录变化立即重新生成。
- 灵活的数据源定义。一个表可以来自多个文件或者一个文件内定义多个表或者一个目录下所有文件甚至来自云表格，以及以上的组合
- 支持表与字段级别分组。可以灵活定义分组，选择性地导出客户端或者服务器或编辑器所用的表及字段
- 多种导出数据格式支持。支持binary、json、lua、xml 等导出数据格式
- 强大灵活的定制能力
	- 支持代码模板，可以用自定义模板定制生成的代码格式
	- 支持数据模板，可以用模板文件定制导出格式。意味着可以在不改动现有程序代码的情况下，把luban当作**配置处理前端**，生成自定义格式的数据与自己项目的配置加载代码配合工作。开发已久的项目或者已经上线的老项目，也能从luban强大的数据处理工作流中获益
- 支持数据标签。 
  - 可以选择导出符合要求的数据，发布正式数据时策划不必手动注释掉那些测试数据了
- 强大的数据校验能力。
  - 支持内建数据格式检查
  - 支持ref表引用检查（策划不用担心填错id）
  - 支持path资源检查（策划不用担心填错资源路径）
  - 支持range检查
- 支持常量别名。策划不必再为诸如 升级丹 这样的道具手写具体道具id了
- 支持多种常见数据表模式。 
  - one(单例表)、
  - map（常规key-value表）
- 支持res资源标记。
  - 可以一键导出配置中引用的所有资源列表(icon,ui,assetbundle等等)
- 统一了自定义编辑器的配置数据。
  - 与Unity和UE4的自定义编辑器良好配合。
  - 为编辑器生成合适的加载与保存json配置的的c#(Unity)或c++(UE4)代码。
  - 保存的json配置能够被luban识别和处理。
- 支持emmylua anntations。
  - 生成的lua包含符合emmylua 格式anntations信息。
  - 配合emmylua有良好的配置代码提示能力
- 支持主流的游戏开发语言
   - c++ (11+)
   - c# (.net framework 4+. dotnet core 3+)
   - java (1.6+)
   - go (1.10+)
   - lua (5.1+)
   - js 和 typescript (3.0+)
   - python (3.0+)
   - erlang (18+)
- 支持主流引擎和平台
   - unity + c#
   - unity + [tolua](https://github.com/topameng/tolua)、[xlua](https://github.com/Tencent/xLua)
   - unity + [ILRuntime](https://github.com/Ourpalm/ILRuntime)
   - unity + [puerts](https://github.com/Tencent/puerts)
   - unity + [GameFramework](https://github.com/EllanJiang/GameFramework)
   - unity + [ET游戏框架](https://github.com/egametang/ET)
   - unreal + c++
   - unreal + [unlua](https://github.com/Tencent/UnLua)
   - unreal + [sluaunreal](https://github.com/Tencent/sluaunreal)
   - unreal + [puerts](https://github.com/Tencent/puerts)
   - cocos2d-x + lua
   - cocos2d-x + js
   - [skynet](https://github.com/cloudwu/skynet)
   - 微信小程序平台
   - 其他家基于js的小程序平台
   - 其他所有支持lua的引擎和平台
   - 其他所有支持js的引擎和平台

- 多分支 数据
  - 支持 main + patches的数据模式。
  - 在主版本数据基础上，提供一个补丁数据，合并处理后生成最终目标数据。
  - 适合制作海外有细节配置不同的多地区配置，不需要复制出主版本数据，接着在上面修改出最终数据。极大优化了制作本地化配置的工作流。

- [**本地化支持**](docs/export_localization.md)
 	- 支持时间本地化。datetime类型数据会根据指定的timezone，转换为目标地区该时刻的UTC时间，方便程序使用。
	- 支持文本静态本地化。导出时所有text类型数据正确替换为最终的本地化字符串。绝大多数的业务功能不再需要运行根据本地化id去查找文本的内容，简化程序员的工作。
	- 支持文本动态本地化。运行时动态切换所有text类型数据为目标本地化字符串。
	- 支持 main + patches 数据合并。在基础数据上，施加差分数据，生成最终完整数据，适用于制作有细微不同的多地区的配置数据。
	- [TODO] 【独创】 支持任意粒度和任意类型数据（如int,bean,list,map）的本地化。 

- 时间本地化
  - datetime类型数据在指定了本地化时区后，会根据目标时区，生成相应时刻的UTC时间，
  - 方便程序使用

- [导出模板支持](docs/export_format.md)


-----

## 路线图

- [ ] 新增 unity 内置编辑器
- [ ] 新增 unreal 内置编辑器
- [ ] 补充单元测试

## 开发环境架设

- 安装 [VS2019 社区版](https://visualstudio.microsoft.com/zh-hans/vs/)
- 安装 [.dotnet core sdk 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)

## 安装与使用

参见 [client&server安装与使用说明](docs/luban_install_manual.md)

## 如何贡献

- [Contributing](CONTRIBUTING.md) explains what kinds of changes we welcome
- [Workflow Instructions](docs/workflow/README.md) explains how to build and test

## 有用的链接

- [.NET Core source index](https://source.dot.net)

## License

Luban is licensed under the [MIT](LICENSE.TXT) license.
