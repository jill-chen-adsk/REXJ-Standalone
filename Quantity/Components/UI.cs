using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Windows;

namespace Quantity.Components
{
  /// <summary>Ribbon UI for Quantity on the shared REXJ Standalone tab.</summary>
  internal sealed class UI
  {
    private readonly UIControlledApplication _app;
    private readonly Attribute _attr;
    private readonly string _assemblyPath;
    private readonly string _iconsDir;

    private string _internalIdDimCombo = "";
    private string _internalIdTextCombo = "";

    private Elements _cmpElements;

    public UI(UIControlledApplication app)
    {
      _app = app;
      _attr = new Attribute();
      _assemblyPath = _attr.ExecuteFile;
      _iconsDir = Path.Combine(Path.GetDirectoryName(_assemblyPath) ?? ".", "Icons");

      string tabName = _attr.ResourceText("IDS_BTN_TABNAME");
      string panelName = _attr.ResourceText("IDS_BTN_PANELNAME");

      string internalIdHead = _attr.ResourceText("IDS_RVT_INTERNALID_HEAD") + "%" + tabName + "%" +
                                panelName + "%";
      _internalIdDimCombo = internalIdHead + _attr.ResourceText("IDS_BTN_DIMTYPE_NAME");
      _internalIdTextCombo = internalIdHead + _attr.ResourceText("IDS_BTN_TEXTTYPE_NAME");

      app.ViewActivated += RvtUICtrlApp_ViewActivated;
    }

    public void SetRibbon()
    {
      if (!File.Exists(_assemblyPath))
        return;

      string tabName = _attr.ResourceText("IDS_BTN_TABNAME");
      try
      {
        _app.CreateRibbonTab(tabName);
      }
      catch
      {
      }

      string panelName = _attr.ResourceText("IDS_BTN_PANELNAME");
      Autodesk.Revit.UI.RibbonPanel ribbonPanel = null;
      foreach (var p in _app.GetRibbonPanels(tabName))
      {
        if (p.Name == panelName)
        {
          ribbonPanel = p;
          break;
        }
      }
      ribbonPanel ??= _app.CreateRibbonPanel(tabName, panelName);

      ContextualHelp contextHelp;
      string manualRel = _attr.ResourceText("IDS_TXT_JEXTRME_MANUAL").TrimStart('\\', '/')
        .Replace('/', Path.DirectorySeparatorChar);
      string baseDir = Path.GetDirectoryName(_assemblyPath) ?? ".";
      string contextHelpPath = Path.GetFullPath(Path.Combine(baseDir, manualRel));

      if (File.Exists(contextHelpPath))
        contextHelp = new ContextualHelp(ContextualHelpType.Url, contextHelpPath);
      else
        contextHelp = new ContextualHelp(ContextualHelpType.Url, "https://help.autodesk.com/view/RVT/2027/ENU/");

      PushButtonData pushBtnData = new PushButtonData(
          _attr.ResourceText("IDS_BTN_REASON_NAME"),
          _attr.ResourceText("IDS_BTN_REASON_TEXT"),
          _assemblyPath,
          _attr.ResourceText("IDS_BTN_REASON_CLASSNAME"));
      pushBtnData.ToolTip = _attr.ResourceText("IDS_BTN_REASON_TOOLTIP_S");
      pushBtnData.LongDescription = _attr.ResourceText("IDS_BTN_REASON_TOOLTIP_L");
      pushBtnData.Image = LoadPng("IDI_BTN_MTODRAW_S.png", 16);
      pushBtnData.LargeImage = LoadPng("IDI_BTN_MTODRAW_L.png", 32);
      pushBtnData.SetContextualHelp(contextHelp);
      ribbonPanel.AddItem(pushBtnData);

      ribbonPanel.AddSeparator();

      pushBtnData = new PushButtonData(
          _attr.ResourceText("IDS_BTN_QUANTITY_PIPE_NAME"),
          _attr.ResourceText("IDS_BTN_QUANTITY_PIPE_TEXT"),
          _assemblyPath,
          _attr.ResourceText("IDS_BTN_QUANTITY_PIPE_CLASSNAME"));
      pushBtnData.ToolTip = _attr.ResourceText("IDS_BTN_QUANTITY_PIPE_TOOLTIP_S");
      pushBtnData.LongDescription = _attr.ResourceText("IDS_BTN_QUANTITY_PIPE_TOOLTIP_L");
      pushBtnData.Image = LoadPng("IDI_BTN_PIPE_S.png", 16);
      pushBtnData.LargeImage = LoadPng("IDI_BTN_PIPE_L.png", 32);
      pushBtnData.SetContextualHelp(contextHelp);

      var ductBtnData = new PushButtonData(
          _attr.ResourceText("IDS_BTN_QUANTITY_DUCT_NAME"),
          _attr.ResourceText("IDS_BTN_QUANTITY_DUCT_TEXT"),
          _assemblyPath,
          _attr.ResourceText("IDS_BTN_QUANTITY_DUCT_CLASSNAME"));
      ductBtnData.ToolTip = _attr.ResourceText("IDS_BTN_QUANTITY_DUCT_TOOLTIP_S");
      ductBtnData.LongDescription = _attr.ResourceText("IDS_BTN_QUANTITY_DUCT_TOOLTIP_L");
      ductBtnData.Image = LoadPng("IDI_BTN_DUCT_S.png", 16);
      ductBtnData.LargeImage = LoadPng("IDI_BTN_DUCT_L.png", 32);
      ductBtnData.SetContextualHelp(contextHelp);

      ribbonPanel.AddStackedItems(pushBtnData, ductBtnData);
      ribbonPanel.AddSeparator();

      ApplyAdWindowsRibbonLabels(tabName, panelName);

      ribbonPanel.AddSlideOut();
      var dimTypeComboBoxData = new ComboBoxData(_attr.ResourceText("IDS_BTN_DIMTYPE_NAME"));
      dimTypeComboBoxData.SetContextualHelp(contextHelp);
      var textTypeComboBoxData = new ComboBoxData(_attr.ResourceText("IDS_BTN_TEXTTYPE_NAME"));
      textTypeComboBoxData.SetContextualHelp(contextHelp);
      ribbonPanel.AddStackedItems(dimTypeComboBoxData, textTypeComboBoxData);

      WireRibbonCombos(ribbonPanel);
    }

