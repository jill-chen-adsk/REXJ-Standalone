# REXJ Standalone Add-ins for Revit 2027

A collection of standalone Revit 2027 add-ins, localized to English, originally derived from the REXJ (Revit Extension Japan) toolset. Each tool is an independent Revit add-in that registers under a shared **REXJ Standalone** ribbon tab.

## Prerequisites

- **Autodesk Revit 2027**
- **.NET 10 SDK** (x64)
- **Visual Studio 2022** (17.x+) with the .NET desktop development workload
- **Inno Setup 6** (optional, for building the installer)

## Building

Open `REXJ-Standalone.sln` in Visual Studio and build in **Release | x64**, or use the command line:

```
dotnet build REXJ-Standalone.sln -c Release -p:Platform=x64
```

Each tool builds independently. You can also build individual projects:

```
dotnet build AreaSchedule\ADSK.JExtRAC.AreaSchedule.csproj -c Release -p:Platform=x64
```

## Deployment

After building, copy each tool's output (DLL + dependencies) and its `.addin` manifest to the Revit add-ins folder:

```
%APPDATA%\Autodesk\Revit\Addins\2027\
```

Each tool has an `.addin` file that references `<ToolFolder>\<Assembly>.dll`. Place the `.addin` file in the root of the Addins folder and the tool's DLL + resources in a subfolder.

## Building the Installer

If you have Inno Setup 6 installed:

```
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" REXJ_Installer.iss
```

The installer will be output to `Installer\Output\`.

## Tools

### Architecture / Common (27 tools)

| Tool | Panel | Commands |
|------|-------|----------|
| AreaSchedule | Code Check | Room to Area, Grounds Expression, Legal Area |
| AutoCreateDimension | Dimension | Batch Object Dimension |
| AutoCreateRoomView | Room | Room View Creation |
| AutoLayoutTag | Array & Tag | Auto Tag Placement |
| AutomaticFloor | Auto Floor | Architecture Floor, Structural Floor, Foundation Slab |
| AveSiteLevelHeightCalc | Code Check | Avg Site Level Calc |
| CheckingALVS | Code Check | Daylight Check, Smoke Exhaust Check, Ventilation Check |
| EnhancedSectionBox | Section Box | Section Box Adjustment, Box View |
| ExcelImageInsert | Excel | Excel Image Insert |
| ExportExcel | Excel | Export Excel |
| ExportSchedule | Excel | Schedule Export |
| FittingSchedule | Window/Door View | Create View, Layout Views in Sheet |
| FloorHeightDimension | Dimension | Level Height Dimension |
| Fukashi | Fukashi | Face Furring, Region Furring |
| GridDimension | Dimension | Grid Dimension |
| ImportExcel | Excel | Import Excel |
| JoinOrderInspector | Join | Join Order Inspector |
| LayoutInstanceInRegion | Array & Tag | Instance Array Layout |
| LevelFilter | Filter | Level Filter |
| LocateSlab | Auto Floor | Beam Range Floor Layout |
| ParameterFilter | Filter | Parameter Filter |
| PrintRegion | Print | Range Print |
| SwitchJoinOrder | Join | Join Adjustment |
| ValueCopy | Value Copy | Copy Parameter |
| ViewDuplicate | Views | View Copy |
| SheetLayout | Views | Auto Sheet Layout |
| TenkaiView | Views | Interior Elevation for Room |

### MEP Tools (7 tools)

| Tool | Panel | Commands |
|------|-------|----------|
| MEPConnectTool | Connection Tool | Rect Duct Connection, Round Duct Connection, Duct/Pipe T-45 Connection |
| MepManholeTool | Manhole Tool | Model Line, Parameter Mapping |
| MepDuctPipeTool | Edit | Flange Settings, Flange/Accessory, Insert Flange |
| MepVerticalMark | Arrow Mark Tool | Arrow Duct, Arrow Pipe |
| MEPExtension | Edit | Duct/Pipe Level Offset, Rotate Accessory, Move Accessory, Flexible Duct Convert |
| PipeSizing | Pipe Size Correction | Pipe Size Correction, Edit Pipe System Definition |
| Quantity | Pipe/Duct Quantity Pickup | Quantity Diagram, Pipe Quantity, Duct Quantity |

### Structural Tools (5 tools)

| Tool | Panel | Commands |
|------|-------|----------|
| RSTExtension | Structure Tag Filter | Correct Framing Plan, Exclusion Special Mention |
| SectionListRC | RC Section List | Common/Column/Beam Settings, Column List, Beam List |
| SectionListSteel | S Section List | Settings, Story Sort, List (All/Column/Post/Girder/Beam/Brace) |
| MappingTable | Mapping Table | Edit Mapping Table, Base Family Path, Batch Add Parameters |
| STBLink | ST-Bridge Link | ST-Bridge Import, Diff Import, STB Export |

### Manager

| Tool | Panel | Commands |
|------|-------|----------|
| REXJManager | Settings | Ribbon Tab Visibility (show/hide tools by preset) |

## Documentation

Product and portfolio docs live in [`docs/`](./docs/):

- **[REXJ Review — Done & Remains](./docs/REXJ_Review_Done_and_Remains.html)** — achievement vs gaps summary (styled HTML)
- **[Edit source (Markdown)](./docs/REXJ_Review_Done_and_Remains.md)** — preferred file for collaborator updates via pull request

See [`docs/README.md`](./docs/README.md) for the full doc index and editing workflow.

## License

Internal use. All rights reserved.
