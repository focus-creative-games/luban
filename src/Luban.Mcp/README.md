# Luban.Mcp

stdio MCP Server for Luban: list/get schema, validate, generate, search docs.

## Build

Preferred (with sibling `luban_examples`):

```bash
# Windows
luban_examples/Tools/build-luban.bat

# macOS / Linux
luban_examples/Tools/build-luban.sh
```

Outputs under `luban_examples/Tools/`:

- `Luban/` — main generator
- `Luban.Agent/` — agent CLI
- `Luban.Mcp/` — this MCP server

Or from the luban repo:

```bash
dotnet build src/Luban/Luban.csproj -c Release -o <out>/Luban
dotnet build src/Luban.Agent/Luban.Agent.csproj -c Release -o <out>/Luban.Agent
dotnet build src/Luban.Mcp/Luban.Mcp.csproj -c Release -o <out>/Luban.Mcp
```

## Env

- `LUBAN_AGENT_DLL` — path to `Luban.Agent.dll`
- `LUBAN_DLL` — path to `Luban.dll`
- `LUBAN_DOC` — path to `luban-doc/docs`

## Tools

- `ListTables` / `GetSchema` / `Describe` / `Validate` / `Generate` / `SearchDocs`

See documentation: `docs/ai/mcp` in luban-doc.
