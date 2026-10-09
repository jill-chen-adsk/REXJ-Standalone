# Pre-flight: bundled .rfa deploy paths and saved Revit version (no Revit UI required).
$ErrorActionPreference = 'Stop'
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

function Get-RfaFormatVersion([string]$path) {
    $b = [System.IO.File]::ReadAllBytes($path)
    foreach ($off in 0, 1) {
        if ($b.Length -le $off) { continue }
        $u = [System.Text.Encoding]::Unicode.GetString($b, $off, $b.Length - $off)
        $m = [regex]::Match($u, '(?:Format|Autodesk Revit|Revit Build)[^\d]{0,20}(20\d\d)')
        if ($m.Success) { return $m.Groups[1].Value }
    }
    return '?'
}

$checks = @(
    @{ Label = 'MEPExtension (S-curve)'; Path = "$env:APPDATA\Autodesk\Revit\Addins\2027\MEPExtension"; Pattern = '*.rfa' }
    @{ Label = 'MepManholeTool'; Path = "$env:APPDATA\Autodesk\Revit\Addins\2027\MepManholeTool\Resources\rfa2027"; Pattern = '*.rfa' }
    @{ Label = 'MepVerticalMark'; Path = "$env:APPDATA\Autodesk\Revit\Addins\2027\MepVerticalMark"; Pattern = '*.rfa' }
    @{ Label = 'Fukashi'; Path = "$env:APPDATA\Autodesk\Revit\Addins\2027\ADSK.Ext.Fukashi"; Pattern = '*.rfa' }
    @{ Label = 'SectionListRC'; Path = "$env:APPDATA\Autodesk\Revit\Addins\2027\SectionListRC\rfa"; Pattern = '*.rfa' }
    @{ Label = 'AveSiteLevelHeightCalc (Released)'; Path = 'C:\REXJ\Standalone\Released\AveSiteLevelHeightCalc\rfa2027'; Pattern = '*.rfa' }
)

$fail = 0
Write-Host "REXJ family smoke pre-flight (disk)" -ForegroundColor Cyan
foreach ($c in $checks) {
    if (-not (Test-Path $c.Path)) {
        Write-Host "FAIL  $($c.Label): missing folder $($c.Path)" -ForegroundColor Red
        $fail++
        continue
    }
    $files = Get-ChildItem -Path $c.Path -Filter $c.Pattern -File -ErrorAction SilentlyContinue
    if ($files.Count -eq 0) {
        Write-Host "FAIL  $($c.Label): no .rfa in $($c.Path)" -ForegroundColor Red
        $fail++
        continue
    }
    $vers = $files | ForEach-Object { Get-RfaFormatVersion $_.FullName } | Group-Object
    $bad = $vers | Where-Object { $_.Name -ne '2027' -and $_.Name -ne '?' }
    if ($bad) {
        Write-Host "FAIL  $($c.Label): not all 2027 — $(($vers | ForEach-Object { "$($_.Name)x$($_.Count)" }) -join ', ')" -ForegroundColor Red
        $fail++
    } else {
        Write-Host "OK    $($c.Label): $($files.Count) files, version $(($vers | ForEach-Object { $_.Name }) -join ', ')" -ForegroundColor Green
    }
}

if ($fail -gt 0) {
    Write-Host "`n$fail check(s) failed. Fix deploy before Revit UI smoke test." -ForegroundColor Red
    exit 1
}
Write-Host "`nDisk pre-flight passed. Next: restart Revit 2027, open a project, run ribbon tools (see run-rexj skill)." -ForegroundColor Green
exit 0
