# Copies Release 2027 build outputs into paths expected by REXJ_Installer.iss.
param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot),
    [string]$Config = "Release 2027",
    [string]$Platform = "x64",
    [string]$FallbackReleased = ""
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path (Join-Path $Root "REXJ_Installer.iss"))) {
    throw "Root must be the REXJ-Standalone folder (REXJ_Installer.iss not found). Root=$Root"
}

if ($FallbackReleased -and -not (Test-Path $FallbackReleased)) {
    throw "FallbackReleased path does not exist: $FallbackReleased"
}

$releasedRoot = Join-Path $Root "Released"

function Get-ToolSourceDir {
    param([string]$ToolFolder)

    $bin = Join-Path $Root "$ToolFolder\bin\$Platform\$Config"
    if (Test-Path $bin) {
        return $bin
    }

    if ($FallbackReleased) {
        $fallback = Join-Path $FallbackReleased $ToolFolder
        if (Test-Path $fallback) {
            Write-Host "  Using fallback payload for ${ToolFolder}: $fallback"
            return $fallback
        }
    }

    throw "No build output for '$ToolFolder'. Expected: $bin"
}

function Sync-ToolPayload {
    param(
        [string]$ToolFolder,
        [string[]]$DestDirs
    )

    $source = Get-ToolSourceDir -ToolFolder $ToolFolder
    $addins = Get-ChildItem (Join-Path $Root $ToolFolder) -Filter "*.addin" -File -ErrorAction SilentlyContinue

    foreach ($dest in $DestDirs) {
        if (Test-Path $dest) {
            Remove-Item -Path $dest -Recurse -Force
        }
        New-Item -ItemType Directory -Force -Path $dest | Out-Null
        Copy-Item -Path "$source\*" -Destination $dest -Recurse -Force
        foreach ($addin in $addins) {
            Copy-Item -Path $addin.FullName -Destination $dest -Force
        }
        Write-Host "  Staged: $ToolFolder -> $dest"
    }
}

$centralTools = @(
    "AreaSchedule", "AveSiteLevelHeightCalc", "CheckingALVS", "ExportExcel", "ExportSchedule",
    "ImportExcel", "ExcelImageInsert", "AutomaticFloor", "LocateSlab", "FittingSchedule"
)
foreach ($tool in $centralTools) {
    Sync-ToolPayload -ToolFolder $tool -DestDirs @(Join-Path $releasedRoot $tool)
}

$toolReleased = @(
    "LevelFilter", "ParameterFilter", "AutoCreateDimension", "GridDimension", "FloorHeightDimension",
    "ViewDuplicate", "SheetLayout", "TenkaiView", "EnhancedSectionBox", "AutoCreateRoomView",
    "SwitchJoinOrder", "JoinOrderInspector", "AutoLayoutTag", "LayoutInstanceInRegion",
    "Fukashi", "MappingTable", "STBLink", "ValueCopy", "PrintRegion"
)
foreach ($tool in $toolReleased) {
    Sync-ToolPayload -ToolFolder $tool -DestDirs @(Join-Path $Root "$tool\Released")
}

$releasedSubdirs = @(
    "RSTExtension", "SectionListRC", "SectionListSteel", "MEPConnectTool", "MepManholeTool",
    "MepDuctPipeTool", "MepVerticalMark", "MEPExtension", "PipeSizing", "Quantity", "REXJManager"
)
foreach ($tool in $releasedSubdirs) {
    Sync-ToolPayload -ToolFolder $tool -DestDirs @(Join-Path $releasedRoot $tool)
}

# Copy root-level .addin manifests referenced from Released\ in the installer script.
$rootAddinTools = @(
    @{ Tool = "SectionListRC";    Source = "SectionListRC.addin";    Dest = "SectionListRC.addin" },
    @{ Tool = "SectionListSteel"; Source = "SectionListSteel.addin"; Dest = "SectionListSteel.addin" },
    @{ Tool = "MEPConnectTool";   Source = "MEPConnectTool.addin";   Dest = "MEPConnectTool.addin" },
    @{ Tool = "MepManholeTool";   Source = "MepManholeTool.addin";   Dest = "MepManholeTool.addin" },
    @{ Tool = "MepDuctPipeTool";  Source = "MepDuctPipeTool.addin";  Dest = "MepDuctPipeTool.addin" },
    @{ Tool = "MepVerticalMark";  Source = "ADSK.MepVerticalMark.addin"; Dest = "MepVerticalMark.addin" },
    @{ Tool = "PipeSizing";       Source = "PipeSizing.addin";       Dest = "PipeSizing.addin" },
    @{ Tool = "Quantity";         Source = "Quantity.addin";         Dest = "Quantity.addin" },
    @{ Tool = "REXJManager";      Source = "REXJManager.addin";      Dest = "REXJManager.addin" }
)
foreach ($entry in $rootAddinTools) {
    $src = Join-Path $Root "$($entry.Tool)\$($entry.Source)"
    if (-not (Test-Path $src)) {
        throw "Missing addin manifest for $($entry.Tool): $src"
    }
    Copy-Item -Path $src -Destination (Join-Path $releasedRoot $entry.Dest) -Force
}

Write-Host "Installer payload staging complete."
