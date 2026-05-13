using Autodesk.Revit.UI;
using Autodesk.Windows;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// ================================================================================
  /// <summary>UI</summary>
  /// ================================================================================
  class UI
  {
    #region Member Variables

    private Attribute _CmpAttribute;

#if (REVIT2021 || REVIT2022 || REVIT2023)
    private string TabName => _CmpAttribute.ResourceText("IDS_BTN_TABNAME_OLD");
#else
    private string TabName => _CmpAttribute.ResourceText("IDS_BTN_TABNAME");
#endif

    #endregion

    #region Constructor

    public UI(Attribute cmpAttribute, UIControlledApplication rvtUICtrlApp)
    {
      _CmpAttribute = cmpAttribute;
    }

    public UI(Attribute cmpAttribute, UIApplication rvtUIApp)
    {
      _CmpAttribute = cmpAttribute;
    }

    #endregion

    #region Member Functions

    public string GetCurrentMaterialCmbBoxValue()
    {
      string ret = "";

      string tabName = TabName;
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      RibbonCombo cmbBox = null;

      RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

          foreach (Autodesk.Windows.RibbonPanel rbnPanel in rbnPanelCollection)
          {
            if (rbnPanel.Source.AutomationName == pnlName)
            {
              Autodesk.Windows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

              if (item != null)
              {
                cmbBox = item as RibbonCombo;

                break;
              }
            }
          }

          break;
        }
      }

      if (cmbBox != null)
      {
        ComboBoxMemberData memberData = cmbBox.Current as ComboBoxMemberData;

        if (memberData != null)
        {
          ret = memberData.Name;
        }
      }

      return ret;
    }

    public void SetRibbonEnable(bool enable)
    {
      string tabName = TabName;
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

          foreach (Autodesk.Windows.RibbonPanel rbnPanel in rbnPanelCollection)
          {
            if (rbnPanel.Source.AutomationName == pnlName)
            {
              Autodesk.Windows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

              if (item != null)
              {
                item.IsEnabled = enable;
              }
              item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_UPPERLEVEL"), true);

              if (item != null)
              {
                item.IsEnabled = enable;
              }

              item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_TXTBOX_OFFSET"), true);

              if (item != null)
              {
                item.IsEnabled = enable;
              }

              break;
            }
          }

          break;
        }
      }
    }

    #endregion

  }
}
