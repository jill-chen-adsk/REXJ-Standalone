using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using Autodesk.Revit.Attributes ;

namespace REXJManager
{
  
  [Transaction( TransactionMode.Manual )]
  public class TabSettingCmd : IExternalCommand
  {
    private static TabSettingWindow _tabSettingWindow ;
    
    /// <summary>
    /// 設定ボタンから呼ばれる外部コマンド
    /// </summary>
    /// <param name="commandData"></param>
    /// <param name="message"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      var uiDoc = commandData.Application.ActiveUIDocument ;
      _tabSettingWindow ??= new TabSettingWindow(uiDoc) ;
      if(!_tabSettingWindow.IsShown) _tabSettingWindow = new TabSettingWindow(uiDoc) ;
      _tabSettingWindow.Show();

      return Result.Succeeded ;
    }
  }
}