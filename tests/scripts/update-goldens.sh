#!/usr/bin/env bash
# Regenerates tests/golden from tests/fixtures using the built Luban CLI.
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
cd "$ROOT/src"
dotnet build Luban/Luban.csproj -c Release
LUBAN="$ROOT/src/Luban/bin/Release/net8.0/Luban.dll"
FIXTURES="$ROOT/tests/fixtures"
GOLDEN="$ROOT/tests/golden"

gen() {
  local fixture="$1"; shift
  local out_rel="$1"; shift
  local out="$GOLDEN/$out_rel"
  rm -rf "$out"
  mkdir -p "$out"
  local conf="$FIXTURES/$fixture/luban.conf"
  dotnet "$LUBAN" -t server --conf "$conf" -x "outputDataDir=$out" "$@"
  echo "OK $out_rel"
}

gen smoke "smoke/json" -d json

for t in json json2 xml yaml lua bin bin-offset bson msgpack \
         protobuf2-json protobuf3-json flatbuffers-json json-convert \
         protobuf2-bin protobuf3-bin; do
  gen core "core/$t" -d "$t"
done

TEXTS="$FIXTURES/l10n/Data/texts.json"
gen l10n "l10n/default" -d json -d text-list \
  -x "l10n.provider=default" \
  -x "l10n.textFile.path=*@$TEXTS" \
  -x "l10n.textFile.keyFieldName=key" \
  -x "l10n.textListFile=texts.txt" \
  -x "outputSaver.json.cleanUpOutputDir=false" \
  -x "outputSaver.text-list.cleanUpOutputDir=false"

echo "Golden refresh complete."
