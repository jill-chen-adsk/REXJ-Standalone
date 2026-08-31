# REXJ MCP Host — Engineering Spike

Localhost MCP-style HTTP bridge for REXJ Standalone Revit capabilities. This spike exposes two read-only pilot tools:

| Capability | Source tool | MCP name |
|---|---|---|
| Elements by level | LevelFilter (new query logic) | `rexj.rac.query.elements-by-level` |
| Join order inspect | JoinOrderInspector | `rexj.rac.diagnose.join-order` |

## Architecture

```
Agent / MCP client
    → HTTP POST http://127.0.0.1:28733/mcp  (JSON-RPC)
    → REXJ.McpHost (Revit add-in)
    → ExternalEvent → Revit API
    → Capability handler
```

## Build

```powershell
cd C:\REXJ\REXJ-Standalone
dotnet build REXJ.McpHost\REXJ.McpHost.csproj -c "Release 2027" -p:Platform=x64
```

Output: `REXJ.McpHost\bin\x64\Release 2027\`

## Deploy to Revit 2027

Copy to `%APPDATA%\Autodesk\Revit\Addins\2027\`:

```
REXJ.McpHost\
  REXJ.McpHost.dll
  manifest\
    capabilities.json
0_REXJ.McpHost.addin   ← place in Addins\2027\ root (not inside REXJ.McpHost\)
```

The `.addin` manifest references `REXJ.McpHost\REXJ.McpHost.dll` relative to the Addins folder.

Restart Revit and open a project. The server starts on `ApplicationInitialized`.

## Verify

```powershell
# Health check
Invoke-RestMethod http://127.0.0.1:28733/health

# List tools
.\REXJ.McpHost\scripts\Invoke-RexjMcpTool.ps1 -Method tools/list

# Query elements on Level 1
.\REXJ.McpHost\scripts\Invoke-RexjMcpTool.ps1 -Method tools/call -ToolName rexj.rac.query.elements-by-level -Arguments @{ levelNames = @("Level 1") }

# Inspect join order for element 12345
.\REXJ.McpHost\scripts\Invoke-RexjMcpTool.ps1 -Method tools/call -ToolName rexj.rac.diagnose.join-order -Arguments @{ elementId = 12345 }
```

## JSON-RPC API

**POST** `/mcp`

```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "tools/call",
  "params": {
    "name": "rexj.rac.query.elements-by-level",
    "arguments": { "levelNames": ["Level 1"], "categoryNames": ["Walls"] }
  }
}
```

Methods: `initialize`, `tools/list`, `tools/call`

**GET** `/health` — returns `{ status, revitReady, port }`

## Notes

- All Revit API work runs on the main thread via `ExternalEvent`.
- Default port: **28733** (localhost only).
- `applyViewOverrides: true` on join-order is the only optional side effect in this spike.
- Full capability schemas: `manifest/capabilities.json`

## Next steps (pilot items 3–6)

- `rexj.rac.codecheck.avg-site-level` (AveSiteLevelHeightCalc)
- `rexj.mep.quantity.export-duct` (Quantity)
- `rexj.rac.codecheck.alvs` (CheckingALVS)
- `rexj.rst.stb.export` (STBLink)