    void ApplyAdWindowsRibbonLabels(string tabName, string panelName)
    {
      try
      {
        RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
        foreach (RibbonTab rbnTab in rbnCtrl.Tabs)
        {
          if (rbnTab.AutomationName != tabName)
            continue;
          foreach (Autodesk.Windows.RibbonPanel rbnPnl in rbnTab.Panels)
          {
            if (rbnPnl.Source.AutomationName != panelName)
              continue;

            Autodesk.Windows.RibbonItem item = rbnPnl.Source.FindItem(_internalIdDimCombo, true);
            if (item is RibbonCombo cmbDimTypes)
            {
              cmbDimTypes.Text = " " + _attr.ResourceText("IDS_BTN_DIMTYPE_NAME") + ":";
              cmbDimTypes.ShowText = true;
              cmbDimTypes.IsToolTipEnabled = false;
            }

            item = rbnPnl.Source.FindItem(_internalIdTextCombo, true);
            if (item is RibbonCombo cmbTextTypes)
            {
              cmbTextTypes.Text = " " + _attr.ResourceText("IDS_BTN_TEXTTYPE_NAME") + ":";
              cmbTextTypes.ShowText = true;
              cmbTextTypes.IsToolTipEnabled = false;
            }

            break;
          }
        }
      }
      catch
      {
      }
    }

    void WireRibbonCombos(Autodesk.Revit.UI.RibbonPanel panel)
    {
      string dimName = _attr.ResourceText("IDS_BTN_DIMTYPE_NAME");
      string txtName = _attr.ResourceText("IDS_BTN_TEXTTYPE_NAME");
      foreach (Autodesk.Revit.UI.RibbonItem ri in panel.GetItems())
      {
        if (ri.ItemType != RibbonItemType.ComboBox)
          continue;
        var cb = (Autodesk.Revit.UI.ComboBox)ri;
        if (cb.Name == dimName)
          cb.DropDownOpened += CboDimensionType_DropDownOpened;
        else if (cb.Name == txtName)
          cb.DropDownOpened += CboTextType_DropDownOpened;
      }
    }

