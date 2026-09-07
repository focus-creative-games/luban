---
name: luban-generate-debug
description: Diagnoses Luban generation and validation failures. Use when gen.bat fails, schema/data errors appear, or the user pastes Luban logs / --errorFormat json reports.
---

# Luban: 生成失败排查

## 优先动作

1. 用同一命令加 `--errorFormat json`，解析 `errors[]`。
2. 看 `category`：`schema` / `data` / `validation` / `codegen` / `cli`。
3. 有 `file` + `location` + `fieldPath` 时先修对应单元格/字段。

## 排查顺序

1. **CLI**：`--conf` 路径、`-t` 是否存在、`-c`/`-d` 是否匹配。
2. **Schema**：目标 group、表 value_type、继承关系。
3. **Data**：类型解析、枚举名、必填、分隔符。
4. **Validation**：`ref` / `range` / `path`（path 需 `pathValidator.rootDir`）。
5. **Codegen**：关键字冲突、非法标识符。

## 有用命令

```bash
# 只校验（推荐 Agent CLI）
dotnet Luban.Agent.dll validate --conf luban.conf -t all

# 或主 CLI
dotnet Luban.dll --conf luban.conf -t all -f --strict --errorFormat json -x outputSaver=null

# 导出 / 查询 schema
dotnet Luban.Agent.dll schema --conf luban.conf -t all
dotnet Luban.dll --conf luban.conf -t all -c schema-json -x outputCodeDir=./schema-out
```

## 常见坑

- 未登记 `__tables__`
- Sheet 无 `##` 头
- 引用 ID 不存在（`#ref=`）
- 输出目录被清掉了手写文件（目录选错）

## 原则

修好数据或按程序意图改 schema；禁止为通过生成而削弱校验（除非用户明确要求且说明风险）。
