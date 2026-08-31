param(
    [Parameter(Mandatory = $false)]
    [ValidateSet("tools/list", "tools/call", "initialize")]
    [string]$Method = "tools/list",

    [Parameter(Mandatory = $false)]
    [string]$ToolName,

    [Parameter(Mandatory = $false)]
    [hashtable]$Arguments = @{},

    [Parameter(Mandatory = $false)]
    [string]$BaseUrl = "http://127.0.0.1:28733",

    [Parameter(Mandatory = $false)]
    [int]$Id = 1
)

$ErrorActionPreference = "Stop"

$params = @{}
if ($Method -eq "tools/call") {
    if (-not $ToolName) {
        throw "ToolName is required when Method is tools/call."
    }
    $params = @{
        name      = $ToolName
        arguments = $Arguments
    }
}

$body = @{
    jsonrpc = "2.0"
    id      = $Id
    method  = $Method
    params  = $params
} | ConvertTo-Json -Depth 10

Write-Host "POST $BaseUrl/mcp" -ForegroundColor Cyan
Write-Host $body

$response = Invoke-RestMethod -Uri "$BaseUrl/mcp" -Method Post -Body $body -ContentType "application/json; charset=utf-8"
$response | ConvertTo-Json -Depth 20
