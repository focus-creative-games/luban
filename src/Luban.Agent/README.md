# Luban.Agent

Agent-oriented CLI for Luban: validate, export/query schema, list/describe types.

Keeps the main `Luban` tool focused on generation. This host always prints a single
`AgentResult` JSON document on **stdout**.

## Build

```bash
dotnet build src/Luban.Agent/Luban.Agent.csproj -c Release
```

## Usage

```bash
dotnet Luban.Agent.dll capabilities
dotnet Luban.Agent.dll list-tables --conf path/to/luban.conf -t all
dotnet Luban.Agent.dll describe --conf path/to/luban.conf -t all --name TbItem
dotnet Luban.Agent.dll schema --conf path/to/luban.conf -t all
dotnet Luban.Agent.dll validate --conf path/to/luban.conf -t all
```

First positional argument is the mode (also accepted as `--mode`).

## Modes

| Mode | Purpose |
|------|---------|
| `capabilities` | Machine-readable feature list |
| `list-tables` | Table summaries |
| `describe` | One table/bean/enum (`--name` required) |
| `schema` | Full or filtered schema JSON |
| `validate` | Load + validate data, no file output |

## Exit codes

| Code | Meaning |
|------|---------|
| 0 | OK |
| 1 | Data / validation |
| 2 | Schema / config |
| 3 | Usage |
| 4 | Internal |

See also: `Luban.Mcp` (IDE MCP) and main `Luban` (`--errorFormat json` for generate).
