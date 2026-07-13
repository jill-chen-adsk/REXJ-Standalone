; REXJ Standalone - Legal Compliance Check for Rooms Installer for Revit 2027
; Built with Inno Setup 6
; Weave-compliant: WPF dialogs for Daylight, Smoke Exhaust, and Ventilation Check

#define MyAppName "REXJ - Legal Compliance Check for Rooms"
#define MyAppVersion "1.1.2"
#define MyAppPublisher "ADSK REXJ"
#define MyAppURL "https://github.com/jill-chen-adsk/REXJ-Standalone"
#define RevitYear "2027"
#define SrcReleased "..\..\Released"

[Setup]
AppId={{B1000009-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
DefaultDirName={userappdata}\Autodesk\Revit\Addins\{#RevitYear}
DefaultGroupName={#MyAppName}
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=..\Output\ByFunction
OutputBaseFilename=REXJ_09_LegalComplianceCheck_Setup_{#MyAppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayName={#MyAppName} for Revit {#RevitYear}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "{#SrcReleased}\CheckingALVS\*"; DestDir: "{app}\ADSK.JExtRAC.CheckingALVS"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#SrcReleased}\CheckingALVS\3_ADSK.JExtRAC.CheckingALVS.addin"; DestDir: "{app}"; Flags: ignoreversion

[Code]
procedure UpdateAddinFiles();
var
  SearchRec: TFindRec;
  AddinPath, OldAssembly, NewAssembly, SubDirName, DllName: String;
  Lines: TArrayOfString;
  I, TagStart, TagEnd, J: Integer;
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
                DllName := OldAssembly;
                for J := Length(OldAssembly) downto 1 do
                begin
                  if (OldAssembly[J] = '\') or (OldAssembly[J] = '/') then
                  begin
                    DllName := Copy(OldAssembly, J + 1, Length(OldAssembly) - J);
                    break;
                  end;
                end;
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
              'This add-in requires Revit {#RevitYear} to function.' + #13#10#13#10 +
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
Type: filesandordirs; Name: "{app}\ADSK.JExtRAC.CheckingALVS"
