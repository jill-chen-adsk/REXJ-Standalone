using Autodesk.Revit.Attributes ;
using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using MEPConnectTool.Utils ;
using MEPConnectTool.View ;

namespace MEPConnectTool.Commands ;

[Transaction( TransactionMode.Manual )]
public class Connect45degDuctPipe : IExternalCommand
{
  public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
  {
    var uiApp = commandData.Application ;
    uiApp.BeginConnect45degDuctPipe();
    
    return Result.Succeeded ;
  }
}