    BitmapSource LoadPng(string fileName, int decodeSize)
    {
      try
      {
        string path = Path.Combine(_iconsDir, fileName);
        if (!File.Exists(path))
          return null;
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.UriSource = new Uri(path);
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        if (decodeSize > 0)
        {
          bmp.DecodePixelWidth = decodeSize;
          bmp.DecodePixelHeight = decodeSize;
        }
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
      }
      catch
      {
        return null;
      }
    }

    public void SetComboBoxValue_Dimension(Document rvtDBDoc)
    {
      using var txGrp = new TransactionGroup(rvtDBDoc, "Document switch");
      txGrp.Start();

      string tabName = _attr.ResourceText("IDS_BTN_TABNAME");
      string pnlName = _attr.ResourceText("IDS_BTN_PANELNAME");

      RibbonCombo cmbDimTypes = null;

      ProjectInfo prjInfo = rvtDBDoc.ProjectInformation;
      Parameter parLastTimeValue = prjInfo.LookupParameter(_attr.ResourceText("IDS_SHPARAM_DEF"));

      RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      foreach (RibbonTab rbnTab in rbnCtrl.Tabs)
      {
        if (rbnTab.AutomationName != tabName)
          continue;
        foreach (Autodesk.Windows.RibbonPanel rbnPnl in rbnTab.Panels)
        {
          if (rbnPnl.Source.AutomationName != pnlName)
            continue;
          Autodesk.Windows.RibbonItem item = rbnPnl.Source.FindItem(_internalIdDimCombo, true);
          if (item != null)
          {
            cmbDimTypes = item as RibbonCombo;
            if (cmbDimTypes != null)
              cmbDimTypes.Items.Clear();
          }
          break;
        }
      }

      if (cmbDimTypes != null)
      {
        using var tx = new Transaction(rvtDBDoc, "Dimension Type");
        if (tx.Start() == Autodesk.Revit.DB.TransactionStatus.Started)
        {
          IList<DimensionType> dimTypes = GetDimensionTypes(rvtDBDoc);
          foreach (DimensionType dimType in dimTypes)
          {
            var memberData = new ComboBoxMemberData(dimType.Id.ToString(), dimType.Name);
            cmbDimTypes.Items.Add(memberData);
          }

          tx.Commit();
        }

        if (cmbDimTypes.Items.Count > 0)
          cmbDimTypes.Current = cmbDimTypes.Items[0];

        string lastDimText = parLastTimeValue?.AsString();
        if (!string.IsNullOrEmpty(lastDimText))
        {
          string[] strAry = lastDimText.Split(',');

          if (strAry.Length >= 2)
          {
            string valName = strAry[0];
            string valId = strAry[1];

            int index = 0;
            foreach (object item in cmbDimTypes.Items)
            {
              if (item is ComboBoxMemberData memberData &&
                  memberData.Name == valId &&
                  memberData.Text == valName)
              {
                cmbDimTypes.Current = cmbDimTypes.Items[index];
                break;
              }
              index++;
            }
          }
        }
      }

      txGrp.Assimilate();
    }

