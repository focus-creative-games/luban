---
name: luban-validator
description: Writes Luban field validators (ref, range, path, size, set, regex, non-default). Use when adding validation rules on types or Excel ##type cells.
---

# Luban: 校验器

校验在生成期执行。发布建议 `--strict`。

## 常用写法

| 能力 | 示例 |
|------|------|
| 非默认 | `int!`、`int?!` |
| 引用 | `int#ref=item.TbItem` |
| 引用可跳过 0 | `int#ref=item.TbItem?` |
| 范围 | `int#range=[1,100]` |
| 路径 | `string#path=unity` + `-x pathValidator.rootDir=...` |
| 集合大小 | `(list#size=4),int` |
| 允许值 | `int#set=1;2;3` |
| 正则 | `string#regex=^[a-z]+$` |

写在 XML `type="..."` 或 Excel `##type`。

## 步骤

1. 确认被引用表全名（可用 schema-json / MCP）。
2. 改字段类型字符串，保留原有 group/注释。
3. `dotnet Luban.dll ... -f --strict --errorFormat json` 验证。

## 注意

- 容器约束加在容器上：`(list#size=n),T`
- 可空为 null 时多数引用类校验会跳过
- 不要用削弱校验的方式「修好」坏数据
