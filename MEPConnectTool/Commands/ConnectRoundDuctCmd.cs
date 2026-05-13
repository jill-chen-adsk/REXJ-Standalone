using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using MEPConnectTool.View ;

namespace MEPConnectTool.Commands ;

[Transaction( TransactionMode.Manual )]
public class ConnectRoundDuctCmd : IExternalCommand
{
  public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
  {
    var uiApp = commandData.Application ;
    var window = new ConnectRoundDuctWindow(uiApp) ;
    window.ShowDialog() ;
    
    return Result.Succeeded ;
  }
}