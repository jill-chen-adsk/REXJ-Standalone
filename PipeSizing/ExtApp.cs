using Autodesk.Revit.UI;

namespace PipeSizing
{
  [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
  [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
  public class ExtApp : IExternalApplication
  {
    public Result OnStartup(UIControlledApplication application)
    {
      var ui = new Components.UI(application);
      ui.SetRibbon();
      return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
      return Result.Succeeded;
    }
  }
}
