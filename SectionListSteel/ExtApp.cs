using System;
using SectionListSteel.Components ;
using Autodesk.Revit.Attributes ;
using Autodesk.Revit.UI ;
using Revit       = Autodesk.Revit;
namespace SectionListSteel
{
  /// ================================================================================
  /// <summary>外部アプリケーション</summary>
  /// ================================================================================
  [Transaction(TransactionMode.Manual)]
  [Regeneration(RegenerationOption.Manual)]
  public class ExtApp : IExternalApplication
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
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Result OnStartup(UIControlledApplication rvtUICtrlApp)
    {
      try
      {
        SectionListSteel.Components.Attribute cmpAttribute = new SectionListSteel.Components.Attribute();
        UI cmpUI = new UI(cmpAttribute, rvtUICtrlApp);
        cmpUI.SetRibbon();
        return Result.Succeeded;
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(
          $"SectionListSteel failed to load:\n{ex.Message}\n\n{ex.StackTrace}",
          "SectionListSteel Error",
          System.Windows.MessageBoxButton.OK,
          System.Windows.MessageBoxImage.Error);
        return Result.Failed;
      }
    }

    /// ================================================================================
    /// <summary>シャットダウン処理</summary>
    /// 
    /// <param name="rvtUICtrlApp">Revit UIコントロールアプリケーション</param>
    /// 
    /// <returns>実行結果</returns>
    /// 
    /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Result OnShutdown(UIControlledApplication rvtUICtrlApp)
    {
      // 戻り値
      Result retExtApp = Result.Cancelled;

      retExtApp = Result.Succeeded;
      return retExtApp;
    }
    #endregion
  }
}
