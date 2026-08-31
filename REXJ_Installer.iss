; REXJ Standalone Add-ins Installer for Revit 2027
; Built with Inno Setup 6
; https://jrsoftware.org/isinfo.php

#define MyAppName "REXJ Standalone Add-ins"
#define MyAppVersion "1.1.3"
#define MyAppPublisher "ADSK REXJ"
#define MyAppURL "https://github.com/rexj"
#define RevitYear "2027"

[Setup]
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={userappdata}\Autodesk\Revit\Addins\{#RevitYear}
DefaultGroupName={#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=Installer\Output
OutputBaseFilename=REXJ_Standalone_Setup_{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName={#MyAppName} for Revit {#RevitYear}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Types]
Name: "full"; Description: "Full installation (all tools)"
Name: "compact"; Description: "Compact installation (core tools only)"
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
; --- Building Code Check ---
Name: "codecheck"; Description: "Building Code Check"; Types: full compact
Name: "codecheck\areaschedule"; Description: "Area Schedule (Legal Area / Grounds Expression / Room to Area)"; Types: full compact
Name: "codecheck\avesitelevel"; Description: "Avg Site Level Calculation (Weave-compliant)"; Types: full compact
Name: "codecheck\checkingalvs"; Description: "Daylight / Smoke Exhaust / Ventilation Check"; Types: full compact

; --- Excel Tools ---
Name: "excel"; Description: "Excel Tools"; Types: full
Name: "excel\exportexcel"; Description: "Export to Excel"; Types: full
Name: "excel\exportschedule"; Description: "Export Schedule to Excel"; Types: full
Name: "excel\importexcel"; Description: "Import from Excel"; Types: full
Name: "excel\excelimage"; Description: "Insert Excel Image"; Types: full

; --- Floor Tools ---
Name: "floor"; Description: "Floor Tools"; Types: full
Name: "floor\autofloor"; Description: "Auto Floor Creation"; Types: full
Name: "floor\locateslab"; Description: "Slab Join && Split"; Types: full

; --- Filter Tools ---
Name: "filter"; Description: "Filter Tools"; Types: full compact
Name: "filter\levelfilter"; Description: "Level Filter (Weave-compliant)"; Types: full compact
Name: "filter\paramfilter"; Description: "Parameter Filter"; Types: full compact

; --- Dimension Tools ---
Name: "dimension"; Description: "Dimension Tools"; Types: full
Name: "dimension\autocreatedim"; Description: "Auto Create Dimension"; Types: full
Name: "dimension\griddim"; Description: "Grid Dimension"; Types: full
Name: "dimension\floorheightdim"; Description: "Floor Height Dimension"; Types: full

; --- View Tools ---
Name: "view"; Description: "View Tools"; Types: full
Name: "view\viewduplicate"; Description: "View Duplicate (Copy)"; Types: full
Name: "view\sheetlayout"; Description: "Auto Sheet Layout"; Types: full
Name: "view\tenkaiview"; Description: "Interior Elevation for Room"; Types: full
Name: "view\sectionbox"; Description: "Enhanced Section Box (Weave-compliant)"; Types: full
Name: "view\roomview"; Description: "Room View Creation"; Types: full

; --- Join Tools ---
Name: "join"; Description: "Join Tools"; Types: full
Name: "join\switchjoin"; Description: "Join Adjustment"; Types: full
Name: "join\joinorder"; Description: "Join Order Inspector"; Types: full

; --- Tag & Layout Tools ---
Name: "taglayout"; Description: "Tag && Layout Tools"; Types: full
Name: "taglayout\autotag"; Description: "Auto Tag Placement"; Types: full
Name: "taglayout\layoutinstance"; Description: "Instance Array Layout"; Types: full

; --- Window/Door View ---
Name: "fittingschedule"; Description: "Window/Door View Tools"; Types: full
Name: "fittingschedule\fitting"; Description: "Create && Layout (Doors/Windows) Views (Weave-compliant)"; Types: full

; --- Fukashi (Furring) Tools ---
Name: "fukashi"; Description: "Fukashi (Wall Furring) Tools"; Types: full
Name: "fukashi\fukashi"; Description: "Face Furring && Region Furring"; Types: full

; --- Structural Tools ---
Name: "structural"; Description: "Structural Tools"; Types: full
Name: "structural\mappingtable"; Description: "Mapping Table (ST-Bridge Parameter Mapping)"; Types: full
Name: "structural\stblink"; Description: "STBLink (Legacy ST-Bridge Import/Export with Diff)"; Types: full
Name: "structural\rstextension"; Description: "Structural Extension (Framing Plan / Exclusion Special Mention)"; Types: full
Name: "structural\sectionlistrc"; Description: "Section List RC"; Types: full
Name: "structural\sectionliststeel"; Description: "Section List Steel"; Types: full

; --- MEP Tools ---
Name: "mep"; Description: "MEP Tools"; Types: full
Name: "mep\mepconnecttool"; Description: "Duct/Pipe Connection Tool"; Types: full
Name: "mep\mepmanholetool"; Description: "Manhole Tool"; Types: full
Name: "mep\mepductpipetool"; Description: "Duct/Pipe Flange Tool"; Types: full
Name: "mep\mepverticalmark"; Description: "Vertical Pipe/Duct Arrow Mark Tool"; Types: full
Name: "mep\mepextension"; Description: "MEP Extension (Rotate Tees / Move Connector / Flex Duct Convert / Duct Slope Checker / Duct Displacement)"; Types: full
Name: "mep\pipesizing"; Description: "Pipe Size Correction"; Types: full
Name: "mep\quantity"; Description: "Pipe/Duct Quantity Pickup"; Types: full

; --- REXJ Manager ---
Name: "manager"; Description: "REXJ Manager (Ribbon Tab Visibility)"; Types: full compact
Name: "manager\rexjmanager"; Description: "REXJ Manager"; Types: full compact

; --- Utility Tools ---
Name: "utility"; Description: "Utility Tools"; Types: full
Name: "utility\valuecopy"; Description: "Copy Parameter"; Types: full
Name: "utility\printregion"; Description: "Range Print (Weave-compliant)"; Types: full

; ============================================================
; FILES
; ============================================================
[Files]
; Source base paths
#define SrcReleased "Released"
#define SrcLocal    ""

; --- AreaSchedule ---
Source: "{#SrcReleased}\AreaSchedule\*"; DestDir: "{app}\ADSK.JExtRAC.AreaSchedule"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: codecheck\areaschedule
Source: "{#SrcReleased}\AreaSchedule\1_ADSK.JExtRAC.AreaSchedule.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: codecheck\areaschedule

; --- AveSiteLevelHeightCalc ---
Source: "{#SrcReleased}\AveSiteLevelHeightCalc\*"; DestDir: "{app}\ADSK.JExtRAC.AveSiteLevelHeightCalc"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: codecheck\avesitelevel
Source: "{#SrcReleased}\AveSiteLevelHeightCalc\2_ADSK.JExtRAC.AveSiteLevelHeightCalc.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: codecheck\avesitelevel

; --- CheckingALVS ---
Source: "{#SrcReleased}\CheckingALVS\*"; DestDir: "{app}\ADSK.JExtRAC.CheckingALVS"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: codecheck\checkingalvs
Source: "{#SrcReleased}\CheckingALVS\3_ADSK.JExtRAC.CheckingALVS.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: codecheck\checkingalvs

; --- ExportExcel ---
Source: "{#SrcReleased}\ExportExcel\*"; DestDir: "{app}\ADSK.JExtRAC.ExportExcel"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: excel\exportexcel
Source: "{#SrcReleased}\ExportExcel\1_ADSK.JExtRAC.ExportExcel.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: excel\exportexcel

; --- ExportSchedule ---
Source: "{#SrcReleased}\ExportSchedule\*"; DestDir: "{app}\ADSK.JExtRAC.ExportSchedule"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: excel\exportschedule
Source: "{#SrcReleased}\ExportSchedule\2_ADSK.JExtRAC.ExportSchedule.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: excel\exportschedule

; --- ImportExcel ---
Source: "{#SrcReleased}\ImportExcel\*"; DestDir: "{app}\ADSK.JExtRAC.ImportExcel"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: excel\importexcel
Source: "{#SrcReleased}\ImportExcel\3_ADSK.JExtRAC.ImportExcel.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: excel\importexcel

; --- ExcelImageInsert ---
Source: "{#SrcReleased}\ExcelImageInsert\*"; DestDir: "{app}\ADSK.JExtRAC.ExcelImageInsert"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: excel\excelimage
Source: "{#SrcReleased}\ExcelImageInsert\4_ADSK.JExtRAC.ExcelImageInsert.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: excel\excelimage

; --- AutomaticFloor ---
Source: "{#SrcReleased}\AutomaticFloor\*"; DestDir: "{app}\ADSK.JExtRAC.AutomaticFloor"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: floor\autofloor
Source: "{#SrcReleased}\AutomaticFloor\1_ADSK.JExtRAC.AutomaticFloor.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: floor\autofloor

; --- LocateSlab ---
Source: "{#SrcReleased}\LocateSlab\*"; DestDir: "{app}\ADSK.JExtRAC.LocateSlab"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: floor\locateslab
Source: "{#SrcReleased}\LocateSlab\2_ADSK.JExtRAC.LocateSlab.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: floor\locateslab

; --- LevelFilter ---
Source: "LevelFilter\Released\*"; DestDir: "{app}\ADSK.JExtRAC.LevelFilter"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: filter\levelfilter
Source: "LevelFilter\Released\1_ADSK.JExtRAC.LevelFilter.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: filter\levelfilter

; --- ParameterFilter ---
Source: "ParameterFilter\Released\*"; DestDir: "{app}\ADSK.JExtRAC.ParameterFilter"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: filter\paramfilter
Source: "ParameterFilter\Released\2_ADSK.JExtRAC.ParameterFilter.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: filter\paramfilter

; --- AutoCreateDimension ---
Source: "AutoCreateDimension\Released\*"; DestDir: "{app}\ADSK.JExtRAC.AutoCreateDimension"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: dimension\autocreatedim
Source: "AutoCreateDimension\Released\1_ADSK.JExtRAC.AutoCreateDimension.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: dimension\autocreatedim

; --- GridDimension ---
Source: "GridDimension\Released\*"; DestDir: "{app}\ADSK.JExtRAC.GridDimension"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: dimension\griddim
Source: "GridDimension\Released\2_ADSK.JExtRAC.GridDimension.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: dimension\griddim

; --- FloorHeightDimension ---
Source: "FloorHeightDimension\Released\*"; DestDir: "{app}\ADSK.JExtRAC.FloorHeightDimension"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: dimension\floorheightdim
Source: "FloorHeightDimension\Released\3_ADSK.JExtRAC.FloorHeightDimension.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: dimension\floorheightdim

; --- ViewDuplicate ---
Source: "ViewDuplicate\Released\*"; DestDir: "{app}\ADSK.ViewExtension.ViewDuplicate"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: view\viewduplicate
Source: "ViewDuplicate\Released\1_ADSK.ViewExtension.ViewDuplicate.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: view\viewduplicate

; --- SheetLayout ---
Source: "SheetLayout\Released\*"; DestDir: "{app}\ADSK.ViewExtension.SheetLayout"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: view\sheetlayout
Source: "SheetLayout\Released\2_ADSK.ViewExtension.SheetLayout.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: view\sheetlayout

; --- TenkaiView ---
Source: "TenkaiView\Released\*"; DestDir: "{app}\ADSK.ViewExtension.TenkaiView"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: view\tenkaiview
Source: "TenkaiView\Released\3_ADSK.ViewExtension.TenkaiView.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: view\tenkaiview

; --- EnhancedSectionBox ---
Source: "EnhancedSectionBox\Released\*"; DestDir: "{app}\ADSK.JExtRAC.EnhancedSectionBox"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: view\sectionbox
Source: "EnhancedSectionBox\Released\ADSK.JExtRAC.EnhancedSectionBox.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: view\sectionbox

; --- AutoCreateRoomView ---
Source: "AutoCreateRoomView\Released\*"; DestDir: "{app}\ADSK.JExtRAC.AutoCreateRoomView"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: view\roomview
Source: "AutoCreateRoomView\Released\ADSK.JExtRAC.AutoCreateRoomView.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: view\roomview

; --- SwitchJoinOrder ---
Source: "SwitchJoinOrder\Released\*"; DestDir: "{app}\ADSK.JExtRAC.SwitchJoinOrder"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: join\switchjoin
Source: "SwitchJoinOrder\Released\1_ADSK.JExtRAC.SwitchJoinOrder.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: join\switchjoin

; --- JoinOrderInspector ---
Source: "JoinOrderInspector\Released\*"; DestDir: "{app}\ADSK.JExtRAC.JoinOrderInspector"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: join\joinorder
Source: "JoinOrderInspector\Released\2_ADSK.JExtRAC.JoinOrderInspector.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: join\joinorder

; --- AutoLayoutTag ---
Source: "AutoLayoutTag\Released\*"; DestDir: "{app}\ADSK.JExtRAC.AutoLayoutTag"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: taglayout\autotag
Source: "AutoLayoutTag\Released\1_ADSK.JExtRAC.AutoLayoutTag.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: taglayout\autotag

; --- LayoutInstanceInRegion ---
Source: "LayoutInstanceInRegion\Released\*"; DestDir: "{app}\ADSK.JExtRAC.LayoutInstanceInRegion"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: taglayout\layoutinstance
Source: "LayoutInstanceInRegion\Released\2_ADSK.JExtRAC.LayoutInstanceInRegion.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: taglayout\layoutinstance

; --- FittingSchedule ---
Source: "{#SrcReleased}\FittingSchedule\*"; DestDir: "{app}\ADSK.JExtRAC.FittingSchedule"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: fittingschedule\fitting
Source: "{#SrcReleased}\FittingSchedule\ADSK.JExtRAC.FittingSchedule.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: fittingschedule\fitting

; --- Fukashi ---
Source: "Fukashi\Released\*"; DestDir: "{app}\ADSK.Ext.Fukashi"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: fukashi\fukashi
Source: "Fukashi\Released\ADSK.Ext.Fukashi.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: fukashi\fukashi

; --- MappingTable ---
Source: "MappingTable\Released\*"; DestDir: "{app}\ADSK.RST.MappingTable"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: structural\mappingtable
Source: "MappingTable\Released\ADSK.RST.MappingTable.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: structural\mappingtable

; --- STBLink ---
Source: "STBLink\Released\*"; DestDir: "{app}\STBLink"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: structural\stblink
Source: "STBLink\Released\STBLink.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: structural\stblink

; --- RSTExtension ---
Source: "Released\RSTExtension\*"; DestDir: "{app}\RSTExtension"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: structural\rstextension
Source: "Released\RSTExtension\RSTExtension.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: structural\rstextension

; --- SectionListRC ---
Source: "Released\SectionListRC\*"; DestDir: "{app}\SectionListRC"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: structural\sectionlistrc
Source: "Released\SectionListRC.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: structural\sectionlistrc

; --- SectionListSteel ---
Source: "Released\SectionListSteel\*"; DestDir: "{app}\SectionListSteel"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: structural\sectionliststeel
Source: "Released\SectionListSteel.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: structural\sectionliststeel

; --- MEPConnectTool ---
Source: "Released\MEPConnectTool\*"; DestDir: "{app}\MEPConnectTool"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\mepconnecttool
Source: "Released\MEPConnectTool.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\mepconnecttool

; --- MepManholeTool ---
Source: "Released\MepManholeTool\*"; DestDir: "{app}\MepManholeTool"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\mepmanholetool
Source: "Released\MepManholeTool.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\mepmanholetool

; --- MepDuctPipeTool ---
Source: "Released\MepDuctPipeTool\*"; DestDir: "{app}\MepDuctPipeTool"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\mepductpipetool
Source: "Released\MepDuctPipeTool.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\mepductpipetool

; --- MepVerticalMark ---
Source: "Released\MepVerticalMark\*"; DestDir: "{app}\MepVerticalMark"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\mepverticalmark
Source: "Released\MepVerticalMark.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\mepverticalmark

; --- MEPExtension ---
Source: "Released\MEPExtension\*"; DestDir: "{app}\MEPExtension"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\mepextension
Source: "Released\MEPExtension\MEPExtension.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\mepextension

; --- PipeSizing ---
Source: "Released\PipeSizing\*"; DestDir: "{app}\PipeSizing"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\pipesizing
Source: "Released\PipeSizing\PipeSizing.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\pipesizing

; --- Quantity ---
Source: "Released\Quantity\*"; DestDir: "{app}\Quantity"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: mep\quantity
Source: "Released\Quantity\Quantity.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: mep\quantity

; --- REXJManager ---
Source: "Released\REXJManager\*"; DestDir: "{app}\REXJManager"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: manager\rexjmanager
Source: "Released\REXJManager.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: manager\rexjmanager

; --- ValueCopy ---
Source: "ValueCopy\Released\*"; DestDir: "{app}\ADSK.JExtRAC.ValueCopy"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: utility\valuecopy
Source: "ValueCopy\Released\ADSK.JExtRAC.ValueCopy.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: utility\valuecopy

; --- PrintRegion ---
Source: "PrintRegion\Released\*"; DestDir: "{app}\ADSK.JExtRAC.PrintRegion"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: utility\printregion
Source: "PrintRegion\Released\ADSK.JExtRAC.PrintRegion.addin"; DestDir: "{app}"; Flags: ignoreversion; Components: utility\printregion

; ============================================================
; Update .addin Assembly paths to use install directory
; ============================================================
[Code]
procedure UpdateAddinFiles();
var
  SearchRec: TFindRec;
  AddinPath, OldAssembly, NewAssembly, SubDirName, DllName: String;
  Lines: TArrayOfString;
  I, TagStart, TagEnd, SlashPos, J: Integer;
begin
  if FindFirst(ExpandConstant('{app}\*.addin'), SearchRec) then
  begin
    try
      repeat
        AddinPath := ExpandConstant('{app}\') + SearchRec.Name;
        if LoadStringsFromFile(AddinPath, Lines) then
        begin
          for I := 0 to GetArrayLength(Lines) - 1 do
          begin
            TagStart := Pos('<Assembly>', Lines[I]);
            if TagStart > 0 then
            begin
              TagEnd := Pos('</Assembly>', Lines[I]);
              if TagEnd > TagStart then
              begin
                OldAssembly := Copy(Lines[I], TagStart + 10, TagEnd - TagStart - 10);

                { Extract the DLL filename from the existing path (after last \ or /) }
                DllName := OldAssembly;
                for J := Length(OldAssembly) downto 1 do
                begin
                  if (OldAssembly[J] = '\') or (OldAssembly[J] = '/') then
                  begin
                    DllName := Copy(OldAssembly, J + 1, Length(OldAssembly) - J);
                    break;
                  end;
                end;

                { Determine subdirectory from addin filename }
                SubDirName := SearchRec.Name;
                if (Length(SubDirName) > 2) and (SubDirName[2] = '_') then
                  SubDirName := Copy(SubDirName, 3, Length(SubDirName) - 2);
                SubDirName := Copy(SubDirName, 1, Length(SubDirName) - 6);

                NewAssembly := '    <Assembly>' + ExpandConstant('{app}') + '\' + SubDirName + '\' + DllName + '</Assembly>';
                Lines[I] := NewAssembly;
              end;
            end;
          end;
          SaveStringsToFile(AddinPath, Lines, False);
        end;
      until not FindNext(SearchRec);
    finally
      FindClose(SearchRec);
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    UpdateAddinFiles();
  end;
end;

function InitializeSetup(): Boolean;
var
  RevitPath: String;
begin
  RevitPath := ExpandConstant('{pf}\Autodesk\Revit {#RevitYear}\Revit.exe');
  if not FileExists(RevitPath) then
  begin
    if MsgBox('Revit {#RevitYear} was not detected on this machine.' + #13#10 +
              'The add-ins require Revit {#RevitYear} to function.' + #13#10#13#10 +
              'Do you want to continue the installation anyway?',
              mbConfirmation, MB_YESNO) = IDNO then
    begin
      Result := False;
      Exit;
    end;
  end;
  Result := True;
end;

[UninstallDelete]
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AreaSchedule"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AveSiteLevelHeightCalc"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.CheckingALVS"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ExportExcel"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ExportSchedule"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ImportExcel"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ExcelImageInsert"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AutomaticFloor"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.LocateSlab"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.LevelFilter"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ParameterFilter"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AutoCreateDimension"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.GridDimension"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.FloorHeightDimension"
Type: filesandordirs; Name: "{app}\ADSK.ViewExtension.ViewDuplicate"
Type: filesandordirs; Name: "{app}\ADSK.ViewExtension.SheetLayout"
Type: filesandordirs; Name: "{app}\ADSK.ViewExtension.TenkaiView"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.EnhancedSectionBox"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AutoCreateRoomView"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.SwitchJoinOrder"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.JoinOrderInspector"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.AutoLayoutTag"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.LayoutInstanceInRegion"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.FittingSchedule"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.ValueCopy"
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.PrintRegion"
Type: filesandordirs; Name: "{app}\ADSK.Ext.Fukashi"
Type: filesandordirs; Name: "{app}\ADSK.RST.MappingTable"
Type: filesandordirs; Name: "{app}\STBLink"
Type: filesandordirs; Name: "{app}\RSTExtension"
Type: filesandordirs; Name: "{app}\SectionListRC"
Type: filesandordirs; Name: "{app}\SectionListSteel"
Type: filesandordirs; Name: "{app}\REXJManager"
Type: filesandordirs; Name: "{app}\MEPConnectTool"
Type: filesandordirs; Name: "{app}\MepManholeTool"
Type: filesandordirs; Name: "{app}\MepDuctPipeTool"
Type: filesandordirs; Name: "{app}\MepVerticalMark"
Type: filesandordirs; Name: "{app}\PipeSizing"
Type: filesandordirs; Name: "{app}\Quantity"
Type: filesandordirs; Name: "{app}\MEPExtension"
