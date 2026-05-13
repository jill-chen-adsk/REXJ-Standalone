using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MepDuctPipeTool.Commands
{
  [ Transaction( TransactionMode.Manual ) ]
  public class ShowDockablePaneCmd : IExternalCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      var uiApp = commandData.Application;
      return ShowDockablePane( uiApp );
    }

    public static Result ShowDockablePane( UIApplication uiapp )
    {
      var dpId = new DockablePaneId( new Guid( Resources.DOCKABLE_PANE_ID ) );
      var dp = uiapp.GetDockablePane( dpId );
      dp.Show();
      return Result.Succeeded;
    }
  }
}