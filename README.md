
- [README 中文](./README.md)
- [README English](./README_EN.md)

# Luban

![icon](docs/images/logo.png)

[![license](http://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT) ![star](https://img.shields.io/github/stars/focus-creative-games/luban?style=flat-square)


luban是一个强大、易用、优雅、稳定的游戏配置解决方案。它设计目标为满足从小型到超大型游戏项目的简单到复杂的游戏配置工作流需求。

luban可以处理丰富的文件类型，支持主流的语言，可以生成多种导出格式，支持丰富的数据检验功能，具有良好的跨平台能力，并且生成极快。
luban有清晰优雅的生成管线设计，支持良好的模块化和插件化，方便开发者进行二次开发。开发者很容易就能将luban适配到自己的配置格式，定制出满足项目要求的强大的配置工具。

luban标准化了游戏配置开发工作流，可以极大提升策划和程序的工作效率。

## 核心特性

- 丰富的源数据格式。支持excel族(csv,xls,xlsx,xlsm)、json、xml、yaml、lua等
- 丰富的导出格式。 支持生成binary、json、bson、xml、lua、yaml等格式数据
- 增强的excel格式。可以简洁地配置出像简单列表、子结构、结构列表，以及任意复杂的深层次的嵌套结构
- 完备的类型系统。不仅能表达常见的规范行列表，由于**支持OOP类型继承**，能灵活优雅表达行为树、技能、剧情、副本之类复杂GamePlay数据
- 支持多种的语言。支持生成c#、java、go、cpp、lua、python、typescript 等语言代码
- 支持主流的消息方案。 protobuf(schema + binary + json)、flatbuffers(schema + json)、msgpack(binary)
- 强大的数据校验能力。ref引用检查、path资源路径、range范围检查等等
- 完善的本地化支持
- 支持所有主流的游戏引擎和平台。支持Unity、Unreal、Cocos2x、Godot、微信小游戏等
- 良好的跨平台能力。能在Win,Linux,Mac平台良好运行。
- 支持所有主流的热更新方案。hybridclr、ilruntime、{x,t,s}lua、puerts等
- 清晰优雅的生成管线，很容易在luban基础上进行二次开发，定制出适合自己项目风格的配置工具。

## 文档

- [官方文档](https://luban.doc.code-philosophy.com/)
- [快速上手](https://luban.doc.code-philosophy.com/docs/beginner/quickstart)
- **示例项目** ([github](https://github.com/focus-creative-games/luban_examples)) ([gitee](https://gitee.com/focus-creative-games/luban_examples))
- 支持与联系
  - QQ群: 692890842 （Luban开发交流群）
  - discord: https://discord.gg/dGY4zzGMJ4
  - 邮箱: luban@code-philosophy.com

## Excel格式概览

基础数据格式

![primitive_type](docs/images/cases/primitive_type.jpg)

enum 数据格式

![enum](docs/images/cases/enum.jpg)

bean数据格式

![bean](docs/images/cases/bean.jpg)

多态bean数据格式

![bean](docs/images/cases/bean2.jpg)

容器

![collection](docs/images/cases/collection.jpg)

可空类型

![nullable](docs/images/cases/nullable.jpg)

无主键表

![table_list_not_key](docs/images/cases/table_list_not_key.jpg)

多主键表（联合索引）

![table_list_union_key](docs/images/cases/table_list_union_key.jpg)

多主键表（独立索引）

![table_list_indep_key](docs/images/cases/table_list_indep_key.jpg)

单例表

有一些配置全局只有一份，比如 公会模块的开启等级，背包初始大小，背包上限。此时使用单例表来配置这些数据比较合适。

![singleton](docs/images/cases/singleton2.jpg)

纵表

大多数表都是横表，即一行一个记录。有些表，比如单例表，如果纵着填，一行一个字段，会比较舒服。A1为##column表示使用纵表模式。 上面的单例表，以纵表模式填如下。

![singleton](docs/images/cases/singleton.jpg)

使用sep读入bean及嵌套bean。

![sep_bean](docs/images/cases/sep_bean.jpg)

使用sep读取普通容器。

![sep_bean](docs/images/cases/sep_container1.jpg)

使用sep读取结构容器。

![sep_bean](docs/images/cases/sep_container2.jpg)


多级标题头

![colloumlimit](docs/images/cases/multileveltitle.jpg)

限定列格式

![titlelimit](docs/images/cases/titlelimit.jpg)

枚举的列限定格式

![titlle_enum](docs/images/cases/title_enum.jpg)

多态bean列限定格式

![title_dynamic_bean](docs/images/cases/title_dynamic_bean.jpg)

map的列限定格式

![title_map](docs/images/cases/title_map.jpg)


多行字段

![map](docs/images/cases/multiline.jpg)

数据标签过滤

![tag](docs/images/cases/tag.jpg)


## 其他格式概览

以行为树为例，展示json格式下如何配置行为树配置。xml、lua、yaml等格式请参见 [详细文档](https://luban.doc.code-philosophy.com/docs/intro)。

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

```cpp
    cfg::Tables tables;
    if (!tables.load([](ByteBuf& buf, const std::string& s) { return buf.loadFromFile("../GenerateDatas/bytes/" + s + ".bytes"); }))
    {
        std::cout << "== load fail == " << std::endl;
        return;
    }
    std::cout << tables.TbGlobal->name << std::endl;
    std::cout << tables.TbItem.get(12)->name << std::endl;
```


## license

Luban is licensed under the [MIT](https://github.com/focus-creative-games/luban/blob/main/LICENSE) license
