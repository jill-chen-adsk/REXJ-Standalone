## REXJ Standalone Add-ins for Revit 2027  — v1.1.7

### What's Changed

**View Duplicate (#33 — ViewDuplicate):**
- Weave-compliant WPF main and Add rule dialogs in dark and light theme, replacing WinForms
- Filter source views by discipline, view category, and view family type; multi-select with Shift/Ctrl
- Define multiple duplication rules (prefix/suffix, view type, view template) and create every copy in one run
- Run report lists created views and skips; message dialogs center on Revit and remember window position
- Per-function Weave installer for function **#33**
**Batch Object Dimension (#29  — AutoCreateDimension):**
- Weave-compliant WPF options dialog in dark and light theme, replacing the WinForms dialog
- Face–Face, Face–Reference and Reference–Reference dimensions are created as one chained string along the row at the picked point
- Across-row dimensions are placed once per aligned group, beside the unit nearest the picked point, instead of overlapping
- Ducts and pipes chain correctly, including sloped runs; fittings are skipped
- Zero-length segments and dimensions are removed; chains with a negative segment (including imperial units) are deleted and reported
- Elements that could not be dimensioned are reported as a native Revit warning (Show / Expand) instead of a separate log window
- Pressing **Esc** while picking keeps the dimensions already placed; undo entries follow the UI language

**Level Height Dimension (#36  — FloorHeightDimension):**
- Weave-compliant WPF configuration and view-selection dialogs
- Example diagram is a localized vector graphic that follows the theme, instead of a Japanese-only bitmap
- View list: Shift-click range selection and a select-all header that stays in sync with long, scrolled lists
- Dimension type defaults to a valid selection when no saved setting exists
- Command errors show the underlying reason instead of a generic message

**Room area calculation (#02  — AreaSchedule):**
- Per-function installer rebuilt with the latest code: previously only the full bundle carried these changes
- Weave message and progress dialogs, rounded Weave chrome in dark and light theme, and theme colors for the window chrome
- Room to Area boundary prompt uses a Weave Yes/No dialog; Cancel buttons no longer show a literal **(_C)**

**All Weave dimension dialogs:**
- Fonts and sizes come from the shared Weave typography tokens (Artifakt Element) instead of hardcoded Segoe UI
- Message dialogs center on the Revit window; dropdowns and close buttons are themed in dark mode

> Includes every change from v1.1.6 and v1.1.5 (Join Adjustment / Join Order Inspector, Fitting Schedule Create View fixes, rounded Weave chrome) and v1.1.4.

---

### Full Installer (Recommended)

| File | Description | Version | Last Updated |
|------|-------------|---------|--------------|
| [REXJ_Standalone_Setup_1.1.7.exe](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.7/REXJ_Standalone_Setup_1.1.7.exe) | Complete bundle: choose components and install every REXJ Standalone tool for Revit 2027 | **1.1.7** | **2026-10-21** |

> **Note:** The full bundle has been rebuilt for v1.1.7 with Weave View Duplicate and includes v1.1.6 dimension and room-area updates. Copy Parameter and Parameter Filter (Weave) are still available in the full bundle only. It also includes every v1.1.5 update.

> **Upgrading the dimension tools:** if you installed the v1.0.0 **#29 Generate dimension Automatically** or **#36 Grid/Elevation dimension** installer, uninstall it from Windows **Apps & features** before installing the v1.1.6 version.

---
### Per-Function Installers

Individual installers organized by function. Install only the functions you need.

| Link in **Installer** column | What it means |
|------------------------------|---------------|
| **Download** | Per-function `.exe` when available, or `REXJ_Standalone_Setup_*.exe` for Weave builds that do not have a separate installer yet (use the setup wizard to choose components). |

**Architecture (9 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 01 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_01_MeanGroundSurface_Setup_1.0.0.exe) | Calculate mean ground surface | Computes average ground elevation from topography or boundary points for planning inputs | 1.0.0 | 2026-05-13 |
| 01 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.3/REXJ_Standalone_Setup_1.1.3.exe) | **🌐 Avg Site Level Calculation (Weave-Compliant)** | Dark/light theme WPF dialogs  — available in the full bundle only, no per-function installer yet | 1.1.3 | 2026-08-31 |
| 02 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.1/REXJ_02_RoomAreaCalculation_Setup_1.0.1.exe) | Room area calculation for building application | Legal Area, Grounds Expression, and Room-to-Area workflows with schedules and annotations | 1.0.1 | 2026-05-19 |
| **02** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.6/REXJ_02_RoomAreaCalculation_Setup_1.1.6.exe) | **🌐 Room area calculation (Weave-Compliant)** | **Dark/light theme WPF dialogs for Legal Area, Grounds Expression, and Room to Area  — rounded Weave chrome, Weave message/progress and Yes/No dialogs** | **1.1.6** | **2026-10-20** |
| 03 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_03_WindowDoorView_Setup_1.0.0.exe) | Window/Door View | Creates and organizes views focused on doors and windows | 1.0.0 | 2026-05-13 |
| 03 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_03_WindowDoorView_Weave_Setup_1.1.5.exe) | **🌐 Fitting Schedule / Window Door View (Weave-Compliant)** | Dark/light theme, globalized (JA/EN), WebView2 UI  — Create View (imperial/metric scale presets, custom scale) and Layout Views in Sheet | **1.1.5** | **2026-10-13** |
| 04 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_04_AutoFloor_Setup_1.0.0.exe) | Auto Floor | Automated floor slab creation plus slab join and split helpers | 1.0.0 | 2026-05-13 |
| **04** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_04_AutoFloor_SlabJoinSplit_Weave_Setup_1.1.5.exe) | **🌐 Auto Floor and Slab Join & Split (Weave-Compliant)** | **Dark/light theme WPF dialogs with input validation  — Auto Floor and Slab Join & Split only** | **1.1.5** | **2026-10-12** |
| 05 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_05_JoinOrderAdjustment_Setup_1.0.0.exe) | Join Order Adjustment | Adjusts join geometry order and inspects join relationships between elements | 1.0.0 | 2026-05-13 |
| **05** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_05_JoinAdjustment_JoinOrderInspector_Weave_Setup_1.1.5.exe) | **🌐 Join Adjustment and Join Order Inspector (Weave-Compliant)** | **Rounded Weave dialogs  — Join Adjustment and Join Order Inspector only** | **1.1.5** | **2026-10-12** |
| 06 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_06_FillConcreteGap_Setup_1.0.0.exe) | Fill concrete gap (Fukashi) | Face furring and region furring tools for concrete gaps | 1.0.0 | 2026-05-13 |
| 07 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_07_GenerateRoomView_Setup_1.0.0.exe) | Generate View for each room | Bulk-creates views tied to individual rooms | 1.0.0 | 2026-05-13 |
| 08 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_08_InteriorElevation_Setup_1.0.0.exe) | Interior elevation for rooms | Generates interior elevation views per room | 1.0.0 | 2026-05-13 |
| 09 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_09_LegalComplianceCheck_Setup_1.0.0.exe) | Legal compliance check for Rooms | Daylight, smoke exhaust, and ventilation checks with Excel report | 1.0.0 | 2026-05-13 |
| 09 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.1/REXJ_09_LegalComplianceCheck_Setup_1.1.0.exe) | **🌐 Legal compliance check (Weave-Compliant)** | Dark/light theme dialogs for Daylight, Smoke, and Ventilation Check | 1.1.0 | 2026-07-10 |
| 09 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.2/REXJ_09_LegalComplianceCheck_Setup_1.1.2.exe) | **🌐 Legal compliance check (Weave-Compliant)** | Weave WPF dialogs for all checks; smoke unit fixes and calculation improvements | 1.1.2 | 2026-07-13 |

