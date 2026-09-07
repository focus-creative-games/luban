# Luban

![icon](docs/images/logo.png)

[![license](http://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT) ![star](https://img.shields.io/github/stars/focus-creative-games/luban?style=flat-square)

**中文** | [English](./README_EN.md)

luban是一个强大、易用、优雅、稳定的游戏配置解决方案。它设计目标为满足从小型到超大型游戏项目的简单到复杂的游戏配置工作流需求。

luban可以处理丰富的文件类型，支持主流的语言，可以生成多种导出格式，支持丰富的数据检验功能，具有良好的跨平台能力，并且生成极快。
luban有清晰优雅的生成管线设计，支持良好的模块化和插件化，方便开发者进行二次开发。开发者很容易就能将luban适配到自己的配置格式，定制出满足项目要求的强大的配置工具。

luban标准化了游戏配置开发工作流，可以极大提升策划和程序的工作效率。

自 **5.0.0** 起，Luban 面向 AI Agent / IDE 提供 **AI Native** 能力：官方 Skills、机器可读 schema 与报错、`Luban.Agent` CLI，以及 MCP Server，让加表、查 schema、校验与生成可以稳定地交给 Agent 完成。

## 核心特性

- 丰富的源数据格式。支持excel族(csv,tsv,xls,xlsx,xlsm)、json、xml、yaml、lua等
- 丰富的导出格式。 支持生成binary、json、bson、xml、lua、yaml等格式数据
- 增强的excel格式。可以简洁地配置出像简单列表、子结构、结构列表，以及任意复杂的深层次的嵌套结构
- 完备的类型系统。不仅能表达常见的规范行列表，由于**支持OOP类型继承**，能灵活优雅表达行为树、技能、剧情、副本之类复杂GamePlay数据
- 支持多种的语言。内置支持生成c#、java、go、cpp、lua、python、javascript、typescript、rust、php、erlang、godot 等语言代码，同时还能通过protobuf之类消息方案支持其他语言
- 支持主流的消息方案。 protobuf(schema + binary + json)、flatbuffers(schema + json)、msgpack(binary)
- 强大的数据校验能力。ref引用检查、path资源路径、range范围检查等等
- 完善的本地化支持
- **AI Native（5.0.0+）**。官方 Agent Skills、`-c schema-json`、`--errorFormat json`、`Luban.Agent`（校验 / 查表 / describe）、`Luban.Mcp`（IDE 工具调用）
- 支持所有主流的游戏引擎和平台。支持Unity、Unreal、Cocos2x、Godot、微信小游戏等
- 良好的跨平台能力。能在Win,Linux,Mac平台良好运行。
- 支持所有主流的热更新方案。hybridclr、ilruntime、{x,t,s}lua、puerts等
- 生成的代码未调用任何反射接口，兼容[Obfuz](https://github.com/focus-creative-games/obfuz)、Obfuscator、Confuser、.Net Refactor等常见代码混淆和加固工具。
- 清晰优雅的生成管线，很容易在luban基础上进行二次开发，定制出适合自己项目风格的配置工具。

## 文档

- [官方文档](https://www.datable.cn/docs/intro)
- [快速上手](https://www.datable.cn/docs/guide/install)
- [AI 支持（Skills / Agent / MCP / schema-json）](https://www.datable.cn/docs/ai/overview)
- **示例项目** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples))

仓库内 AI 资源见 [`ai/`](./ai/README.md)。

## 开发者：运行测试

主仓自带 xUnit 集成测试（不依赖外部 `luban_examples`）：

```bash
dotnet test src/Luban.Tests/Luban.Tests.csproj
```

- 夹具与期望输出：`tests/fixtures/`、`tests/golden/`
- 刷新 golden（改导出行为后）：`pwsh tests/scripts/update-goldens.ps1` 或 `bash tests/scripts/update-goldens.sh`
- 也可在跑测试时设置环境变量 `LUBAN_UPDATE_GOLDEN=1` 就地更新 golden

## 支持与联系

- QQ群: 692890842 （Luban开发交流群）
- discord: <https://discord.gg/dGY4zzGMJ4>
- 邮箱: <luban@code-philosophy.com>

## license

Luban is licensed under the [MIT](https://github.com/focus-creative-games/luban/blob/main/LICENSE) license
