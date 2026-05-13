using Autodesk.Revit.UI;

namespace PipeSizing.Components
{
  public sealed class Settings
  {
    public Settings(UIDocument rvtUIDoc)
    {
      RvtUIDoc = rvtUIDoc;
    }

    public UIDocument RvtUIDoc { get; }
  }
}
