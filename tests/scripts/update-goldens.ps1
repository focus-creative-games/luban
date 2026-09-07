#!/usr/bin/env pwsh
# Regenerates tests/golden from tests/fixtures using the built Luban CLI.
# Usage: pwsh tests/scripts/update-goldens.ps1

$ErrorActionPreference = "Stop"
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
Set-Location (Join-Path $repoRoot "src")

dotnet build Luban\Luban.csproj -c Release | Out-Host
$luban = Join-Path $repoRoot "src\Luban\bin\Release\net8.0\Luban.dll"
$fixtures = Join-Path $repoRoot "tests\fixtures"
$golden = Join-Path $repoRoot "tests\golden"

function Invoke-Gen([string]$Fixture, [string[]]$DataTargets, [string]$OutRel, [string[]]$Extra = @()) {
    $out = Join-Path $golden ($OutRel -replace '/', [IO.Path]::DirectorySeparatorChar)
    if (Test-Path $out) { Remove-Item $out -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $out | Out-Null
    $conf = Join-Path (Join-Path $fixtures $Fixture) "luban.conf"
    $argList = @("-t", "server", "--conf", $conf, "-x", "outputDataDir=$out")
    foreach ($d in $DataTargets) { $argList += @("-d", $d) }
    $argList += $Extra
    & dotnet $luban @argList
    if ($LASTEXITCODE -ne 0) { throw "Luban failed for $Fixture -> $OutRel" }
    Write-Host "OK $OutRel"
}

Invoke-Gen "smoke" @("json") "smoke/json"
$coreTargets = @(
    "json","json2","xml","yaml","lua","bin","bin-offset","bson","msgpack",
    "protobuf2-json","protobuf3-json","flatbuffers-json","json-convert",
    "protobuf2-bin","protobuf3-bin"
)
foreach ($t in $coreTargets) {
    Invoke-Gen "core" @($t) "core/$t"
}

$texts = Join-Path $fixtures "l10n\Data\texts.json"
Invoke-Gen "l10n" @("json","text-list") "l10n/default" @(
    "-x", "l10n.provider=default",
    "-x", "l10n.textFile.path=*@$texts",
    "-x", "l10n.textFile.keyFieldName=key",
    "-x", "l10n.textListFile=texts.txt",
    "-x", "outputSaver.json.cleanUpOutputDir=false",
    "-x", "outputSaver.text-list.cleanUpOutputDir=false"
)

Write-Host "Golden refresh complete."
