using System;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MepDuctPipeTool.Develop
{
  [ Transaction( TransactionMode.Manual ) ]
  public class TestCmd : IExternalCommand, MyCommand
  {
    public Result Execute( ExternalCommandData commandData, ref string message, ElementSet elements )
    {
      var uiApp = commandData.Application;
      var uiDoc = uiApp.ActiveUIDocument;

      return Execute( uiDoc, true );
    }

    public Result Execute( UIDocument uiDoc, bool isRibbon )
    {
      try
      {
        ExecuteImpl( uiDoc.Document );
      }
      catch ( Exception e )
      {
        Debug.WriteLine( e );
        throw;
      }

      return Result.Succeeded;
    }

    private void ExecuteImpl( Document document )
    {
      using ( var trans = new Transaction( document, "TestCommand" ) )
      {
        trans.Start();

        trans.Commit();
      }
    }
  }
}