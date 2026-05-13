using Autodesk.Revit.UI;
using Quantity.Components;

namespace Quantity
{
  public sealed class ExtApp : IExternalApplication
  {
    private UI _ui;

    public Result OnStartup(UIControlledApplication application)
    {
      EncodingSupport.RegisterCodePages();

      _ui = new UI(application);
      _ui.SetRibbon();
      return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;

    static class EncodingSupport
    {
      static bool _registered;

      internal static void RegisterCodePages()
      {
        if (_registered)
          return;
        try
        {
          System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
          _registered = true;
        }
        catch { }
      }
    }
  }
}
