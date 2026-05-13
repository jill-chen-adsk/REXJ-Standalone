using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace MepDuctPipeTool
{
  public class GlobalSettings
  {
    private static GlobalSettings _instance = new();
    private static Document? _document;
    public DockPanelPage? DockPanelPage;
    public ExtEventHandler? ExternalEventHandler;
    public ExternalEvent? ExtEvent;

    public static GlobalSettings Instance
    {
      get => _instance;
    }

    public void Init( Document document )
    {
      _document = document;
    }

    private GlobalSettings()
    {
    }

    public static bool RaiseCmd( Type type )
    {
      Instance.ExternalEventHandler!.SetCommandType( type );
      var externalEventRequest = Instance.ExtEvent!.Raise();
      return externalEventRequest == ExternalEventRequest.Accepted;
    }
  }
}