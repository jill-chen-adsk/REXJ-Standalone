using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using AdWindows   = Autodesk.Windows;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Components
{
  /// ================================================================================
  /// <summary>UI</summary>
  /// ================================================================================
  class UI
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private RvtExtApp.Components.Attribute _CmpAttribute;

            
    /// <summary>タブ名</summary>
    #if (REVIT2021 || REVIT2022 || REVIT2023 )
        private string TabName => _CmpAttribute.ResourceText("IDS_BTN_TABNAME_OLD");
    #else
            private string TabName => _CmpAttribute.ResourceText("IDS_BTN_TABNAME");
    #endif
    
    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="cmpAttribute">属性</param>
    /// <param name="rvtUICtrlApp">Revit UI コントロールアプリケーション</param>
    /// 
    /// <history>2016/12/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    UI(RvtExtApp.Components.Attribute cmpAttribute,
       Revit.UI.UIControlledApplication rvtUICtrlApp)
    {
      _CmpAttribute = cmpAttribute;
      _ = rvtUICtrlApp;
    }

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="cmpAttribute">属性</param>
    /// <param name="rvtUIApp"    >Revit UIアプリケーション</param>
    /// 
    /// <history>2016/12/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    UI(RvtExtApp.Components.Attribute cmpAttribute,
       Revit.UI.UIApplication rvtUIApp)
    {
      _CmpAttribute = cmpAttribute;
      _ = rvtUIApp;
    }

    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>マテリアルコンボボックスの値</summary>
    /// 
    /// <history>2016/12/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string GetCurrentMaterialCmbBoxValue()
    {
      // 戻り値
      string ret = "";

      string tabName = TabName;
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      // コンボボックス
      AdWindows.RibbonCombo cmbBox = null;

      #region コンボボックス取得

      // リボン
      AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      // タブ
      AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          // パネル
          AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

          foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
          {
            if (rbnPanel.Source.AutomationName == pnlName)
            {
              AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_MATERIAL"), true);

              if (item != null)
              {
                cmbBox = item as AdWindows.RibbonCombo;

                break;
              }
            }
          }

          break;
        }
      }

      #endregion

      if (cmbBox != null)
      {
        Revit.UI.ComboBoxMemberData memberData = cmbBox.Current as Revit.UI.ComboBoxMemberData;

        if (memberData != null)
        {
          ret = memberData.Name;
        }
        else
        {
          if (cmbBox.Items.Count > 0)
          {
            memberData = cmbBox.Items[0] as Revit.UI.ComboBoxMemberData;
            ret = memberData.Name;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>オフセットテキストボックスの値</summary>
    /// 
    /// <history>2016/12/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string GetCurrentOffsetValue()
    {
      // 戻り値
      string ret = "";

      string tabName = TabName;
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      // テキストボックス
      AdWindows.RibbonTextBox txtBox = null;

      #region テキストボックス取得

      // リボン
      AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      // タブ
      AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          // パネル
          AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

          foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
          {
            if (rbnPanel.Source.AutomationName == pnlName)
            {
              AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_TXTBOX_OFFSET"), true);

              if (item != null)
              {
                txtBox = item as AdWindows.RibbonTextBox;

                break;
              }
            }
          }

          break;
        }
      }

      #endregion

      if (txtBox != null)
      {
        ret = txtBox.TextValue;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>上部レベルコンボボックスの値</summary>
    /// 
    /// <history>2017/01/10 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string GetCurrentUpperLevelCmbBoxValue()
    {
      // 戻り値
      string ret = "";
      
      string tabName = TabName;
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      // コンボボックス
      AdWindows.RibbonCombo cmbBox = null;

      #region コンボボックス取得

      // リボン
      AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;
      // タブ
      AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          // パネル
          AdWindows.RibbonPanelCollection rbnPanelCollection = rbnTab.Panels;

          foreach (AdWindows.RibbonPanel rbnPanel in rbnPanelCollection)
          {
            if (rbnPanel.Source.AutomationName == pnlName)
            {
              AdWindows.RibbonItem item = rbnPanel.Source.FindItem(_CmpAttribute.ResourceText("IDS_RVT_INTERNALID_CMBBOX_UPPERLEVEL"), true);

              if (item != null)
              {
                cmbBox = item as AdWindows.RibbonCombo;

                break;
              }
            }
          }

          break;
        }
      }

      #endregion

      if (cmbBox != null)
      {
        Revit.UI.ComboBoxMemberData memberData = cmbBox.Current as Revit.UI.ComboBoxMemberData;

        if (memberData != null)
        {
          ret = memberData.Name;
        }
        else
        {
          if (cmbBox.Items.Count > 0)
          {
            memberData = cmbBox.Items[0] as Revit.UI.ComboBoxMemberData;
            ret = memberData.Name;
          }
        }
      }
      
      return ret;
    }

    #endregion
  }
}
