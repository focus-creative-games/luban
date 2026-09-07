---
name: luban-runtime-load
description: Guides Luban runtime Tables loading patterns for C#/other languages. Use when integrating generated code, choosing loader, or hot-reload/testing config load.
---

# Luban: 运行时加载

## 推荐形态

```csharp
var tables = new cfg.Tables(loader);
var item = tables.TbItem.Get(1001);
```

- 一个 `Tables` 聚合所有表。
- 避免每张表静态全局单例（热更/测试困难）。

## Agent 检查点

1. `-c` 与 `-d` 格式匹配（如 `cs-bin`+`bin`，`cs-simple-json`+`json`）。
2. `topModule` / 命名空间与工程一致。
3. loader 指向实际数据目录；Unity 注意 StreamingAssets/Addressables 约定。
4. 客户端不要加载仅 `s` group 的字段/表。

## 参考

- 运行时加载、生成目标、code-style 文档
- 示例：`luban_examples/Projects`
