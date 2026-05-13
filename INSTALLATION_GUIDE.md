# REXJ Standalone Add-ins for Revit 2027 — Installation & Usage Guide

This guide explains how to install and use the eighteen standalone Revit 2027 add-ins:

| Add-in | Ribbon Panel | Button(s) | Purpose |
|--------|-------------|-----------|---------|
| Enhanced Section Box | Section Box | **Section Box Adjustment**, **Box View** | Adjust 3D section box dimensions and create box views from selected elements |
| Parameter Filter | Parameter Filter | **Parameter Filter** | Filter and select elements by their length parameters |
| Value Copy (Eyedropper) | Value Copy | **Copy Parameter** | Copy parameter values from one element to others |
| Excel Export | Excel | **Export Excel** | Export Revit element data (parameters) to Excel (.xlsx, .xls) or CSV format |
| Excel Import | Excel | **Import Excel** | Import parameter values from Excel back into Revit elements |
| Auto Floor Creation | Auto Floor | **Architecture Floor**, **Structural Floor**, **Foundation Slab** | Automatically create floors/slabs from wall or beam boundaries |
| Join Adjustment | Join | **Join Adjustment** | Adjust element join order and priority between overlapping structural elements |
| Schedule Export | Excel | **Schedule Export** | Export the active schedule (集計表) to Excel with UID, Type, and Instance breakdown sheets |
| Excel Image Insert | Excel | **Excel Image Insert** | Capture the current Excel selection as a bitmap and insert it as an image in the active Revit view |
| Join Order Inspector | Join | **Join Order Inspector** | Visually inspect which elements cut or are cut by a selected element at joins |
| Level Filter | Filter | **Level Filter** | Filter and select elements by category, family, family type, material (parts), or rule filters |
| Room View Creation | Room | **Room View Creation** | Automatically create floor/ceiling plan views cropped to individual room boundaries |
| Range Print | Print | **Range Print** | Print a user-selected rectangular region of any view with full print setup control |
| Auto Tag Placement | Tagging | **Auto Tag Placement** | Automatically place tags on elements with intelligent collision-free positioning |
| Instance Array Layout | Placement | Instance Array Layout | Place family instances in an array pattern within a specified region |
| Auto Sheet Layout | Sheets | **Auto Sheet Layout** | Automatically lay out views and schedules on new sheets based on a reference sheet |
| View Duplicate | Views | **View Duplicate** | Duplicate selected views with custom naming, view types, and view templates |
| Interior Elevation for Room | Views | **Interior Elevation for Room** | Automatically create interior elevation views for rooms |

All eighteen add-ins appear under the shared **REXJ Standalone** ribbon tab, organized into panels: **Section Box**, **Parameter Filter**, **Value Copy**, **Excel**, **Auto Floor**, **Join**, **Filter**, **Room**, **Print**, **Placement**, **Tagging**, **Sheets**, and **Views**.

All eighteen add-ins support **Japanese** (default) and **English** UI. See the [Language / Localization](#language--localization) section for details.

---

## Prerequisites

- **Revit 2027** installed at `C:\Program Files\Autodesk\Revit 2027\`
- **Microsoft Excel** installed (required for Excel Export and Excel Import add-ins — uses COM Interop)
- No other dependencies required — the add-ins are fully self-contained
- Auto Floor Creation requires an `AutomaticFloor.xml` configuration file (included automatically in the build output)

---

## Step 1: Build the Add-ins (if not already built)

Open a terminal (Command Prompt or PowerShell) and run:

```
dotnet build "C:\REXJ\Standalone\REXJ-Standalone.sln" -c "Release 2027" -p:Platform=x64
```

This produces the DLL and `.addin` files in each project's `bin\x64\Release 2027\` folder.

---

## Step 2: Install the Add-ins

### Option A: Copy files to the Revit Addins folder (Recommended)

1. Open Windows Explorer and navigate to your Revit 2027 add-ins folder:

   ```
   %AppData%\Autodesk\Revit\Addins\2027\
   ```

   To go there quickly, press `Win + R`, paste the path above, and press Enter.

2. Create a subfolder for organization (optional but recommended):

   ```
   %AppData%\Autodesk\Revit\Addins\2027\REXJ-Standalone\
   ```

3. Copy the following files from each project's build output into that folder:

   **From** `C:\REXJ\Standalone\EnhancedSectionBox\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.EnhancedSectionBox.dll`
   - `ADSK.JExtRAC.EnhancedSectionBox.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ParameterFilter\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ParameterFilter.dll`
   - `ADSK.JExtRAC.ParameterFilter.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ValueCopy\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ValueCopy.dll`
   - `ADSK.JExtRAC.ValueCopy.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ExportExcel\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ExportExcel.dll`
   - `ADSK.JExtRAC.ExportExcel.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ImportExcel\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ImportExcel.dll`
   - `ADSK.JExtRAC.ImportExcel.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\AutomaticFloor\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.AutomaticFloor.dll`
   - `ADSK.JExtRAC.AutomaticFloor.addin`
   - `en\` folder (English satellite assembly — required for English UI)
   - `Data\` folder (contains `AutomaticFloor.xml` configuration)

   **From** `C:\REXJ\Standalone\SwitchJoinOrder\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.SwitchJoinOrder.dll`
   - `ADSK.JExtRAC.SwitchJoinOrder.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\JoinOrderInspector\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.JoinOrderInspector.dll`
   - `ADSK.JExtRAC.JoinOrderInspector.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ExportSchedule\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ExportSchedule.dll`
   - `ADSK.JExtRAC.ExportSchedule.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ExcelImageInsert\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.ExcelImageInsert.dll`
   - `ADSK.JExtRAC.ExcelImageInsert.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\LevelFilter\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.LevelFilter.dll`
   - `ADSK.JExtRAC.LevelFilter.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\AutoCreateRoomView\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.AutoCreateRoomView.dll`
   - `ADSK.JExtRAC.AutoCreateRoomView.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\PrintRegion\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.PrintRegion.dll`
   - `ADSK.JExtRAC.PrintRegion.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\AutoLayoutTag\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.AutoLayoutTag.dll`
   - `ADSK.JExtRAC.AutoLayoutTag.addin`
   - `en\` folder (English satellite assembly — required for English UI)
   - `Settings.txt` (tag placement distance configuration)
   - `Newtonsoft.Json.dll` (JSON serialization — required)

   **From** `C:\REXJ\Standalone\LayoutInstanceInRegion\bin\x64\Release 2027\`:
   - `ADSK.JExtRAC.LayoutInstanceInRegion.dll`
   - `ADSK.JExtRAC.LayoutInstanceInRegion.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\SheetLayout\bin\x64\Release 2027\`:
   - `ADSK.ViewExtension.SheetLayout.dll`
   - `ADSK.ViewExtension.SheetLayout.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\ViewDuplicate\bin\x64\Release 2027\`:
   - `ADSK.ViewExtension.ViewDuplicate.dll`
   - `ADSK.ViewExtension.ViewDuplicate.addin`
   - `en\` folder (English satellite assembly — required for English UI)

   **From** `C:\REXJ\Standalone\TenkaiView\bin\x64\Release 2027\`:
   - `ADSK.ViewExtension.TenkaiView.dll`
   - `ADSK.ViewExtension.TenkaiView.addin`
   - `en\` folder (English satellite assembly — required for English UI)

4. **If you placed the files in a subfolder**, you must edit each `.addin` file to use the full path to the DLL. Open each `.addin` file in a text editor and change the `<Assembly>` line. For example:

   ```xml
   <!-- Before (works only if .addin is in the same folder as the DLL) -->
   <Assembly>ADSK.JExtRAC.EnhancedSectionBox.dll</Assembly>

   <!-- After (use the full path if .addin is in a different folder) -->
   <Assembly>C:\path\to\your\folder\ADSK.JExtRAC.EnhancedSectionBox.dll</Assembly>
   ```

   **If you placed both the `.addin` and `.dll` files in the same folder**, no changes are needed.

### Option B: Quick install using PowerShell

Run this script in PowerShell to copy everything automatically:

```powershell
$addinsDir = "$env:APPDATA\Autodesk\Revit\Addins\2027\REXJ-Standalone"
New-Item -ItemType Directory -Force -Path $addinsDir | Out-Null

$projects = @(
    "C:\REXJ\Standalone\EnhancedSectionBox\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ParameterFilter\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ValueCopy\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ExportExcel\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ImportExcel\bin\x64\Release 2027",
    "C:\REXJ\Standalone\AutomaticFloor\bin\x64\Release 2027",
    "C:\REXJ\Standalone\SwitchJoinOrder\bin\x64\Release 2027",
    "C:\REXJ\Standalone\JoinOrderInspector\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ExportSchedule\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ExcelImageInsert\bin\x64\Release 2027",
    "C:\REXJ\Standalone\LevelFilter\bin\x64\Release 2027",
    "C:\REXJ\Standalone\AutoCreateRoomView\bin\x64\Release 2027",
    "C:\REXJ\Standalone\PrintRegion\bin\x64\Release 2027",
    "C:\REXJ\Standalone\AutoLayoutTag\bin\x64\Release 2027",
    "C:\REXJ\Standalone\LayoutInstanceInRegion\bin\x64\Release 2027",
    "C:\REXJ\Standalone\SheetLayout\bin\x64\Release 2027",
    "C:\REXJ\Standalone\ViewDuplicate\bin\x64\Release 2027",
    "C:\REXJ\Standalone\TenkaiView\bin\x64\Release 2027"
)

foreach ($proj in $projects) {
    Copy-Item "$proj\*.dll" -Destination $addinsDir -Force
    Copy-Item "$proj\*.addin" -Destination $addinsDir -Force
    # Copy English satellite assemblies
    $enDir = Join-Path $proj "en"
    if (Test-Path $enDir) {
        $destEn = Join-Path $addinsDir "en"
        New-Item -ItemType Directory -Force -Path $destEn | Out-Null
        Copy-Item "$enDir\*" -Destination $destEn -Force
    }
    # Copy Data folder (AutomaticFloor config)
    $dataDir = Join-Path $proj "Data"
    if (Test-Path $dataDir) {
        $destData = Join-Path $addinsDir "Data"
        New-Item -ItemType Directory -Force -Path $destData | Out-Null
        Copy-Item "$dataDir\*" -Destination $destData -Force
    }
    # Copy Settings.txt (AutoLayoutTag config)
    $settingsFile = Join-Path $proj "Settings.txt"
    if (Test-Path $settingsFile) {
        Copy-Item $settingsFile -Destination $addinsDir -Force
    }
}

# Update .addin files to use full DLL paths
Get-ChildItem "$addinsDir\*.addin" | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $dllName = [System.IO.Path]::GetFileNameWithoutExtension($_.Name) + ".dll"
    $fullPath = Join-Path $addinsDir $dllName
    $content = $content -replace "<Assembly>$dllName</Assembly>", "<Assembly>$fullPath</Assembly>"
    Set-Content $_.FullName -Value $content
}

