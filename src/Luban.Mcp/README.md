# Luban.Mcp

stdio MCP Server for Luban: list/get schema, validate, generate, search docs.

## Build

```bash
dotnet build src/Luban.Mcp/Luban.Mcp.csproj -c Release
dotnet build src/Luban/Luban.csproj -c Release
```

## Env

- `LUBAN_DLL` — path to `Luban.dll`
- `LUBAN_DOC` — path to `luban-doc/docs`

## Tools

- `ListTables` / `GetSchema` / `Validate` / `Generate` / `SearchDocs`

See documentation: `docs/ai/mcp` in luban-doc.