**Structure (5 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 10 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_10_StructuralMappingTool_Setup_1.0.0.exe) | Structural mapping tool | Maps structural parameters between models or standards | 1.0.0 | 2026-05-13 |
| 11 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_11_STBridgeLink_Setup_1.0.0.exe) | ST-Bridge Link | Legacy ST-Bridge import/export with difference tracking | 1.0.0 | 2026-05-13 |
| 12 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_12_StructuralPlanAndTagFilter_Setup_1.0.0.exe) | Structural plan view + Tag Filter | Structural plan views and structure-aware tag filtering | 1.0.0 | 2026-05-13 |
| 14 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_14_SectionListRC_Setup_1.0.0.exe) | Reinforced Concrete Section List | RC section schedules and listings | 1.0.0 | 2026-05-13 |
| 15 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_15_SectionListSteel_Setup_1.0.0.exe) | Steel Section List | Steel section schedules and listings | 1.0.0 | 2026-05-13 |

**MEP (6 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 16 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_16_DuctPipeLengthTool_Setup_1.0.0.exe) | Duct/Pipe length tool (Quantity Pickup) | Quantifies duct and pipe lengths for estimates and documentation | 1.0.0 | 2026-05-13 |
| 17 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_17_PipeSizing_Setup_1.0.0.exe) | Pipe sizing with flow volume | Pipe size correction driven by flow and engineering inputs | 1.0.0 | 2026-05-13 |
| 18 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_18_MEPEditingTools_Setup_1.0.0.exe) | MEP Editing Tools | Flex duct conversion, connector moves, tee rotation, clash avoidance, duct monitoring | 1.0.0 | 2026-05-13 |
| 20 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_20_FlangeTools_Setup_1.0.0.exe) | Flange Tools | Places flanges and flange-related accessories on MEP runs | 1.0.0 | 2026-05-13 |
| 23 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_23_VerticalSymbol_Setup_1.0.0.exe) | Vertical Symbol (Arrow Mark) | Vertical pipe and duct arrow marks for drawings | 1.0.0 | 2026-05-13 |
| 24 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_24_ConnectionTool_Setup_1.0.0.exe) | Connection Tool | Connects ducts and pipes with guided workflows | 1.0.0 | 2026-05-13 |

