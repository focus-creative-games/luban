---
name: luban-excel-fill
description: Explains and fills Luban Excel config tables by header conventions. Use when designers fill sheets, ##var/##type/##group rows, nested cells, or polymorphism columns.
---

# Luban: Excel 填表

## 最小约定

| 行 | 含义 |
|----|------|
| `##var` / `##` | 字段名 |
| `##type` | 类型 |
| `##group` | `c`/`s`/`e`；空=跟随默认 |
| `##` 注释行 | 中文说明，不进逻辑 |
| `#` 开头列名 | 注释列，不导出 |

- A1 必须以 `##` 开头，否则整张 sheet 忽略。
- 空字符串填 `""`（按项目约定）。
- 枚举可填名字或 alias。

## 分组提醒

- `c`：客户端可见
- `s`：仅服务器
- 填错会导致缺字段或敏感数据下发

## 复杂结构

- 嵌套 / 列表：遵循程序给定的分列或多行样表，不要自创分隔符
- 多态：按样表填具体类型名；拿不准先问程序或查 schema-json

## Agent 行为

1. 改数值前确认列的 `##type` 与 group。
2. 不擅自改 `##type` / 主键列语义。
3. 生成失败时保留行列信息，用 `luban-generate-debug` 排查。

## 参考

- 策划概念、表头、复杂结构、Excel 嵌套
