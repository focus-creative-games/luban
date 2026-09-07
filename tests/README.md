# Luban test suite

Self-contained fixtures and goldens for `src/Luban.Tests`.

## Layout

- `fixtures/` — input schemas and source data (prefer text formats)
- `golden/` — expected data-target outputs
- `scripts/update-goldens.ps1` / `update-goldens.sh` — regenerate goldens via Luban CLI

## Run

```bash
dotnet test src/Luban.Tests/Luban.Tests.csproj
```
