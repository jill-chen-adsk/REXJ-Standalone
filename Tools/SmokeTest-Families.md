# REXJ Standalone — bundled family smoke test (Revit 2027)

Use this after upgrading or redeploying bundled `.rfa` files so tools load **Revit 2027** families without in-session upgrade prompts.

## Prerequisites

- Revit **2027** installed.
- Add-ins deployed under `%APPDATA%\Autodesk\Revit\Addins\2027\` (and AveSiteLevelHeightCalc from `C:\REXJ\Standalone\Released\`).
- **Close Revit before deploy**; restart Revit after deploy so DLLs and families reload.

## Step 1 — Disk pre-flight (no Revit UI)

From PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File C:\REXJ\REXJ-Standalone\Tools\SmokeTest-Families.ps1
```

**Pass:** every location reports OK with version **2027**.

| Location checked | Role |
|------------------|------|
| `%APPDATA%\...\Addins\2027\MEPExtension` | S-curve (`S管`) families next to DLL |
| `%APPDATA%\...\Addins\2027\MepManholeTool\Resources\rfa2027` | Manhole families |
| `%APPDATA%\...\Addins\2027\MepVerticalMark` | Vertical-mark tag families |
| `%APPDATA%\...\Addins\2027\ADSK.Ext.Fukashi` | Fukashi families (flat folder) |
| `%APPDATA%\...\Addins\2027\SectionListRC\rfa` | Rebar mark families |
| `C:\REXJ\Standalone\Released\AveSiteLevelHeightCalc\rfa2027` | Site level families |

**Fail:** missing folder, zero `.rfa` files, or version not 2027 → re-run `deploy.ps1` for the affected tool and copy to AppData if needed.

## Step 2 — Revit session

1. Close Revit completely.
2. Start **Revit 2027**.
3. Open a test project (any `.rvt` with MEP/structure content is fine).

**Watch during open and first tool use:**

- No **“upgrade family to 2027”** dialog.
- No **MepManholeTool “Family Load Warning”** (load path or missing `rfa2027`).

If the project was open before deploy, **close and reopen the project** (or restart Revit) so `DocumentOpened` handlers run with the new files.

## Step 3 — Per-tool ribbon smoke (≈15 min)

| Tool | Action | Pass |
|------|--------|------|
| **MepManholeTool** | Open project (auto-load on document open). Optionally run **Model Line**. | Manhole families load; no warnings. |
| **MepVerticalMark** | Run vertical-mark tagging once. | Tag families load. |
| **MEPExtension** | Run **duct displacement** (S-curve) on a duct. | `S管` families load from add-in folder. |
| **Fukashi** | Run Face or Opening once. | Families load from DLL folder. |
| **SectionListRC** | Start workflow that loads rebar mark families. | No upgrade prompt. |
| **AveSiteLevelHeightCalc** | Run average site level / symbol command. | Loads from Released `rfa2027`. |

Optional: in **Manage → Family**, search for names such as `08070_公共桝`, `SectionList_RebarMark`, `S管`, `フカシファミリ`, `タグ 矢羽` — they should appear **after** the relevant tool has loaded them into the project.

## Step 4 — Optional MCP verification (Cursor + Revit MCP)

When Revit is open and families have been loaded into the project, an agent can call `query_model` with `familyNameFilters` (partial match), for example:

- `08070_公共桝`
- `SectionList`
- `S管`
- `フカシファミリ`
- `矢羽`

MCP cannot run ribbon commands; it only confirms families present in the model after load.

## One-time dev: upgrade families on disk

The optional add-in under `Tools/FamilyUpgrader/` upgrades repo/AppData families to the running Revit version. After a successful run it renames its manifest to `.addin.done`. Re-enable only when preparing the next Revit year.

## Troubleshooting

| Symptom | Likely cause |
|---------|----------------|
| Upgrade dialog on load | Deployed `.rfa` still older than 2027; re-run upgrader or replace from repo. |
| Family Load Warning (manhole) | Wrong path (`rfa2026` vs `rfa2027`) or missing `%APPDATA%\...\MepManholeTool\Resources\rfa2027`. |
| S-curve tool cannot find family | `MEPExtension` folder missing four `.rfa` next to `MEPExtension.dll`. |
| Fukashi cannot find family | Families not copied flat to `ADSK.Ext.Fukashi` output (rebuild + deploy Fukashi). |
| Changes not picked up | Revit still running during deploy, or project not reopened after deploy. |

## Related

- Build/deploy: `.claude/skills/run-rexj/SKILL.md`
- Deploy script: `.claude/skills/run-rexj/deploy.ps1`