**For All Users (12 functions)**

| # | Installer | Function | Description | Version | Last Updated |
|---|-----------|----------|-------------|---------|--------------|
| 27 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_27_ArrayPlacement_Setup_1.0.0.exe) | Array Placement | Pattern placement and array helpers for families | 1.0.0 | 2026-05-13 |
| 28 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_28_ChooseFamilyByParam_Setup_1.0.0.exe) | Choose Family based on parameter | Swaps or picks families according to parameter rules | 1.0.0 | 2026-05-13 |
| 29 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_29_AutoDimension_Setup_1.0.0.exe) | Generate dimension Automatically | Automated dimension chains from model geometry | 1.0.0 | 2026-05-13 |
| **29** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.6/REXJ_29_AutoDimension_Weave_Setup_1.1.6.exe) | **🌐 Generate dimension Automatically / Batch Object Dimension (Weave-Compliant)** | **Dark/light theme WPF dialog, chained dimensions, native Revit warnings for skipped elements** | **1.1.6** | **2026-10-20** |
| 30 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_30_EyedropperTool_Setup_1.0.0.exe) | Eyedropper tool (Copy Parameter) | Copies parameter values between matching elements | 1.0.0 | 2026-05-13 |
| **30** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_Standalone_Setup_1.1.5.exe) | **🌐 Eyedropper tool / Copy Parameter (Weave-Compliant)** | **Dark/light theme WPF dialog  — available in the full bundle only** | **1.1.5** | **2026-10-12** |
| 31 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_31_AdvancedSectionBox_Setup_1.0.0.exe) | Advanced Section box | Enhanced section box manipulation for 3D views | 1.0.0 | 2026-05-13 |
| 31 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.0/REXJ_31_AdvancedSectionBox_Setup_1.1.0.exe) | **🌐 Advanced Section Box (Weave-Compliant)** | Dark/light theme, WPF borderless dialogs | **1.1.0** | **2026-06-04** |
| **32** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.2/REXJ_32_AutoTag_Setup_1.0.2.exe) | **Auto Tag** | **Automated tag placement on views** | **1.0.2** | **2026-05-28** |
| 33 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_33_ViewDuplication_Setup_1.0.0.exe) | View duplication tool | Duplicates views with naming and option presets | 1.0.0 | 2026-05-13 |
| **33** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.7/REXJ_33_ViewDuplication_Weave_Setup_1.1.7.exe) | **🌐 View Duplicate / View Copy (Weave-Compliant)** | **Dark/light theme WPF dialogs, multi-rule batch duplication, run reporting** | **1.1.7** | **2026-10-21** |
| 34 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_34_PrintRegion_Setup_1.0.0.exe) | Print Region tool | Prints user-defined regions on sheets | 1.0.0 | 2026-05-13 |
| 34 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.3/REXJ_Standalone_Setup_1.1.3.exe) | **🌐 Print Region / Range Print (Weave-Compliant)** | Dark/light theme WPF dialog, metric and imperial scale presets, plan-view region crop fix | 1.1.3 | 2026-08-31 |
| 35 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_35_SpreadsheetExportImport_Setup_1.0.0.exe) | Spreadsheet Export/Import | Export to Excel, export schedules, import from Excel, insert Excel images | 1.0.0 | 2026-05-13 |
| 36 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_36_GridElevationDimension_Setup_1.0.0.exe) | Grid/Elevation dimension | Grid dimensions and floor-to-floor height dimensions | 1.0.0 | 2026-05-13 |
| **36** | [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.6/REXJ_36_GridElevationDimension_Weave_Setup_1.1.6.exe) | **🌐 Grid/Elevation dimension — Grid Dimension and Level Height Dimension (Weave-Compliant)** | **Dark/light theme WPF dialogs for Grid Dimension and Level Height Dimension, with a localized vector diagram** | **1.1.6** | **2026-10-20** |
| 37 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_37_MultiQueryFilter_Setup_1.0.0.exe) | Multi query filter (Level Filter) | Advanced filtering including level-based queries | 1.0.0 | 2026-05-13 |
| 37 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.0/REXJ_37_MultiQueryFilter_Setup_1.1.0.exe) | **🌐 Multi Query Filter / Level Filter (Weave-Compliant)** | Dark/light theme, WPF DataGrid, tabbed UI | **1.1.0** | **2026-06-04** |
| 38 | [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_38_SheetAutoLayout_Setup_1.0.0.exe) | Sheet auto layout tool | Automates sheet layouts and viewport placement | 1.0.0 | 2026-05-13 |

