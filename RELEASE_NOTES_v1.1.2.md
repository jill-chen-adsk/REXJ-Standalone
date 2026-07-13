## REXJ Standalone Add-ins for Revit 2027 — v1.1.2

### What's Changed

**Legal Compliance Check (#09 — CheckingALVS):**
- Weave-compliant WPF dialogs for **Daylight Check**, **Smoke Exhaust Check**, and **Ventilation Check**
- Smoke and ventilation checks now use the same Weave UI as daylight (replaces legacy WinForms)
- Fixed ceiling height unit consistency for smoke calculations (room volume/area height uses project length units)
- Smoke effective height and area calculations align with head height and smoke wall values in mm/m projects
- Improved property-line horizontal distance measurement and bulk-update behavior for daylight openings
- Imperial and mixed-unit project support for area and distance calculations

**Room Area Calculation (#02 — AreaSchedule):**
- No changes in v1.1.2 — see [v1.1.1](https://git.autodesk.com/chenji/REXJ-Standalone/releases/tag/v1.1.1) for Weave-themed WPF dialogs and geometry fixes

---

### Full Installer (Recommended)

| File | Description | Version | Last Updated |
|------|-------------|---------|--------------|
| [REXJ_Standalone_Setup_1.1.0.exe](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_Standalone_Setup_1.1.0.exe) | Complete bundle: choose components and install every REXJ Standalone tool for Revit 2027 | **1.1.0** | **2026-06-04** |

> **Note:** The full bundle installer has not been rebuilt for v1.1.2. Install the updated per-function installer below (#09), or use the full bundle and update CheckingALVS individually.

---

### Per-Function Installers

Individual installers organized by function. Install only the functions you need.

**Architecture (9 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 01 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_01_MeanGroundSurface_Setup_1.0.0.exe) | Calculate mean ground surface | Computes average ground elevation from topography or boundary points for planning inputs | 1.0.0 | 2026-05-13 |
| 02 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.1/REXJ_02_RoomAreaCalculation_Setup_1.0.1.exe) | Room area calculation for building application | Legal Area, Grounds Expression, and Room-to-Area workflows with schedules and annotations | 1.0.1 | 2026-05-19 |
| **02** | [**Download**](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.1/REXJ_02_RoomAreaCalculation_Setup_1.1.0.exe) | **🌐 Room area calculation (Weave-Compliant)** | **Dark/light theme WPF dialogs for Legal Area, Grounds Expression, and Room to Area** | **1.1.0** | **2026-07-10** |
| 03 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_03_WindowDoorView_Setup_1.0.0.exe) | Window/Door View | Creates and organizes views focused on doors and windows | 1.0.0 | 2026-05-13 |
| 03 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_03_WindowDoorView_Weave_Setup_1.1.0.exe) | **🌐 Window/Door View (Weave-Compliant)** | Dark/light theme, globalized (JA/EN), WebView2 UI | **1.1.0** | **2026-06-04** |
| 04 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_04_AutoFloor_Setup_1.0.0.exe) | Auto Floor | Automated floor slab creation plus slab join and split helpers | 1.0.0 | 2026-05-13 |
| 05 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_05_JoinOrderAdjustment_Setup_1.0.0.exe) | Join Order Adjustment | Adjusts join geometry order and inspects join relationships between elements | 1.0.0 | 2026-05-13 |
| 06 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_06_FillConcreteGap_Setup_1.0.0.exe) | Fill concrete gap (Fukashi) | Face furring and region furring tools for concrete gaps | 1.0.0 | 2026-05-13 |
| 07 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_07_GenerateRoomView_Setup_1.0.0.exe) | Generate View for each room | Bulk-creates views tied to individual rooms | 1.0.0 | 2026-05-13 |
| 08 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_08_InteriorElevation_Setup_1.0.0.exe) | Interior elevation for rooms | Generates interior elevation views per room | 1.0.0 | 2026-05-13 |
| 09 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_09_LegalComplianceCheck_Setup_1.0.0.exe) | Legal compliance check for Rooms | Daylight, smoke exhaust, and ventilation checks with Excel report | 1.0.0 | 2026-05-13 |
| 09 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.1/REXJ_09_LegalComplianceCheck_Setup_1.1.0.exe) | **🌐 Legal compliance check (Weave-Compliant)** | Dark/light theme dialogs for Daylight, Smoke, and Ventilation Check | 1.1.0 | 2026-07-10 |
| **09** | [**Download**](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.2/REXJ_09_LegalComplianceCheck_Setup_1.1.2.exe) | **🌐 Legal compliance check (Weave-Compliant)** | **Weave WPF dialogs for all checks; smoke unit fixes and calculation improvements** | **1.1.2** | **2026-07-13** |

**Structure (5 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 10 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_10_StructuralMappingTool_Setup_1.0.0.exe) | Structural mapping tool | Maps structural parameters between models or standards | 1.0.0 | 2026-05-13 |
| 11 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_11_STBridgeLink_Setup_1.0.0.exe) | ST-Bridge Link | Legacy ST-Bridge import/export with difference tracking | 1.0.0 | 2026-05-13 |
| 12 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_12_StructuralPlanAndTagFilter_Setup_1.0.0.exe) | Structural plan view + Tag Filter | Structural plan views and structure-aware tag filtering | 1.0.0 | 2026-05-13 |
| 14 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_14_SectionListRC_Setup_1.0.0.exe) | Reinforced Concrete Section List | RC section schedules and listings | 1.0.0 | 2026-05-13 |
| 15 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_15_SectionListSteel_Setup_1.0.0.exe) | Steel Section List | Steel section schedules and listings | 1.0.0 | 2026-05-13 |

