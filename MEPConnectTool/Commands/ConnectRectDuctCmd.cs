using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using MEPConnectTool.View ;

namespace MEPConnectTool.Commands ;

[Transaction( TransactionMode.Manual )]
public class ConnectRectDuctCmd : IExternalCommand
{
  // 角ダクト接続コマンド
  public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
  {
    var uiApp = commandData.Application ;
    var window = new ConnectRectDuctWindow(uiApp) ;
    window.ShowDialog() ;
    
    return Result.Succeeded ;
  }
}