Write-Host "Installation complete. Restart Revit 2027 to load the add-ins."
```

---

## Step 3: Launch Revit 2027

1. Start **Revit 2027**.
2. When Revit loads, you may see a security dialog asking whether to load the add-ins. Click **Always Load** or **Load Once** for each add-in.
3. Look for the **REXJ Standalone** tab in the ribbon at the top of the Revit window.

---

## Using the Add-ins

### Enhanced Section Box

Located in the **REXJ Standalone** tab > **Section Box** panel.

#### Section Box Adjustment

Adjusts the dimensions of an existing 3D section box by entering offset values.

1. Open or switch to a **3D view** in your project.
2. Make sure the **Section Box** is visible (check the Section Box property in the Properties panel).
3. Click **Section Box Adjustment** on the ribbon.
4. A dialog appears showing the current section box dimensions.
5. Enter offset values (in millimeters) for Left, Right, Forward, Back, Top, and Bottom.
6. The section box updates in real-time as you type.
7. Click **OK** to accept the changes, or **Cancel** to revert.

#### Box View

Creates a section box around selected elements.

1. Click **Box View** on the ribbon.
2. A dialog appears with options to select elements:
   - **Object Selection**: Pick individual elements from the model
   - **Link Selection**: Pick elements from linked Revit models
   - **Region Selection**: Draw a rectangular region to select elements
3. After selecting elements, configure the box view options.
4. Click **OK** to create the section box around the selected elements.

---

### Parameter Filter

Located in the **REXJ Standalone** tab > **Parameter Filter** panel.

Filters and selects elements based on their length parameter values.

1. **Select one or more elements** in the model before running the command.
2. Click **Parameter Filter** on the ribbon.
3. A dialog appears showing:
   - A table of all selected elements with their categories, families, types, and length parameters
   - Filter controls for specifying exact values, or minimum/maximum ranges
   - A parameter group settings button to choose which parameter groups to display
4. Enter your filter criteria:
   - **Value**: Enter an exact length value to match
   - **Min / Max**: Enter a range to match elements within that range
5. Click the **Select** button to highlight matching elements in the model.
6. Use the **Connected Elements** option to also find elements that are physically connected to the selected ones (useful for MEP systems).
7. Click **Close** when finished.

---

### Value Copy (Eyedropper)

Located in the **REXJ Standalone** tab > **Value Copy** panel.

Copies parameter values from a source element to one or more target elements.

1. **Select the target elements** (the ones you want to copy values TO) before running the command.
2. Click **Copy Parameter** on the ribbon.
3. You will be prompted to **pick the source element** (the one you want to copy values FROM) in the model view. Click on it.
4. A parameter selection dialog appears showing all parameters of the source element:
   - Each parameter shows its name, group, and current value
   - Check/uncheck the parameters you want to copy
5. Click **OK** to start copying.
6. A **report dialog** appears showing the result for each target element:
   - **Success**: Parameter was copied successfully
   - **Read-Only**: Parameter cannot be modified
   - **Can't Copy**: Parameter types are incompatible
   - **Out of Range**: Value is outside the valid range for the target parameter
7. Click **Error Log** to see detailed error messages if any copies failed.
8. Click **Close** when finished.

---

### Excel Export

Located in the **REXJ Standalone** tab > **Excel** panel.

Exports Revit element parameters to Excel (.xlsx, .xls) or CSV format. Requires Microsoft Excel to be installed.

1. Click **Export Excel** on the ribbon.
2. A mode selection dialog appears — choose the scope of elements to export:
   - **All elements in project** — exports all elements and types in the entire project
   - **Elements in current view** — exports only elements visible in the active view
   - **Currently selected elements** — exports only the elements you have selected
3. The **Excel Export** dialog appears showing:
   - **Select Category** (left panel) — check the categories you want to include
   - **Parameters** (center panel) — available parameters for the checked categories
   - **Output** (right panel) — parameters selected for export
4. Use the **>** and **<** buttons to move parameters between the Parameters and Output lists.
5. Use the **Up** / **Down** arrows to reorder parameters in the Output list.
6. Use **Save** / **Load** / **Restore** buttons to save and reuse parameter configurations.
7. Click **OK** when ready.
8. A **Save As** dialog appears — choose the output file format and location:
   - `.xlsx` — Excel Workbook (default)
   - `.xls` — Legacy Excel format
   - `.csv` — CSV UTF-8 (comma delimited)
9. The export completes and a success message is shown.

> **Note:** Excel Export requires Microsoft Excel to be installed on the machine because it uses COM Interop to create and format workbooks. CSV export does not require Excel.

---

### Excel Import

Located in the **REXJ Standalone** tab > **Excel** panel.

Imports parameter values from an Excel file back into Revit elements. This is the reverse operation of Excel Export — use it to update element parameters in bulk from a spreadsheet. Requires Microsoft Excel to be installed.

1. Click **Import Excel** on the ribbon.
2. **If Excel is already open** with one or more workbooks:
   - If only one workbook with one sheet is open, it is selected automatically.
   - If multiple workbooks/sheets are open, a **Select Excel File to Import** dialog appears. Select the workbook and sheet you want to import from, then click **OK**.
3. **If Excel is not running**, a file dialog opens — browse to and select the Excel file (.xlsx, .xls, or .csv) to import.
4. The import reads the spreadsheet looking for:
   - A **UID** column (containing element unique IDs from the export)
   - **Parameter columns** with `T:` (Type) or `I:` (Instance) prefixes matching Revit parameter names
5. For each row, the tool finds the corresponding Revit element by its UID and updates the parameter values.
6. After import:
   - **Successful cells** are highlighted in gray in the Excel sheet (when Excel is open)
   - A summary message shows the result, including any elements that could not be modified (e.g., elements in groups)

> **Tip:** Use **Excel Export** first to generate a spreadsheet with the correct UID and parameter columns. Edit the values in Excel, then use **Excel Import** to write them back to Revit.
>
> **Note:** Excel Import requires Microsoft Excel to be installed on the machine because it uses COM Interop to read workbooks.

---

### Auto Floor Creation

Located in the **REXJ Standalone** tab > **Auto Floor** panel. Provides three buttons for automatically creating floors and slabs from surrounding walls or beams.

#### Architecture Floor

Creates architectural floor slabs bounded by room-bounding walls.

1. Open or switch to a **Floor Plan**, **Ceiling Plan**, or **Engineering Plan** view.
2. Click **Architecture Floor** on the ribbon.
3. A configuration dialog appears:
   - **Floor Type** — select the floor type to use from the dropdown
   - **Level Height Offset** — enter an offset value in millimeters from the level
   - **Lock to Walls** — check to constrain (align/lock) the floor edges to the walls
   - **Span Direction** — select or enter the span direction angle in degrees (-90 to 90)
4. Click **OK** to accept the settings.
5. **Pick points** in the model — click inside each enclosed wall region where you want a floor created. The tool detects the wall boundary polygon at each pick point and creates the floor automatically.
6. Press **Esc** when done picking points. The created floors will be selected.

#### Structural Floor

Creates structural floor slabs bounded by structural beams.

1. Open or switch to a **Floor Plan**, **Ceiling Plan**, or **Engineering Plan** view.
2. Click **Structural Floor** on the ribbon.
3. A configuration dialog appears (same layout as Architecture Floor):
   - **Floor Type** — select the structural floor type
   - **Level Height Offset** — enter offset in mm
   - **Lock to Beams** — check to constrain floor edges to beams
   - **Span Direction** — select angle
4. Click **OK** then pick points inside beam-bounded regions.
5. Press **Esc** when done.

#### Foundation Slab

Creates foundation slabs bounded by structural beams.

1. Open or switch to a **Floor Plan**, **Ceiling Plan**, or **Engineering Plan** view.
2. Click **Foundation Slab** on the ribbon.
3. A configuration dialog appears:
   - **Slab Type** — select the foundation slab type
   - **Level Height Offset** — enter offset in mm
   - **Lock to Beams** — check to constrain slab edges to beams
   - **Span Direction** — select angle
4. Click **OK** then pick points inside beam-bounded regions.
5. Press **Esc** when done.

> **Note:** The tool calculates wall/beam intersection polygons to determine floor boundaries. The view must contain room-bounding walls (for Architecture) or structural beams with Usage set to "Girder" or "Joist" (for Structural/Foundation). The `AutomaticFloor.xml` configuration file must be present in the same folder as the DLL.

---

### Join Adjustment

Located in the **REXJ Standalone** tab > **Join** panel.

Adjusts the join order and priority between overlapping structural and architectural elements. When two elements intersect, this tool controls which element "cuts" the other at the join.

1. **Optionally select elements** before running the command. If no elements are selected, the tool processes all elements in the active view.
2. Click **Join Adjustment** on the ribbon.
3. The **Join Order Adjustment** dialog appears with two panels:
   - **Category Selection** (left panel) — all element categories found in the model
   - **Join Priority Order** (right panel) — categories added for priority ordering
4. Use the **>>** and **<<** buttons to move categories between the two panels.
5. Use the **Up** / **Down** arrows to reorder categories in the priority list — higher position means higher priority (that category's elements will "cut" lower-priority elements at joins).
6. Click **Detail Adjustment** to fine-tune join order within a category at the family level.
7. Check **Process only joined elements** to skip unjoin/rejoin steps and only switch the existing join order.
8. Click **OK** to execute.
9. A progress bar shows the processing status.
10. If any joins could not be processed, an **Error Log** dialog appears with details. You can save the log to a text file for review.

> **Tip:** Use Detail Adjustment when you need different priority orders for families within the same category (e.g., different wall types).

---

### Join Order Inspector

Located in the **REXJ Standalone** tab > **Join** panel.

Visually inspects the join order of a selected element, showing which joined elements "cut" it and which are "cut by" it. The view is color-coded for easy identification.

1. Click **Join Order Inspector** on the ribbon.
2. You will be prompted to **pick an element** in the model view.
3. The inspector window opens showing the join order analysis:
   - **Elements cutting the selected element** (shown in red in the view) — these elements win at the join and cut into the selected element
   - **Selected element** (shown in yellow in the view)
   - **Elements cut by the selected element** (shown in blue in the view) — these elements lose at the join
4. Click any **element ID button** in the list to select and zoom to that element in the model.
5. Click **Re-analyze** to pick a different element and inspect its join order.

> **Note:** The tool temporarily overrides element colors in the active view to visualize the join hierarchy. The color overrides are applied via a transaction and can be undone with Ctrl+Z. The tool works in any view type but is most useful in plan or 3D views where joined elements are visible.

| Color | Meaning |
|-------|---------|
| Yellow | The selected (inspected) element |
| Red | Elements that CUT the selected element (higher priority) |
| Blue | Elements that are CUT BY the selected element (lower priority) |

---

### Schedule Export

Located in the **REXJ Standalone** tab > **Excel** panel.

Exports the active ViewSchedule (集計表) to Excel with three worksheets: the main schedule, a Type breakdown, and an Instance breakdown. Requires Microsoft Excel to be installed.

1. Open or switch to a **Schedule view** (ViewSchedule) in your project.
2. Click **Schedule Export** on the ribbon.
3. A **Save As** dialog appears with options:
   - **Add date to schedule name** — appends a timestamp to the filename
   - **Each instance breakdown** — controls whether the schedule is itemized
   - **Prioritize import function** / **Prioritize schedule display output** — chooses export mode
4. Choose the save location and file format (`.xlsx` or `.xls`).
5. The export creates an Excel workbook with three sheets:
   - **集計表** — the main schedule data
   - **タイプ内訳** — Type breakdown with linked values
   - **インスタンス内訳** — Instance breakdown with linked values
6. A success message appears when complete.

> **Note:** The active view must be a Schedule view. If not, a message asks you to select a schedule first. Excel must be installed for workbook creation.

---

### Excel Image Insert

Located in the **REXJ Standalone** tab > **Excel** panel.

Captures the current Excel selection as a bitmap image and inserts it into the active Revit view. Useful for pasting Excel tables and charts directly into Revit drawings.

1. **Open Excel** and select the cells or range you want to capture.
2. Switch to **Revit** and click **Excel Image Insert** on the ribbon.
3. The tool automatically:
   - Copies the current Excel selection to the clipboard as a bitmap
   - Saves it as a temporary BMP file
   - Imports the image into the center of the active Revit view
4. The image appears as an `ImageInstance` in the view and can be moved, resized, or deleted like any other Revit image.

> **Note:** Excel must be running with a visible selection before using this command. The tool requires Excel to be open — it does not open Excel or prompt for a file.

---

### Level Filter

Located in the **REXJ Standalone** tab > **Filter** panel.

Filters the current selection by category, family, family type, part material, or rule filters. Provides a tabbed dialog with checkboxes to narrow down the selection.

1. **Select one or more elements** in the model before running the command. You can also select a Group (its members will be expanded).
2. Click **Level Filter** on the ribbon.
3. The **Level Filter** dialog appears with five tabs:
   - **Category** — lists all categories found in the selection with element counts. Check/uncheck to include/exclude.
   - **Family** — lists all families, grouped by category, with element counts.
   - **Family Type** — lists all family types with element counts.
   - **Parts** — lists materials found in Part elements (if any Parts are selected).
   - **Filters** — lists all Rule Filters defined in the project and shows how many of the selected elements match each filter.
4. On each tab:
   - Check the rows you want to keep in the selection.
   - Use **Select All** to check all rows, or **Clear Selection** to uncheck all.
   - The counters at the bottom show the number of checked items and total elements.
5. Click **Apply** (preview) to update the Revit selection to match only the checked items, without closing the dialog.
6. Click **OK** to apply the selection and close the dialog.
7. Click **Cancel** to restore the original selection and close.

> **Note:** If no elements are selected before running the command, an error message will appear asking you to select elements first.

---

### Room View Creation

Located in the **REXJ Standalone** tab > **Room** panel.

Automatically creates floor plan or ceiling plan views cropped to individual room boundaries, with configurable view types, templates, room tags, and crop shapes.

1. Click **Room View Creation** on the ribbon.
2. The **Room View Creation** dialog appears (modeless — you can interact with Revit while it's open):
   - **View Category** — select **Floor Plan** or **Ceiling Plan**
   - **View Type** — choose the plan type (e.g., "Floor Plan", "Reflected Ceiling Plan"). Click **Type Edit** to modify the type properties.
   - **View Template** — select a view template to apply. Click **Manage Templates** to open Revit's template management dialog.
   - **Room Tag** — choose which room tag type to place. Click **Type Edit** to modify tag properties.
   - **Trimming** — configure the crop region:
     - **Shape**: Rectangle (bounding box) or Room Shape (follows room boundary)
     - **Offset**: Enter offset distance in mm to expand/shrink the crop region
   - **Level** list (left) — check levels to filter the room list
   - **Room** list (right) — check the rooms you want to create views for
   - **When a view with the same name exists**: Skip (don't create), Overwrite (delete and recreate), or Copy (create with numbered suffix)
3. Click **OK** to create views and close, or **Apply** to create views and keep the dialog open.
4. Click **Cancel** to close without creating views.

> **Note:** The dialog stays open (modeless) so you can inspect created views while continuing to create more. Each created view is named `{ViewType}_{Level}_{RoomName}`. Rooms that are not in properly enclosed regions will be skipped with a warning message.

### Range Print

Located in the **REXJ Standalone** tab > **Print** panel.

Prints a user-selected rectangular region of the current view with full print setup control, including printer selection, paper size, orientation, zoom, and appearance options.

1. Click **Range Print** on the ribbon.
2. In the Revit view, drag a rectangle to select the print area. A status bar prompt reads "Drag to specify a rectangular area for printing."
3. A preview duplicate view is created showing the cropped region. The **Print** dialog appears:
   - **Printer** — select from installed printers via dropdown
   - **Print Setup** — click to open the full **Print Setup** dialog with:
     - Paper size, source, orientation (portrait/landscape)
     - Paper placement (center or offset from corner with user-defined margins)
     - Hidden line processing (vector or raster)
     - Zoom (fit to page or custom percentage)
     - Appearance (raster quality, colors: monochrome/grayscale/color)
     - Options (view links in blue, hide scope boxes, hide crop boundaries, etc.)
     - Settings management (save, save as, rename, revert, delete)
   - **Print Scale** — choose from standard scales (1:1 to 1:5000) or enter a custom scale in `1:N` format
   - **Update View Scale** — preview the scale change in the duplicate view
4. Click **OK** to print the region and close.
5. Click **Cancel** to close without printing. The temporary duplicate view is automatically deleted.

> **Note:** The print region is defined in the current view's coordinate system. Grid lines that extend beyond the selected region are automatically trimmed to the region boundary. Section views and other annotation elements are hidden in the preview. All changes are rolled back after printing — no permanent modifications are made to the model.

### Auto Tag Placement

Located in the **REXJ Standalone** tab > **Tagging** panel.

Automatically places tags on elements with intelligent collision-free positioning. Tags are placed around elements, checking for intersections with other elements to avoid overlaps.

1. Click **Auto Tag Placement** on the ribbon.
2. The active view must be a **Floor Plan** or **Ceiling Plan** view. If not, an error message is shown.
3. The **Tag Placement Settings** dialog appears with two tabs:

   **Condition Settings tab:**
   - **Target Objects** — choose between:
     - *Selected Objects* — click **Select** to pick elements manually in the view, then return to the dialog
     - *All objects of specified categories* — check categories from the list (Doors, Windows, Walls, Floors, etc.)
   - **Tag Position** — check **Left / Right** and/or **Top / Bottom** to control tag placement directions
   - **Tag Leader Line** — choose *With Leader* or *Without Leader*
   - **Placement Area** — choose *Automatic* (system determines best area) or *Manual* (drag a rectangle in the view)
   - **Existing Tag Processing** — choose *New Only* (skip already-tagged elements), *Replace & Relocate* (delete existing tags and recreate), or *Add More* (place additional tags)

   **Tag Settings tab:**
   - A grid showing categories and their corresponding tag family types (selected via dropdown)
   - **View Template** — displays the current view template name
   - **Save Settings** — saves the current configuration per view template using Revit Extensible Storage

4. Click **Place Tags** to execute tag placement.
5. After placement, if any tags are placed outside the view crop region, an **Error Log** dialog lists their IDs.

> **Note:** Tag placement distances are configured via `Settings.txt` (located alongside the DLL). The file contains four values: distance A (element-to-tag offset), distance B (tag-to-tag offset), and two reserved values. Default values are 5, 30, 10, 10 (in mm). Settings are persisted per view template in Revit Extensible Storage (JSON format).

---

### 15. Instance Array Layout

Located in the **REXJ Standalone** tab > **Placement** panel.

Places family instances in an array pattern within a room/space or a user-specified region.

1. Click **Instance Array Layout** on the ribbon.
2. The command must be run in a **Floor Plan**, **Structural Plan**, or **Ceiling Plan** view.
3. Select a **Room** or **Space** object, or specify a region using a pick box.
4. Configure **placement settings**: **Category**, **Family**, and **Type**.
5. Choose a **placement pattern**:
   - **Equal Spacing** — uniform spacing across the region
   - **Ratio (1:2:1)** — weighted spacing using a 1:2:1 ratio
6. For **X** and **Y**, choose a **method**:
   - **Count-Based** — specify the number of instances
   - **Interval-Based** — specify spacing intervals
7. Set **margins**: **Front**, **Back**, **Left**, and **Right**.
8. Set the **axis angle** and **family rotation angle**, and the **offset from level**.
9. Use the **preview** to review placement before confirming.
10. Optionally **exclude instances outside the region** when applicable.
11. Click **Place** to execute, **Place and Close** to execute and close the dialog, or **Close** to cancel without placing.

---

### 16. Auto Sheet Layout

Located in the **REXJ Standalone** tab > **Sheets** panel.

Automatically creates new sheets and places views/schedules on them, using an existing reference sheet as a layout template.

1. Open or activate a **Sheet** view that you want to use as the reference layout.
2. Click **Auto Sheet Layout** on the ribbon.
3. The dialog shows:
   - **Discipline filter** — filter views by discipline (Architectural, Structural, Mechanical, etc.)
   - **View category** — select the type of views to place (Floor Plan, Ceiling Plan, Section, etc.)
   - **View family type** — further refine by view family type
   - **View list** — shows all matching views; select one or more to place
   - **Sheet creation options** — configure sheet numbering and naming
4. The tool reads the layout of the reference sheet (viewport positions, sizes, and schedule positions).
5. For each selected view, a new sheet is created and the view is placed in the same position as the reference.
6. Schedules from the reference sheet are also duplicated onto each new sheet.
7. Click **OK** to create the sheets, or **Cancel** to abort.

---

### 17. View Duplicate

Located in the **REXJ Standalone** tab > **Views** panel.

Duplicates selected views with custom prefix/suffix naming, view type changes, and view template assignments.

1. Click **View Duplicate** on the ribbon.
2. The dialog shows:
   - **Discipline** dropdown — filter views by discipline
   - **View Category** dropdown — select view type (Floor Plan, Section, Elevation, etc.)
   - **View Family Type** dropdown — optional filter by view family type (or "All")
   - **View list** — select one or more views to duplicate (multi-select supported)
   - **Duplication mode** — choose between **Duplicate** (structure only) or **Duplicate with Detailing**
3. Click **Add** to define a duplication rule:
   - Choose **Prefix** or **Suffix** for naming
   - Enter the text to prepend/append
   - Select a **View Template** to apply to the duplicated view
   - Select a **View Family Type** for the duplicated view
4. Add multiple rules to create multiple copies of each selected view.
5. Click **Delete** to remove a selected rule.
6. Click **OK** to execute duplication, or **Cancel** to abort.

> **Note:** If a view with the same name already exists, the tool automatically appends a number in parentheses, e.g., "Level 1 - Copy(1)".

---

### 18. Interior Elevation for Room

Located in the **REXJ Standalone** tab > **Views** panel.

Automatically creates interior elevation views for selected rooms.

1. Open or switch to a **Floor Plan** view.
2. Click **Interior Elevation for Room** on the ribbon.
3. The dialog shows:
   - **Level** dropdown — select the level to find rooms on
   - **Room list** — displays all rooms on the selected level; select one or more rooms
   - **Elevation View Family** — choose which elevation view family to use
   - **Dimension Style** — select a dimension style to apply (optional)
   - **Options**: crop region size adjustment, far clip offset, section box settings
4. Select the rooms for which you want to create elevation views.
5. Click **Create** to start the process.
6. A progress dialog appears showing the creation status for each room.
7. For each room, the tool creates four elevation views (North, South, East, West) positioned at the room center, with crop regions sized to the room boundaries.
8. Click **Stop** to interrupt the process, or wait for it to complete.
9. Click **Close** when finished.

> **Note:** If elevation views already exist for a selected room, a warning is shown and that room is skipped.

---

## Language / Localization

All eighteen add-ins support **Japanese** and **English** user interfaces. The language is selected **automatically** based on your Windows display language — no manual configuration is needed.

### How it works

| Windows Language | Add-in UI Language |
|------------------|--------------------|
| Japanese (日本語) | Japanese (default) |
| English (any variant) | English |
| Any other language | English (fallback) |

The add-ins use .NET satellite assemblies for localization. The English translations are stored in an `en\` subfolder next to each main DLL:

```
REXJ-Standalone\
├── ADSK.JExtRAC.EnhancedSectionBox.dll
├── ADSK.JExtRAC.ParameterFilter.dll
├── ADSK.JExtRAC.ValueCopy.dll
├── ADSK.JExtRAC.ExportExcel.dll
├── ADSK.JExtRAC.ImportExcel.dll
├── ADSK.JExtRAC.AutomaticFloor.dll
├── ADSK.JExtRAC.SwitchJoinOrder.dll
├── ADSK.JExtRAC.JoinOrderInspector.dll
├── ADSK.JExtRAC.ExportSchedule.dll
├── ADSK.JExtRAC.ExcelImageInsert.dll
├── ADSK.JExtRAC.LevelFilter.dll
├── ADSK.JExtRAC.AutoCreateRoomView.dll
├── ADSK.JExtRAC.PrintRegion.dll
├── ADSK.JExtRAC.AutoLayoutTag.dll
├── ADSK.JExtRAC.LayoutInstanceInRegion.dll
├── ADSK.ViewExtension.SheetLayout.dll
├── ADSK.ViewExtension.ViewDuplicate.dll
├── ADSK.ViewExtension.TenkaiView.dll
├── en\
│   ├── ADSK.JExtRAC.EnhancedSectionBox.resources.dll
│   ├── ADSK.JExtRAC.ParameterFilter.resources.dll
│   ├── ADSK.JExtRAC.ValueCopy.resources.dll
│   ├── ADSK.JExtRAC.ExportExcel.resources.dll
│   ├── ADSK.JExtRAC.ImportExcel.resources.dll
│   ├── ADSK.JExtRAC.AutomaticFloor.resources.dll
│   ├── ADSK.JExtRAC.SwitchJoinOrder.resources.dll
│   ├── ADSK.JExtRAC.JoinOrderInspector.resources.dll
│   ├── ADSK.JExtRAC.ExportSchedule.resources.dll
│   ├── ADSK.JExtRAC.ExcelImageInsert.resources.dll
│   ├── ADSK.JExtRAC.LevelFilter.resources.dll
│   ├── ADSK.JExtRAC.AutoCreateRoomView.resources.dll
│   ├── ADSK.JExtRAC.PrintRegion.resources.dll
│   ├── ADSK.JExtRAC.AutoLayoutTag.resources.dll
│   ├── ADSK.JExtRAC.LayoutInstanceInRegion.resources.dll
│   ├── ADSK.ViewExtension.SheetLayout.resources.dll
│   ├── ADSK.ViewExtension.ViewDuplicate.resources.dll
│   └── ADSK.ViewExtension.TenkaiView.resources.dll
├── Data\
│   └── AutomaticFloor.xml
└── *.addin files
```

### Using the English version

If your Windows display language is already set to English, the add-ins will show English UI automatically after installation. No extra steps are needed.

**If your Windows is set to Japanese but you want English UI**, you can change the display language:

1. Open **Settings** > **Time & Language** > **Language & region**.
2. Under **Windows display language**, select **English**.
3. Sign out and sign back in (or restart) for the change to take effect.
4. Launch Revit — the add-ins will now show English UI.

> **Note:** You do not need to change Revit's language separately. The add-ins follow the Windows display language, not Revit's language setting.

### What changes in English mode

| Element | Japanese | English |
|---------|----------|---------|
| Ribbon buttons | 選択ボックス調整 / ボックスビュー | Section Box Adjustment / Box View |
| Form titles | 選択ボックス調整 / 選択ボックス作成 | Section Box Adjustment / Create Box View |
| Labels (directions) | 左・右・前・後・上・下 | Left / Right / Front / Back / Top / Bottom |
| Error messages | 3Dビューで実行してください。 | Please run this command in a 3D view. |
| Parameter Filter UI | パラメータフィルタ | Parameter Filter |
| Value Copy UI | プロパティ一覧 / レポート | Property List / Report |
| Excel Export dialog | Excelエクスポート | Excel Export |
| Excel Export labels | カテゴリを選択 / パラメーター / 出力 | Select Category / Parameters / Output |
| Excel Export buttons | 検索 / 設定復元 / 設定読込 / 設定保存 / キャンセル | Search / Restore / Load / Save / Cancel |
| Excel Import dialog | インポートするExcelファイルを選択 | Select Excel File to Import |
| Excel Import messages | 読み込みが終了しました / エラー | Import completed / Error |
| Auto Floor — Architecture | 意匠床配置 | Architecture Floor Placement |
| Auto Floor — Structural | 構造床配置 | Structural Floor Placement |
| Auto Floor — Foundation | 基礎スラブ配置 | Foundation Slab Placement |
| Auto Floor labels | レベル高さオフセット / 梁に拘束 / スパン方向 | Level Height Offset / Lock to Beams / Span Direction |
| Auto Floor errors | コマンドが失敗しました / 床が作成できません | Command failed / Could not create floor |
| Join Adjustment title | 結合順位調整 | Join Order Adjustment |
| Join Adjustment labels | カテゴリ選択 / 結合優先順位 / 詳細調整 | Category Selection / Join Priority Order / Detail Adjustment |
| Join Adjustment buttons | OK / キャンセル / 結合している要素のみ処理 | OK / Cancel / Process only joined elements |
| Join Adjustment log | エラーログ / ログ保存 / 閉じる | Error Log / Save Log / Close |
| Join Order Inspector title | 結合順位確認 | Join Order Inspector |
| Join Order Inspector prompt | 結合順位を調べたい要素を選択 | Select an element to inspect join order |
| Join Order Inspector categories | 勝っている要素 / 負けている要素 / 選択した要素 | Elements cutting / Elements cut by / Selected element |
| Join Order Inspector legend | 勝っている (カット側) / 負けている (被カット側) | Cutting (wins over) / Cut by (loses to) |
|| Schedule Export title | エラーログ / 情報 | Error Log / Information |
|| Schedule Export dialog | 名前を付けて保存 / 集計表名称に日付を付ける | Save As / Add date to schedule name |
|| Schedule Export options | インポート機能を優先 / 集計表表示出力を優先 | Prioritize import function / Prioritize schedule display output |
| Excel Image Insert errors | Excelを起動してください / イメージファイルの取り込みに失敗しました | Please start Excel first / Failed to import image file |
| Level Filter title | 階層フィルタ | Level Filter |
| Level Filter tabs | カテゴリ / ファミリ / ファミリタイプ / パーツ / フィルタ | Category / Family / Family Type / Parts / Filters |
| Level Filter buttons | 全選択 / 選択解除 / OK / キャンセル / 適用 | Select All / Clear Selection / OK / Cancel / Apply |
| Level Filter labels | 選択項目数 / 選択総数 / 個数 | Selected Items / Total Selected / Count |
| Level Filter errors | 要素を選択してください。 | Please select elements first. |
| Room View Creation title | 各室ビュー作成 | Room View Creation |
| Room View Creation labels | ビューカテゴリ / ビュータイプ / ビューテンプレート | View Category / View Type / View Template |
| Room View Creation options | 平面図 / 天井伏図 / 矩形 / 部屋形状 | Floor Plan / Ceiling Plan / Rectangle / Room Shape |
| Room View Creation buttons | タイプ編集 / テンプレート管理 / 適用 | Type Edit / Manage Templates / Apply |
| Room View Creation duplicate | スキップ / 上書き / コピー | Skip / Overwrite / Copy |
| Room View Creation errors | 部屋が正しく閉じた領域にないため... | Skipped creating views for rooms not in enclosed regions... |
| Range Print title | 出力 | Print |
| Range Print labels | プリンタ / 印刷スケール / 出力設定 | Printer / Print Scale / Print Setup |
| Range Print buttons | ビュースケール更新 / OK / キャンセル | Update View Scale / OK / Cancel |
| Range Print setup | 用紙 / 向き / 縦 / 横 / 用紙の配置 | Paper / Orientation / Portrait / Landscape / Paper Placement |
| Range Print margins | 中心 / 出力オフセット / 余白なし / プリンタの上限 / ユーザ設定 | Center / Offset from Corner / No Margin / Printer Limit / User Defined |
| Range Print quality | 低 / 標準 / 高 / 仕上 | Low / Medium / High / Presentation |
| Range Print colors | モノクロ / グレースケール / 色 | Monochrome / Grayscale / Color |
| Range Print options | スコープボックスを非表示 / トリミング境界を非表示 | Hide Scope Boxes / Hide Crop Boundaries |
| Auto Tag title | 配置条件確認 | Tag Placement Settings |
| Auto Tag object options | 選択オブジェクト / 現ビューの指定カテゴリ全オブジェクト | Selected Objects / All objects of specified categories |
| Auto Tag position | 左右 / 上下 | Left / Right / Top / Bottom |
| Auto Tag leader | あり / なし | With Leader / Without Leader |
| Auto Tag placement area | 自動判定 / 手動設定 / 領域設定 | Automatic / Manual / Set Area |
| Auto Tag existing tags | 新規のみ / 書換再配置 / 追加配置 | New Only / Replace & Relocate / Add More |
| Auto Tag buttons | タグ配置 / キャンセル / 選択 / 設定保存 | Place Tags / Cancel / Select / Save Settings |
| Auto Tag errors | 平面図・天井伏図ビューで実行して下さい | Please run in a Floor Plan or Ceiling Plan view |
| Auto Tag log | エラーログ / ログ保存 / 閉じる | Error Log / Save Log / Close |
| Instance Array Layout form title | 配列配置 | Instance Array Layout |
| Instance Array Layout group | 配置位置 | Placement Position |
| Instance Array Layout group | 配置設定 | Placement Settings |
| Instance Array Layout pattern | 均等配置 | Equal Spacing |
| Instance Array Layout pattern | 比率配置(1:2:1) | Ratio (1:2:1) |
| Instance Array Layout method | 個数指定配置 | Count-Based |
| Instance Array Layout method | 間隔指定配置 | Interval-Based |
| Instance Array Layout buttons | 配置 / 配置して終了 / 終了 | Place / Place and Close / Close |
| Instance Array Layout labels | カテゴリ / ファミリ / タイプ | Category / Family / Type |
| Auto Sheet Layout title | シート自動配置 | Auto Sheet Layout |
| Auto Sheet Layout errors | 参考となるシートをアクティブにしてから... | Please activate a reference sheet before launching... |
| Auto Sheet Layout disciplines | 意匠 / 構造 / 機械 / 電気 / 衛生 | Architectural / Structural / Mechanical / Electrical / Plumbing |
| View Duplicate title | ビュー複製 | View Duplicate |
| View Duplicate labels | 専門分野 / ビューカテゴリ / ビュータイプ | Discipline / View Category / View Family Type |
| View Duplicate options | 接頭辞 / 接尾辞 | Prefix / Suffix |
| View Duplicate view types | 平面図 / 天井伏図 / 立面図 / 断面図 / 3D | Floor Plan / Ceiling Plan / Elevation / Section / 3D |
| View Duplicate errors | 複製対象のビューを選択してください | Please select target views to duplicate |
| Interior Elevation title | 展開図作成 | Interior Elevation for Room |
| Interior Elevation labels | レベル / 部屋 / ビューファミリ / 寸法スタイル | Level / Room / View Family / Dimension Style |
| Interior Elevation directions | 北 / 南 / 東 / 西 | North / South / East / West |
| Interior Elevation errors | 平面図ビューで実行してください | Please run this from a floor plan view |
| Interior Elevation progress | {0}の展開図を作成しています... | Creating interior elevations for {0}... |

### Troubleshooting English UI

| Problem | Solution |
|---------|----------|
| **UI still shows Japanese on English Windows** | Make sure the `en\` subfolder with `.resources.dll` files is present next to the main DLLs. If it's missing, re-run the build and copy the `en\` folder. |
| **Some text is English but some is still Japanese** | Check that all eighteen `.resources.dll` files are in the `en\` folder. Each add-in has its own satellite assembly. |
| **Want English on a Japanese system without changing Windows language** | This is not supported out of the box. The add-ins follow the OS display language. Changing the Windows display language to English is the recommended approach. |

---

## Uninstalling

To remove the add-ins:

1. Close Revit 2027.
2. Navigate to `%AppData%\Autodesk\Revit\Addins\2027\`
3. Delete the `REXJ-Standalone` folder (or delete the individual `.addin` and `.dll` files).
4. Restart Revit.

To temporarily disable an add-in without uninstalling, rename or delete just its `.addin` file.

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| **REXJ Standalone tab does not appear** | Check that the `.addin` files are in the correct folder and that the `<Assembly>` path points to the actual DLL location. Restart Revit. |
| **"Could not load file or assembly" error** | Verify the DLL files are in the same folder as the `.addin` files, or that the `<Assembly>` path in the `.addin` is correct. |
| **Add-in loads but button does nothing** | Make sure you have the correct view or selection active (e.g., Section Box tools require a 3D view, Parameter Filter requires selected elements). |
| **Security warning on every startup** | Click **Always Load** when prompted by Revit's add-in security dialog. |
| **Japanese text appears garbled** | Ensure your system supports Japanese text rendering (most modern Windows installations do). If you prefer English, see the [Language / Localization](#language--localization) section. |
| **English UI not showing on English Windows** | Ensure the `en\` subfolder with `.resources.dll` files was copied alongside the main DLLs. See [Language / Localization](#language--localization). |

---

## Project Structure Reference

```
C:\REXJ\Standalone\
├── REXJ-Standalone.sln              ← Visual Studio solution file
├── INSTALLATION_GUIDE.md            ← This file
│
├── EnhancedSectionBox\              ← Section Box Extension
│   ├── ADSK.JExtRAC.EnhancedSectionBox.csproj
│   ├── ADSK.JExtRAC.EnhancedSectionBox.addin
│   ├── App.cs                       ← Ribbon registration
│   ├── Commands\                    ← IExternalCommand implementations
│   ├── Common\                      ← Shared utilities
│   ├── Components\                  ← Attribute (resource helper)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   └── Screen\                      ← WinForms UI
│
├── ParameterFilter\                 ← Parameter-Based Selection
│   ├── ADSK.JExtRAC.ParameterFilter.csproj
│   ├── ADSK.JExtRAC.ParameterFilter.addin
│   ├── App.cs
│   ├── Commands\
│   ├── Components\                  ← Attribute, Elements, ProgressBarThread
│   ├── Entities\                    ← Data model classes
│   ├── Resources\                   ← Localized strings and icons
│   └── UI\                          ← WinForms UI
│
├── ValueCopy\                       ← Copy Parameter (Eyedropper)
│   ├── ADSK.JExtRAC.ValueCopy.csproj
│   ├── ADSK.JExtRAC.ValueCopy.addin
│   ├── App.cs
│   ├── Commands\
│   ├── Components\
│   ├── Entities\
│   ├── Resources\
│   ├── Warning\                     ← Failure preprocessor
│   └── UI\                          ← WPF UI (active), WinForms UI (legacy, excluded)
│
├── ExportExcel\                     ← Excel Export
│   ├── ADSK.JExtRAC.ExportExcel.csproj
│   ├── ADSK.JExtRAC.ExportExcel.addin
│   ├── App.cs                       ← Ribbon registration (Excel panel)
│   ├── Commands\                    ← CmdExportExcel (IExternalCommand)
│   ├── Components\                  ← Attribute, Parameters (local replacements)
│   ├── Entities\                    ← CategoryItem, ParameterData, GetData
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN), Image.resx
│   ├── UI\                          ← WinForms export dialog
│   └── Utils\                       ← ExcelUtils (COM Interop), Setting, Common
│
├── ImportExcel\                     ← Excel Import
│   ├── ADSK.JExtRAC.ImportExcel.csproj
│   ├── ADSK.JExtRAC.ImportExcel.addin
│   ├── App.cs                       ← Ribbon registration (Excel panel)
│   ├── Commands\                    ← CmdImportExcel (IExternalCommand)
│   ├── Components\                  ← Attribute, Parameters (local replacements)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← WinForms sheet selection dialog
│   └── Utils\                       ← ExcelUtils (COM Interop, P/Invoke), CultureHelper
│
├── AutomaticFloor\                  ← Auto Floor Creation
│   ├── ADSK.JExtRAC.AutomaticFloor.csproj
│   ├── ADSK.JExtRAC.AutomaticFloor.addin
│   ├── App.cs                       ← Ribbon registration (3 buttons)
│   ├── Commands\                    ← CmdCreateArchitectureFloor, CmdCreateStructuralFloor, CmdFoundationSlab
│   ├── Components\                  ← Attribute, Elements, Geometry (12+ algorithms), Parameters, Settings, Service
│   ├── Data\                        ← AutomaticFloor.xml configuration
│   ├── Entities\                    ← DtBase, DtCmd, DtItems, DtSlabType, SpBase, SpCmd, SpSlabType
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN), Image.resx
│   ├── UI\                          ← WinForms configuration dialog
│   └── Utils\                       ← FloorCreator, Common, CultureHelper
│
├── SwitchJoinOrder\                 ← Join Adjustment
│   ├── ADSK.JExtRAC.SwitchJoinOrder.csproj
│   ├── ADSK.JExtRAC.SwitchJoinOrder.addin
│   ├── App.cs                       ← Ribbon registration (Join panel)
│   ├── Commands\                    ← CmdSwitchJoin (IExternalCommand)
│   ├── Components\                  ← Attribute, Elements (join/overlap detection)
│   ├── Entities\                    ← CategoryItem, CategoryItems, FamilyItem
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN), Image.resx
│   ├── UI\                          ← FormSwitchJoin, FormSettingDetail, FormLog
│   └── Utils\                       ← ProgressDialog, CultureHelper
│
├── JoinOrderInspector\              ← Join Order Inspector
│   ├── ADSK.JExtRAC.JoinOrderInspector.csproj
│   ├── ADSK.JExtRAC.JoinOrderInspector.addin
│   ├── App.cs                       ← Ribbon registration (Join panel)
│   ├── Commands\                    ← CmdJoinOrderInspect (IExternalCommand)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← WPF JoinOrderInspectWindow (XAML + code-behind)
│   └── Utils\                       ← CultureHelper
│
├── ExportSchedule\                  ← Schedule Export
│   ├── ADSK.JExtRAC.ExportSchedule.csproj
│   ├── ADSK.JExtRAC.ExportSchedule.addin
│   ├── App.cs                       ← Ribbon registration (Excel panel)
│   ├── Commands\                    ← CmdExportSchedule (IExternalCommand)
│   ├── Components\                  ← Attribute, Parameters (local replacements)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← FormLog (error log dialog)
│   ├── UserControls\                ← CustomSaveFileDialog, MyUserControl
│   └── Utils\                       ← ScheduleExporter, ExcelUtils, CultureHelper
│
├── ExcelImageInsert\                ← Excel Image Insert
│   ├── ADSK.JExtRAC.ExcelImageInsert.csproj
│   ├── ADSK.JExtRAC.ExcelImageInsert.addin
│   ├── App.cs                       ← Ribbon registration (Excel panel)
│   ├── Commands\                    ← CmdExcelImageInsert (IExternalCommand)
│   ├── Components\                  ← Attribute (resource helper)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   └── Utils\                       ← ExcelHelper, CultureHelper
│
├── AutoCreateRoomView\                  ← Room View Creation
│   ├── ADSK.JExtRAC.AutoCreateRoomView.csproj
│   ├── ADSK.JExtRAC.AutoCreateRoomView.addin
│   ├── App.cs                       ← Ribbon registration (Room panel)
│   ├── Commands\                    ← CmdAutoCreateRoomView (IExternalCommand)
│   ├── Common\                      ← ComDialog (TaskDialog helper)
│   ├── Components\                  ← Attribute, Settings (room area/volume)
│   ├── ExternalEvent\               ← ExternalViewCreate (modeless event handler)
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── Screen\                      ← FormAutoCreateRoomView (WinForms modeless dialog)
│   └── Utils\                       ← CultureHelper
│
├── LevelFilter\                     ← Level Filter
│   ├── ADSK.JExtRAC.LevelFilter.csproj
│   ├── ADSK.JExtRAC.LevelFilter.addin
│   ├── App.cs                       ← Ribbon registration (Filter panel)
│   ├── Commands\                    ← CmdLevelFilter (IExternalCommand)
│   ├── Components\                  ← Attribute, Elements, Parameters, Service, Settings
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← FormLevelFilter (5-tab WinForms dialog)
│   └── Utils\                       ← CultureHelper
│
├── SheetLayout\                     ← Auto Sheet Layout (VB→C# conversion)
│   ├── ADSK.ViewExtension.SheetLayout.csproj
│   ├── ADSK.ViewExtension.SheetLayout.addin
│   ├── App.cs                       ← Ribbon registration (Sheets panel)
│   ├── Commands\                    ← CmdSheetLayout (IExternalCommand)
│   ├── DialogItem\                  ← ItmView, ItmViewDiscipline, ItmViewFamilyType, ItmViewType, ItmViewPort, ItmSchedule
│   ├── Sorter\                      ← CmpLevel, CmpViewGenLevel, CmpVpByNum
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← DlgSheetLayout (WinForms dialog)
│   └── Utils\                       ← DialogUtil, RegistryHelper, CultureHelper
│
├── ViewDuplicate\                   ← View Duplicate (VB→C# conversion)
│   ├── ADSK.ViewExtension.ViewDuplicate.csproj
│   ├── ADSK.ViewExtension.ViewDuplicate.addin
│   ├── App.cs                       ← Ribbon registration (Views panel)
│   ├── Commands\                    ← CmdViewDuplicate (IExternalCommand)
│   ├── DialogItem\                  ← ItmView, ItmViewDiscipline, ItmViewFamilyType, ItmViewType
│   ├── Sorter\                      ← CmpLevel
│   ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
│   ├── UI\                          ← DlgViewDuplicate, DlgViewDuplicateItem (WinForms)
│   └── Utils\                       ← DialogUtil, RegistryHelper, CultureHelper
│
└── TenkaiView\                      ← Interior Elevation for Room (VB→C# conversion)
    ├── ADSK.ViewExtension.TenkaiView.csproj
    ├── ADSK.ViewExtension.TenkaiView.addin
    ├── App.cs                       ← Ribbon registration (Views panel)
    ├── Commands\                    ← CmdTenkaiView (IExternalCommand)
    ├── DialogItem\                  ← ItmViewFamily, ItmRoom, ItmDimStyle
    ├── Sorter\                      ← CmpLevel, CmpLevelElement
    ├── Resources\                   ← Text.resx (JA), Text.en.resx (EN)
    ├── UI\                          ← DlgTenkaiView, DlgCreateTenkaiProcess (WinForms)
    └── Utils\                       ← RoomElevation, CreateTenkaiJoken, DialogUtil, RegistryHelper, CultureHelper
```
