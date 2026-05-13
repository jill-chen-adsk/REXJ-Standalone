using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using MEPConnectTool.View ;

namespace MEPConnectTool.Commands ;

[Transaction( TransactionMode.Manual )]
public class ConnectElbowCmd : IExternalCommand
{
  public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
  {
    var uiApp = commandData.Application ;
    var window = new ConnectElbowWindow( uiApp ) ;
    window.ShowDialog() ;

    return Result.Succeeded ;
  }
}