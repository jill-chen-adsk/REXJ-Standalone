using Autodesk.Revit.UI;

namespace RSTExtension
{
    public class ExtApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            var attribute = new Components.Attribute();
            var ui = new Components.UI(attribute, application);
            ui.SetRibbonPanel();
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