    public void SetComboBoxValue_Text(Document rvtDBDoc)
    {
      using var txGrp = new TransactionGroup(rvtDBDoc, "Document switch");
      txGrp.Start();

      string tabName = _attr.ResourceText("IDS_BTN_TABNAME");
      string pnlName = _attr.ResourceText("IDS_BTN_PANELNAME");

      RibbonCombo cmbTextTypes = null;

      ProjectInfo prjInfo = rvtDBDoc.ProjectInformation;
      Parameter parLastTimeValue = prjInfo.LookupParameter(_attr.ResourceText("IDS_SHPARAM_DEF"));

      RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      foreach (RibbonTab rbnTab in rbnCtrl.Tabs)
      {
        if (rbnTab.AutomationName != tabName)
          continue;
        foreach (Autodesk.Windows.RibbonPanel rbnPnl in rbnTab.Panels)
        {
          if (rbnPnl.Source.AutomationName != pnlName)
            continue;
          Autodesk.Windows.RibbonItem item = rbnPnl.Source.FindItem(_internalIdTextCombo, true);
          if (item != null)
          {
            cmbTextTypes = item as RibbonCombo;
            if (cmbTextTypes != null)
              cmbTextTypes.Items.Clear();
          }
          break;
        }
      }

      if (cmbTextTypes != null)
      {
        using var tx = new Transaction(rvtDBDoc, "Text Type");
        if (tx.Start() == Autodesk.Revit.DB.TransactionStatus.Started)
        {
          IList<TextNoteType> textTypes = GetTextTypes(rvtDBDoc);
          foreach (TextNoteType textType in textTypes)
          {
            var memberData = new ComboBoxMemberData(textType.Id.ToString(), textType.Name);
            cmbTextTypes.Items.Add(memberData);
          }

          tx.Commit();
        }

        if (cmbTextTypes.Items.Count > 0)
          cmbTextTypes.Current = cmbTextTypes.Items[0];

        string lastTxt = parLastTimeValue?.AsString();
        if (!string.IsNullOrEmpty(lastTxt))
        {
          string[] strAry = lastTxt.Split(',');

          if (strAry.Length >= 4)
          {
            string valName = strAry[2];
            string valId = strAry[3];

            int index = 0;
            foreach (object item in cmbTextTypes.Items)
            {
              if (item is ComboBoxMemberData memberData &&
                  memberData.Name == valId &&
                  memberData.Text == valName)
              {
                cmbTextTypes.Current = cmbTextTypes.Items[index];
                break;
              }
              index++;
            }
          }
        }
      }

      txGrp.Assimilate();
    }

    public IList<DimensionType> GetDimensionTypes(Document rvtDBDoc)
    {
      var ret = new List<DimensionType>();
      var fec = new FilteredElementCollector(rvtDBDoc)
        .OfClass(typeof(DimensionType))
        .WhereElementIsElementType();

      foreach (DimensionType dimType in fec)
      {
        if (dimType.StyleType == DimensionStyleType.Linear && dimType.GetOrderedParameters().Count > 0)
          ret.Add(dimType);
      }

      ret.Sort(new DimensionTypeNameComparer());

      return ret;
    }

    public IList<TextNoteType> GetTextTypes(Document rvtDBDoc)
    {
      var ret = new List<TextNoteType>();

      var fec = new FilteredElementCollector(rvtDBDoc).OfClass(typeof(TextNoteType))
        .WhereElementIsElementType();

      foreach (TextNoteType textType in fec)
        ret.Add(textType);

      ret.Sort(new TextTypeNameComparer());

      return ret;
    }

    void RvtUICtrlApp_ViewActivated(object obj, Autodesk.Revit.UI.Events.ViewActivatedEventArgs args)
    {
      View elemView = args.CurrentActiveView;
      Document rvtDBDoc = elemView.Document;
      Autodesk.Revit.ApplicationServices.Application rvtSvcApp = rvtDBDoc.Application;
      var rvtUiApp = new UIApplication(rvtSvcApp);

      UIDocument active = rvtUiApp.ActiveUIDocument;
      _cmpElements = new Elements(active, _attr);
      SetComboBoxValue_Dimension(rvtDBDoc);
      SetComboBoxValue_Text(rvtDBDoc);
    }

    void CboDimensionType_DropDownOpened(object obj, Autodesk.Revit.UI.Events.ComboBoxDropDownOpenedEventArgs args)
    {
      if (_cmpElements != null)
        SetComboBoxValue_Dimension(_cmpElements.RvtDBDoc);
    }

    void CboTextType_DropDownOpened(object obj, Autodesk.Revit.UI.Events.ComboBoxDropDownOpenedEventArgs args)
    {
      if (_cmpElements != null)
        SetComboBoxValue_Text(_cmpElements.RvtDBDoc);
    }
  }

  internal sealed class DimensionTypeNameComparer : IComparer<DimensionType>
  {
    public int Compare(DimensionType a, DimensionType b) =>
      string.Compare(a?.Name, b?.Name, StringComparison.Ordinal);
  }

  internal sealed class TextTypeNameComparer : IComparer<TextNoteType>
  {
    public int Compare(TextNoteType a, TextNoteType b) =>
      string.Compare(a?.Name, b?.Name, StringComparison.Ordinal);
  }
}
