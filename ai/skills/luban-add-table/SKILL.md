---
name: luban-add-table
description: Adds a new Luban config table (Excel + __tables__ registration + regenerate). Use when creating a new table, registering reward/item tables, or when the user asks to add a Luban table.
---

# Luban: 加一张表

## 规则

- Schema 是契约：先登记表，再填数据。
- Excel sheet 仅当 A1 以 `##` 开头才被识别。
- `read_schema_from_file=true` 时不要在 `__beans__` 重复定义同名 bean。

## 步骤

1. 在 `dataDir`（常为 `Data`）新建 `xxx.xlsx`/`csv`：
   - `##var`：字段名
   - `##type`：类型（如 `int` / `string`）
   - 可选 `##group`：`c`/`s`
   - 其后为数据行
2. 在 `Data/__tables__.xlsx`（或 XML schema）增加一行：
   - `full_name`：如 `TbReward`
   - `value_type`：如 `Reward`
   - `read_schema_from_file`：`true`（从表头推断字段）时常用
   - `input`：相对 dataDir 的文件名
   - `index`：主键字段（map 表）
3. 运行项目 `gen.bat` / `gen.sh`，或：

```bash
dotnet Luban.dll --conf luban.conf -t all -d json -x outputDataDir=output
```

4. 确认输出出现新表；若有 `-c`，确认生成了对应类型。

## 常见失败

| 现象 | 处理 |
|------|------|
| 新文件无输出 | 是否写入 `__tables__` |
| sheet 被忽略 | A1 是否 `##` 开头 |
| bean 冲突 | 关闭重复的 `__beans__` 定义 |

## 参考

- 文档：加一张表、Excel 基础、Excel Schema
- 需要查现有表时用 MCP `ListTables` / `GetSchema`
