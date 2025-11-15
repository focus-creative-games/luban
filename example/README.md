# Luban example project (XML schema + JSON data)

This minimal, runnable example shows how to set up a Luban data project from first principles and run the full pipeline:
- Schema loading (XML)
- Data loading (JSON array)
- Code generation (example command uses C# SimpleJSON)
- Data export (JSON)

Files:
- Schema
  - [`Defines/enums.xml`](Defines/enums.xml)
  - [`Defines/beans.xml`](Defines/beans.xml)
  - [`Defines/tables.xml`](Defines/tables.xml)
- Data
  - [`Datas/item.json`](Datas/item.json)
- Config
  - [`luban.conf`](luban.conf)

Outputs (configured via xargs):
- Code: `example/Gen/Code`
- Data: `example/Gen/Data`


## Concepts recap (how Luban works)

- Schema layer (Def): compiled from raw schema files into DefTable/DefBean/DefEnum and then converted to type descriptors (TType).
- Data layer (D): actual values loaded from sources as DType (DBean, DList, etc.).
- Pipeline: Schema → Context → Code/Data targets (parallel).
- Table modes:
  - ONE (singleton)
  - MAP (keyed by index, emits object/dict)
  - LIST (array)

This example uses:
- Enum `cfg.Quality` with 3 values (Common/Rare/Epic)
- Bean `cfg.Item`
- Table `cfg.TbItem` (mode=map, index=id)
- Data in JSON array format where enum fields use item names (e.g., `"quality": "Common"`)


## Project layout

```
example/
├─ Defines/
│  ├─ enums.xml      # DefEnum
│  ├─ beans.xml      # DefBean
│  └─ tables.xml     # DefTable
├─ Datas/
│  └─ item.json      # Table data (JSON array)
├─ Gen/
│  ├─ Code/          # Generated code output
│  └─ Data/          # Exported data output
└─ luban.conf        # Luban configuration
```

Key points in schema:
- [`Defines/enums.xml`](Defines/enums.xml): defines `cfg.Quality` = {Common=1, Rare=2, Epic=3}
- [`Defines/beans.xml`](Defines/beans.xml): defines `cfg.Item` with fields
  - id:int (index)
  - name:string
  - quality:cfg.Quality (enum)
  - price:int
  - tags:list,string
- [`Defines/tables.xml`](Defines/tables.xml): defines `cfg.TbItem` (mode=map, index=id) reading `Datas/item.json`


## Data format

[`Datas/item.json`](Datas/item.json) is a JSON array (LIST in file), but table is MAP by index=id. Luban will read the array and build a keyed object in the exported data (for json2).

Example record:
```json
{ "id": 101, "name": "Potion", "quality": "Common", "price": 50, "tags": ["consumable", "healing"] }
```

Enum values accept the enum item name or alias per schema; here names are used.


## Configuration

[`luban.conf`](luban.conf) wires everything:
- schemaFiles → XML loaders
- dataDir → `Datas`
- targets → `client`, `server` (groups)
- xargs:
  - `-x outputCodeDir=example/Gen/Code`
  - `-x outputDataDir=example/Gen/Data`
  - `-x codeStyle=pascal`
  - `-x namingConvention=default`

You choose code/data targets at run time (CLI flags below).


## Running the example

Prerequisites:
- .NET 8 SDK (or compatible with the Luban you build/run)
- This repo checked out (you are in the repo root)

Build (optional but recommended):
```bash
dotnet build ./src/Luban/Luban.csproj -c Release
```

Run pipeline (client target, generate C# SimpleJSON code, export JSON):
```bash
dotnet run --project ./src/Luban/Luban.csproj -- \
  --conf ./example/luban.conf \
  --target client \
  --codeTarget cs-simple-json \
  --dataTarget json2
```

Notes:
- Flags are long-form for clarity and commonly supported by Luban:
  - `--conf` (aka `-c`): config file
  - `--target`: one of targets in luban.conf (client/server), controls group filtering
  - `--codeTarget`: code generator (e.g., cs-simple-json, cs-dotnet-json, typescript-json, go-json, rust-json, etc.)
  - `--dataTarget`: data exporter (e.g., json2, lua, xml, bin/msgpack/protobuf/flatbuffers variants)
- On Windows PowerShell, the same command works; ensure the `--` separator is present after `dotnet run --project`.

After success:
- Generated code → `example/Gen/Code`
- Exported data → `example/Gen/Data`
  - For json2, `TbItem.json` (object keyed by id):
    ```json
    {
      "101": { "id": 101, "name": "Potion", "quality": 1, "price": 50, "tags": ["consumable","healing"] },
      "102": { "id": 102, "name": "Iron Sword", "quality": 2, "price": 1200, "tags": ["weapon","melee"] },
      "103": { "id": 103, "name": "Wizard Staff", "quality": 3, "price": 3200, "tags": ["weapon","magic"] }
    }
    ```
    Note: json2 materializes enum to number by default. Some targets can emit names; pick target per runtime needs.


## Switching targets

- Only export data (no code): omit `--codeTarget`, keep `--dataTarget json2`.
- Different code language: choose another `--codeTarget`:
  - `typescript-json`, `go-json`, `rust-json`, `java-json`, etc.
- Another data format: choose another `--dataTarget`:
  - `lua`, `xml`, `bin` (binary), `msgpack`, `protobuf-json`/`flatbuffers-json` (requires their schema/code flows).


## Extending

- Add validations: use field tags (e.g., range/ref/path) in bean fields; then re-run. Built-in validators run after data load.
- Add a second table with `ref` to `TbItem` (e.g., a drop table) and run reference resolution in generated code.
- Switch groups: tables/fields can have group tags; `--target` selects which to include.


## Troubleshooting

- CLI flags vary between Luban versions. If `--conf/--target/--codeTarget/--dataTarget` are not recognized, try short forms (`-c`, `-t`) or consult the repo’s README.
- Ensure data enum names match defined enum items (case-sensitive by default).
- For Windows paths, prefer forward slashes or quote paths with spaces.


## Principle summary (why this works)

- Schema System: XML definitions compiled into `DefAssembly` of DefBean/DefEnum/DefTable → TType descriptors.
- Data Loading: JSON array parsed to DType (DBean/list), validated against schema (types, enum names).
- Aggregation: Table mode MAP uses `index="id"` to emit key→record structures.
- Code Gen: A `CodeTarget` traverses TTypes with template visitors to produce language-native code.
- Data Export: A `DataTarget` traverses DTypes to serialize (json2/lua/xml/bin/...).
- Groups and Targets: `--target client/server` filters by groups declared in schema/config.