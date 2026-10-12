# Copies Release 2027 build outputs into paths expected by REXJ_Installer.iss.
param(
    [string]$Root = "C:\REXJ\REXJ-Standalone",
    [string]$Config = "Release 2027",
    [string]$Platform = "x64"
)

$ErrorActionPreference = "Stop"
$releasedRoot = Join-Path $Root "Released"

function Copy-ToolBin {
    param(
        [string]$ToolFolder,
        [string[]]$DestDirs
    )

    $bin = Join-Path $Root "$ToolFolder\bin\$Platform\$Config"
    if (-not (Test-Path $bin)) {
        Write-Warning "No build output for $ToolFolder — skipped."
        return
    }

    $addins = Get-ChildItem (Join-Path $Root $ToolFolder) -Filter "*.addin" -File -ErrorAction SilentlyContinue
    foreach ($dest in $DestDirs) {
        New-Item -ItemType Directory -Force -Path $dest | Out-Null
        Copy-Item -Path "$bin\*" -Destination $dest -Recurse -Force
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
    Copy-ToolBin -ToolFolder $tool -DestDirs @(Join-Path $releasedRoot $tool)
}

$toolReleased = @(
    "LevelFilter", "ParameterFilter", "AutoCreateDimension", "GridDimension", "FloorHeightDimension",
    "ViewDuplicate", "SheetLayout", "TenkaiView", "EnhancedSectionBox", "AutoCreateRoomView",
    "SwitchJoinOrder", "JoinOrderInspector", "AutoLayoutTag", "LayoutInstanceInRegion",
    "Fukashi", "MappingTable", "STBLink", "ValueCopy", "PrintRegion"
)
foreach ($tool in $toolReleased) {
    Copy-ToolBin -ToolFolder $tool -DestDirs @(Join-Path $Root "$tool\Released")
}

$releasedSubdirs = @(
    "RSTExtension", "SectionListRC", "SectionListSteel", "MEPConnectTool", "MepManholeTool",
    "MepDuctPipeTool", "MepVerticalMark", "MEPExtension", "PipeSizing", "Quantity", "REXJManager"
)
foreach ($tool in $releasedSubdirs) {
    Copy-ToolBin -ToolFolder $tool -DestDirs @(Join-Path $releasedRoot $tool)
}

# Copy root-level .addin manifests referenced from Released\ in the installer script.
$rootAddinTools = @{
    "SectionListRC"   = "SectionListRC.addin"
    "SectionListSteel" = "SectionListSteel.addin"
    "MEPConnectTool"  = "MEPConnectTool.addin"
    "MepManholeTool"  = "MepManholeTool.addin"
    "MepDuctPipeTool" = "MepDuctPipeTool.addin"
    "MepVerticalMark" = "MepVerticalMark.addin"
    "PipeSizing"      = "PipeSizing.addin"
    "Quantity"        = "Quantity.addin"
    "REXJManager"     = "REXJManager.addin"
}
foreach ($entry in $rootAddinTools.GetEnumerator()) {
    $src = Join-Path $Root "$($entry.Key)\$($entry.Value)"
    if (Test-Path $src) {
        Copy-Item -Path $src -Destination (Join-Path $releasedRoot $entry.Value) -Force
    }
}

$fallbackReleased = "C:\REXJ\Standalone\Released"
if (Test-Path $fallbackReleased) {
    foreach ($tool in $releasedSubdirs) {
        $dst = Join-Path $releasedRoot $tool
        $dll = Get-ChildItem $dst -Filter *.dll -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($null -eq $dll) {
            $src = Join-Path $fallbackReleased $tool
            if (Test-Path $src) {
                New-Item -ItemType Directory -Force -Path $dst | Out-Null
                Copy-Item -Path "$src\*" -Destination $dst -Recurse -Force
                Write-Host "  Fallback: $tool from $fallbackReleased"
            }
        }
    }
}

Write-Host "Installer payload staging complete."
