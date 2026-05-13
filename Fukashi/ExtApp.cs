using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi
{
  /// ================================================================================
  /// <summary>外部アプリケーション</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class ExtApp : Revit.UI.IExternalApplication
  {
    // メンバ変数
    #region Member Variables

    RvtExtApp.Components.UI _CmpUI;

    #endregion

    // メンバ関数
    #region
    /// ================================================================================
    /// <summary>スタートアップ処理</summary>
    /// 
    /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
    /// 
    /// <returns>実行結果</returns>
    /// 
    /// <history>2016/11/17 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.UI.Result OnStartup(Revit.UI.UIControlledApplication rvtUICtrlApp)
    {
      // 戻り値
      Revit.UI.Result retExtApp = Revit.UI.Result.Cancelled;

      // 属性
      RvtExtApp.Components.Attribute cmpAttribute = new RvtExtApp.Components.Attribute();

      // UI
      _CmpUI = new RvtExtApp.Components.UI(cmpAttribute, rvtUICtrlApp);

      // リボン設定
      _CmpUI.SetRibbon();

      // イベント追加
      rvtUICtrlApp.ViewActivated += new EventHandler<Revit.UI.Events.ViewActivatedEventArgs>(SetViewActivatedEvent);
      //rvtUICtrlApp.ControlledApplication.DocumentOpened += new EventHandler<Revit.DB.Events.DocumentOpenedEventArgs>(SetDocumentOpendEvent);
      //_CmpUI.AddComboChangedEvent();
      
      retExtApp = Revit.UI.Result.Succeeded;
      return retExtApp;
    }

    /// ================================================================================
    /// <summary>シャットダウン処理</summary>
    /// 
    /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
    /// 
    /// <returns>実行結果</returns>
    /// 
    /// <history>2016/11/17 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.UI.Result OnShutdown(Revit.UI.UIControlledApplication rvtUICtrlApp)
    {
      // 戻り値
      Revit.UI.Result retExtApp = Revit.UI.Result.Cancelled;

      // イベント削除
      rvtUICtrlApp.ViewActivated -= SetViewActivatedEvent;
      //rvtUICtrlApp.ControlledApplication.DocumentOpened -= SetDocumentOpendEvent;
      //_CmpUI.RemoveComboChangedEvent();

      retExtApp = Revit.UI.Result.Succeeded;
      return retExtApp;
    }

    /// ================================================================================
    /// <summary>ドキュメント開始時イベント設定</summary>
    /// 
    /// <param name="obj" >object</param>
    /// <param name="args">event argments</param>
    /// 
    /// <history>2016/11/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void SetDocumentOpendEvent(object obj, Revit.DB.Events.DocumentOpenedEventArgs args)
    {
      // 開かれたドキュメント
      Revit.DB.Document                     rvtDBDoc  = args.Document;
      Revit.ApplicationServices.Application rvtSvcApp = rvtDBDoc.Application;
      Revit.UI.UIApplication                rvtUIApp  = new Revit.UI.UIApplication(rvtSvcApp);
      Revit.UI.UIDocument                   rvtUIDoc  = rvtUIApp.ActiveUIDocument;

      RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);

      // 前回値の設定
      _CmpUI.SetLastTimeValues(cmpElements);
    }

    /// ================================================================================
    /// <summary>ビュー変更時イベント設定</summary>
    /// 
    /// <param name="obj" >object</param>
    /// <param name="args">event argments</param>
    /// 
    /// <history>2016/11/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void SetViewActivatedEvent(object obj, Revit.UI.Events.ViewActivatedEventArgs args)
    {
      // アクティブになったビューのドキュメント
      Revit.DB.Document                     rvtDBDoc  = args.Document;
      Revit.ApplicationServices.Application rvtSvcApp = rvtDBDoc.Application;
      Revit.UI.UIApplication                rvtUIApp  = new Revit.UI.UIApplication(rvtSvcApp);
      Revit.UI.UIDocument                   rvtUIDoc  = rvtUIApp.ActiveUIDocument;

      RvtExtApp.Components.Elements cmpElements = new RvtExtApp.Components.Elements(rvtUIDoc);

      // 前回値の設定
      _CmpUI.SetLastTimeValues(cmpElements);

      // 上部レベルの設定
      _CmpUI.SetUpperLevels(cmpElements);
    }
    
    #endregion
  }
}