**Additional**

| Installer | Function | Description | Version | Last Updated |
|-----------|----------|-------------|---------|--------------|
| [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.0/REXJ_EnhancedSectionBox_Setup_1.1.0.exe) | **🌐 Enhanced Section Box (Weave-Compliant)** | Dark/light theme WPF dialogs for Box View and Section Box Adjustment | **1.1.0** | **2026-06-04** |
| [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_FittingSchedule_Setup_1.1.5.exe) | **🌐 Fitting Schedule / Window Door View (Weave-Compliant)** | Same build as function **#03** (alternate installer name) | **1.1.5** | **2026-10-13** |
| [**Download**](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.5/REXJ_Standalone_Setup_1.1.5.exe) | **🌐 Parameter Filter (Weave-Compliant)** | **Dark/light theme WPF dialog with Weave tabs and progress window  — available in the full bundle only** | **1.1.5** | **2026-10-12** |
| [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.1.0/REXJ_LevelFilter_Setup_1.1.0.exe) | **🌐 Level Filter (Weave-Compliant)** | Per-tool installer for Level Filter | **1.1.0** | **2026-06-04** |
| [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_MepManholeTool_Setup_1.0.0.exe) | Manhole Tool | Places and documents manhole elements for MEP | 1.0.0 | 2026-05-13 |
| [Download](https://github.com/jill-chen-adsk/REXJ-Standalone/releases/download/v1.0.0/REXJ_REXJManager_Setup_1.0.0.exe) | REXJ Manager | Controls visibility of REXJ Standalone ribbon tabs and command presets | 1.0.0 | 2026-05-13 |

### Installation

1. Download the installer(s) above
2. Close Revit 2027 if running
3. Run the installer  -  tools deploy to the Revit 2027 Addins folder automatically
4. Launch Revit 2027  -  tools appear under the **REXJ Standalone** ribbon tab

### Notes

- Set **Opening Coefficient** on window/door family types for smoke and ventilation effective area calculations
- **Range Print** offers metric or imperial scale presets based on the project's unit system, so the scale list differs between mm/m and foot-inch projects
- **Range Print** creates a temporary preview view while the dialog is open and removes it once you print or cancel
- **Auto Floor** requires a floor type to be selected before it creates slabs

