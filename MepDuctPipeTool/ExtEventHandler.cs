using System;
using Autodesk.Revit.UI;

namespace MepDuctPipeTool
{
  public class ExtEventHandler : IExternalEventHandler
  {
    private Type? _cmdType;

    public void Execute( UIApplication app )
    {
      if ( _cmdType == null ) return;
      var cmd = Activator.CreateInstance( _cmdType ) as MyCommand;
      cmd!.Execute( app.ActiveUIDocument, false );
    }

    public string GetName()
    {
      return _cmdType?.Name ?? "";
    }

    public void SetCommandType( Type type )
    {
      _cmdType = type;
    }
  }
}