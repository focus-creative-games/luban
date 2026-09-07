# Luban AI 资源

本目录提供官方 Agent Skills，与文档站「AI 支持」章节配套。

## 内容

- `skills/`：可复制到 `.cursor/skills/` 的 Skill 包
- 源码侧能力：
  - `-c schema-json`：导出机器可读 schema
  - `--errorFormat json`：可解析报错
  - `src/Luban.Mcp`：MCP Server（查 schema / 校验生成 / 搜文档）

## 文档

见 luban-doc：`docs/ai/`，以及站点 `/llms.txt`。

## 示例工程模板

`luban_examples/MiniTemplate` 与 `DataTables` 含 `AGENTS.md` 与 `.cursor/rules`，可复制到自有项目。