**MEP (6 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 16 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_16_DuctPipeLengthTool_Setup_1.0.0.exe) | Duct/Pipe length tool (Quantity Pickup) | Quantifies duct and pipe lengths for estimates and documentation | 1.0.0 | 2026-05-13 |
| 17 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_17_PipeSizing_Setup_1.0.0.exe) | Pipe sizing with flow volume | Pipe size correction driven by flow and engineering inputs | 1.0.0 | 2026-05-13 |
| 18 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_18_MEPEditingTools_Setup_1.0.0.exe) | MEP Editing Tools | Flex duct conversion, connector moves, tee rotation, clash avoidance, duct monitoring | 1.0.0 | 2026-05-13 |
| 20 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_20_FlangeTools_Setup_1.0.0.exe) | Flange Tools | Places flanges and flange-related accessories on MEP runs | 1.0.0 | 2026-05-13 |
| 23 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_23_VerticalSymbol_Setup_1.0.0.exe) | Vertical Symbol (Arrow Mark) | Vertical pipe and duct arrow marks for drawings | 1.0.0 | 2026-05-13 |
| 24 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_24_ConnectionTool_Setup_1.0.0.exe) | Connection Tool | Connects ducts and pipes with guided workflows | 1.0.0 | 2026-05-13 |

**For All Users (12 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 27 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_27_ArrayPlacement_Setup_1.0.0.exe) | Array Placement | Pattern placement and array helpers for families | 1.0.0 | 2026-05-13 |
| 28 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_28_ChooseFamilyByParam_Setup_1.0.0.exe) | Choose Family based on parameter | Swaps or picks families according to parameter rules | 1.0.0 | 2026-05-13 |
| 29 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_29_AutoDimension_Setup_1.0.0.exe) | Generate dimension Automatically | Automated dimension chains from model geometry | 1.0.0 | 2026-05-13 |
| 30 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_30_EyedropperTool_Setup_1.0.0.exe) | Eyedropper tool (Copy Parameter) | Copies parameter values between matching elements | 1.0.0 | 2026-05-13 |
| 31 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_31_AdvancedSectionBox_Setup_1.0.0.exe) | Advanced Section box | Enhanced section box manipulation for 3D views | 1.0.0 | 2026-05-13 |
| 31 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_31_AdvancedSectionBox_Setup_1.1.0.exe) | **🌐 Advanced Section Box (Weave-Compliant)** | Dark/light theme, WPF borderless dialogs | **1.1.0** | **2026-06-04** |
| **32** | [**Download**](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.2/REXJ_32_AutoTag_Setup_1.0.2.exe) | **Auto Tag** | **Automated tag placement on views** | **1.0.2** | **2026-05-28** |
| 33 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_33_ViewDuplication_Setup_1.0.0.exe) | View duplication tool | Duplicates views with naming and option presets | 1.0.0 | 2026-05-13 |
| 34 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_34_PrintRegion_Setup_1.0.0.exe) | Print Region tool | Prints user-defined regions on sheets | 1.0.0 | 2026-05-13 |
| 35 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_35_SpreadsheetExportImport_Setup_1.0.0.exe) | Spreadsheet Export/Import | Export to Excel, export schedules, import from Excel, insert Excel images | 1.0.0 | 2026-05-13 |
| 36 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_36_GridElevationDimension_Setup_1.0.0.exe) | Grid/Elevation dimension | Grid dimensions and floor-to-floor height dimensions | 1.0.0 | 2026-05-13 |
| 37 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_37_MultiQueryFilter_Setup_1.0.0.exe) | Multi query filter (Level Filter) | Advanced filtering including level-based queries | 1.0.0 | 2026-05-13 |
| 37 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_37_MultiQueryFilter_Setup_1.1.0.exe) | **🌐 Multi Query Filter / Level Filter (Weave-Compliant)** | Dark/light theme, WPF DataGrid, tabbed UI | **1.1.0** | **2026-06-04** |
| 38 | [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_38_SheetAutoLayout_Setup_1.0.0.exe) | Sheet auto layout tool | Automates sheet layouts and viewport placement | 1.0.0 | 2026-05-13 |

**Additional**

| Installer | Function | Description | Version | Last Updated |
|-----------|----------|-------------|---------|--------------|
| [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_EnhancedSectionBox_Setup_1.1.0.exe) | **🌐 Enhanced Section Box (Weave-Compliant)** | Dark/light theme WPF dialogs for Box View and Section Box Adjustment | **1.1.0** | **2026-06-04** |
| [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_FittingSchedule_Setup_1.1.0.exe) | **🌐 Fitting Schedule / Window Door View (Weave-Compliant)** | Per-tool installer for FittingSchedule | **1.1.0** | **2026-06-04** |
| [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.1.0/REXJ_LevelFilter_Setup_1.1.0.exe) | **🌐 Level Filter (Weave-Compliant)** | Per-tool installer for Level Filter | **1.1.0** | **2026-06-04** |
| [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_MepManholeTool_Setup_1.0.0.exe) | Manhole Tool | Places and documents manhole elements for MEP | 1.0.0 | 2026-05-13 |
| [Download](https://git.autodesk.com/chenji/REXJ-Standalone/releases/download/v1.0.0/REXJ_REXJManager_Setup_1.0.0.exe) | REXJ Manager | Controls visibility of REXJ Standalone ribbon tabs and command presets | 1.0.0 | 2026-05-13 |

### Installation

1. Download the installer(s) above
2. Close Revit 2027 if running
3. Run the installer — tools deploy to the Revit 2027 Addins folder automatically
4. Launch Revit 2027 — tools appear under the **REXJ Standalone** ribbon tab

### Notes

- Set **Opening Coefficient** on window/door family types for smoke and ventilation effective area calculations
