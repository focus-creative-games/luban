---
name: luban-schema-design
description: Designs Luban schema (beans, enums, polymorphism, collections, groups). Use when modeling config structures, inheritance, nested beans, or choosing Excel vs XML schema.
---

# Luban: Schema 设计

## 原则

- 程序维护 Schema；策划填 Data。
- 复杂 GamePlay（技能/行为树）优先 OOP 继承/多态，而不是塞字符串。
- 客户端敏感字段用 `s` group，不要泄漏到 `c`。

## 选型

| 需求 | 建议 |
|------|------|
| 扁平行表 | Excel + `read_schema_from_file` |
| 多模块/复用 bean | XML `Defines/*.xml` 或 `__beans__` |
| 多态配置 | 抽象 bean + 子类；Excel 填类型名/别名 |
| 一对多嵌套 | `list,Bean` / 多行 / sep 紧凑格式 |

## 类型要点

- 容器：`list,T` / `map,K,V`；**元素不可写 `list,int?`**
- 可空：`T?`；多态非空必须给具体子类
- 引用：`int#ref=module.TbX`
- 字段名建议 `snake_case`，生成时按语言转风格

## 检查清单

1. 主键与 `mode`（map/list/one）是否匹配
2. group 是否覆盖 client/server 需求
3. 多态子类是否都已定义且可区分
4. 用 `-c schema-json` 或 MCP `GetSchema` 复核结构

## 参考

- 类型系统、XML Schema、多态、分组 targets
