using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC
{
  /// ================================================================================
  /// <summary>外部アプリケーション</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class ExtApp : Revit.UI.IExternalApplication
  {
    // メンバ関数
    #region
      /// ================================================================================
      /// <summary>スタートアップ処理</summary>
      /// 
      /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
      /// 
      /// <returns>実行結果</returns>
      /// 
      /// <history>2013/02/04 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public Revit.UI.Result OnStartup(Revit.UI.UIControlledApplication rvtUICtrlApp)
      {
        try
        {
          SectionListRC.Components.Attribute cmpAttribute = new SectionListRC.Components.Attribute();
          SectionListRC.Components.UI cmpUI = new SectionListRC.Components.UI(cmpAttribute, rvtUICtrlApp);
          cmpUI.SetRibbonPanel();
          return Revit.UI.Result.Succeeded;
        }
        catch (Exception ex)
        {
          System.Windows.MessageBox.Show(
            $"SectionListRC failed to load:\n{ex.Message}\n\n{ex.StackTrace}",
            "SectionListRC Error",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error);
          return Revit.UI.Result.Failed;
        }
      }

      /// ================================================================================
      /// <summary>シャットダウン処理</summary>
      /// 
      /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
      /// 
      /// <returns>実行結果</returns>
      /// 
      /// <history>2013/02/04 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public
      Revit.UI.Result OnShutdown(Revit.UI.UIControlledApplication rvtUICtrlApp)
      {
        // 戻り値
        Revit.UI.Result retExtApp = Revit.UI.Result.Cancelled;

        retExtApp = Revit.UI.Result.Succeeded;
        return retExtApp;
      }
    #endregion
  }
}
