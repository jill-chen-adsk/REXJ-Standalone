using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListSteel
{
  /// ================================================================================
  /// <summary>コマンド 操作マニュアル</summary>
  /// ================================================================================
  [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
  [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
  public class CmdShowManual : Revit.UI.IExternalCommand
  {
    // メンバ関数
    #region Member Functions
    /// ================================================================================
    /// <summary>コマンド実行処理</summary>
    /// 
    /// <param name="commandData" >Revit コマンドデータ</param>
    /// <param name="message"     >エラーメッセージ</param>
    /// <param name="elemenets"   >エラー要素</param>
    /// 
    /// <returns>実行結果</returns>
    /// 
    /// <history>2016/08/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                   ref string message,
                                   Revit.DB.ElementSet elemenets)
    {
      // 初期化
      Revit.UI.UIApplication                rvtUiApp    = commandData.Application;
      Revit.UI.UIDocument                   rvtUiDoc    = rvtUiApp.ActiveUIDocument;
      Revit.DB.Document                     rvtDbDoc    = rvtUiDoc.Document;
      Revit.ApplicationServices.Application rvtSrvcApp  = rvtUiApp.Application;
      
      SectionListSteel.Components.Attribute cmpAttribute = new SectionListSteel.Components.Attribute();
      
      // 戻り値
      Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;
      
      // 操作マニュアルパス
      // 実行フォルダ
      string filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) +
                        cmpAttribute.ResourceText("IDS_TXT_STEELSECTIONLISTMANUAL");
      
      if (System.IO.File.Exists(filePath))
      {
        System.Diagnostics.Process.Start(filePath);
         
        retCmd = Revit.UI.Result.Succeeded;
      }
      else
      {
        System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_MANUALFILE"));
        
        retCmd = Revit.UI.Result.Failed;
      }

      return retCmd;
    }
    #endregion
  }
}
