using System;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace PipeSizing.Components
{
  /// <summary>Ribbon creation (icons loaded from the Icons subfolder).</summary>
  internal sealed class UI
  {
    private readonly UIControlledApplication _app;
    private readonly Attribute _attr;
    private readonly string _assemblyPath;
    private readonly string _iconsDir;

    public UI(UIControlledApplication application)
    {
      _app = application;
      _attr = new Attribute();
      _assemblyPath = _attr.ExecuteFile;
      _iconsDir = Path.Combine(Path.GetDirectoryName(_assemblyPath) ?? ".", "Icons");
    }

    public void SetRibbon()
    {
      string assembly = _assemblyPath;
      if (!File.Exists(assembly))
      {
        return;
      }

      string tabName = _attr.ResourceText("IDS_BTN_TABNAME");
      try
      {
        _app.CreateRibbonTab(tabName);
      }
      catch
      {
        // Tab already exists (thrown as Autodesk.Revit.Exceptions.ArgumentException)
      }

      string panelName = _attr.ResourceText("IDS_BTN_SIZING_PANELNAME");
      RibbonPanel ribbonPanel = _app.CreateRibbonPanel(tabName, panelName);

      string contextHelpPath = Path.Combine(Path.GetDirectoryName(_assemblyPath) ?? ".", "Resources", "MEP_PipeSizing_Manual.pdf");
      ContextualHelp contextHelp;
      if (File.Exists(contextHelpPath))
      {
        contextHelp = new ContextualHelp(ContextualHelpType.Url, contextHelpPath);
      }
      else
      {
        contextHelp = new ContextualHelp(ContextualHelpType.Url, "http://help.autodesk.com/view/RVT/2027/ENU/");
      }

      var pulldownData = new PulldownButtonData(
        "PipeSizingPulldown",
        _attr.ResourceText("IDS_BTN_PIPESIZING_NAME"));
      pulldownData.ToolTip = _attr.ResourceText("IDS_BTN_PIPESIZING_TOOLTIP_S");
      pulldownData.Image = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_S.png");
      pulldownData.LargeImage = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_L.png");

      var pulldown = ribbonPanel.AddItem(pulldownData) as PulldownButton;
      if (pulldown == null)
      {
        return;
      }

      pulldown.ToolTip = _attr.ResourceText("IDS_BTN_PIPESIZING_TOOLTIP_S");
      pulldown.LargeImage = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_L.png");
      pulldown.Image = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_S.png");
      pulldown.SetContextualHelp(contextHelp);

      var pushPipe = new PushButtonData(
        "CmdPipeSizing",
        _attr.ResourceText("IDS_BTN_PIPESIZING_TEXT"),
        assembly,
        _attr.ResourceText("IDS_BTN_PIPESIZING_CLASSNAME"));
      pushPipe.ToolTip = _attr.ResourceText("IDS_BTN_PIPESIZING_TOOLTIP_S");
      pushPipe.Image = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_S.png");
      pushPipe.LargeImage = LoadRibbonImageSafe("IDI_BTN_PIPESIZING_L.png");
      pushPipe.SetContextualHelp(contextHelp);
      pulldown.AddPushButton(pushPipe);

      var pushDef = new PushButtonData(
        "CmdEditDefSystem",
        _attr.ResourceText("IDS_BTN_EDITDEFSYSTEM_TEXT"),
        assembly,
        _attr.ResourceText("IDS_BTN_EDITDEFSYSTEM_CLASSNAME"));
      pushDef.ToolTip = _attr.ResourceText("IDS_BTN_EDITDEFSYSTEM_TOOLTIP_S");
      pushDef.Image = LoadRibbonImageSafe("IDI_BTN_EDITDEFSYSTEM_S.png");
      pushDef.LargeImage = LoadRibbonImageSafe("IDI_BTN_EDITDEFSYSTEM_L.png");
      pushDef.SetContextualHelp(contextHelp);
      pulldown.AddPushButton(pushDef);
    }

    private BitmapSource LoadRibbonImageSafe(string fileName)
    {
      try
      {
        string path = Path.Combine(_iconsDir, fileName);
        if (!File.Exists(path))
        {
          return null;
        }

        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(Path.GetFullPath(path));
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
      }
      catch
      {
        return null;
      }
    }
  }
}
